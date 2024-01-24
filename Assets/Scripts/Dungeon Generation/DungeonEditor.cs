using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using System;
using UnityEditor.ShaderGraph;

public class DungeonEditor : EditorWindow {

  public int tabsAmount = 3;
  readonly string[] tabsName = new string[] {"Dungeon Settings", "Room Types", "How it works?"};

  private enum WindowSection {Settings, Types, Information}
  private WindowSection currentSection = WindowSection.Settings;
  private DungeonSettings dungeonSettings;


  // -------------- FOLDOUT --------------
  private bool showRoomSize = true;
  GUIContent roomSizeFoldout;

  private bool showDungeonConnexions = true;
  GUIContent dungeonConnexionFoldout;


  // -------------- EDITOR --------------

  [MenuItem("New Dungeon/3D Dungeon Generation")]
  private static void ShowWindow() {
    DungeonEditor dungeonEditor = (DungeonEditor)GetWindow(typeof(DungeonEditor));
    dungeonEditor.minSize = new Vector2(640, 360);
    dungeonEditor.maxSize = new Vector2(1920, 1080);

    dungeonEditor.titleContent = new GUIContent("3D Dungeon Generation");
  }

  private void OnEnable() {
    dungeonSettings = new();

    // foldoutHeaderStyle = new GUIStyle(EditorStyles.foldoutHeader) {
    //   fontStyle = FontStyle.Bold
    // };
  }

  private void OnGUI() {

    GUILayout.Space(25);

    GUILayout.BeginHorizontal();

      if(GUILayout.Button(tabsName[0], GUILayout.Height(50))) 
        currentSection = WindowSection.Settings;
      if(GUILayout.Button(tabsName[1], GUILayout.Height(50))) 
        currentSection = WindowSection.Types;
      if(GUILayout.Button(tabsName[2], GUILayout.Height(50))) 
        currentSection = WindowSection.Information;

    GUILayout.EndHorizontal();

    SeparatorLine(Color.white);

    switch (currentSection) {
      case WindowSection.Settings:
        ShowSettings();
        break;
      case WindowSection.Types:
        ShowTypes();
        break;
      case WindowSection.Information:
        ShowInformation();
        break;
    }
  }

  private void ShowSettings() {

    GUIStyle customFoldoutStyle = new(EditorStyles.foldoutHeader);
    customFoldoutStyle.padding.left = 0;

    GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      GUILayout.Label("Dungeon Settings", EditorStyles.boldLabel);
      GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();

    GUILayout.Space(10);

    GUILayout.BeginHorizontal();
      roomSizeFoldout = EditorGUIUtility.TrIconContent(showRoomSize ? "d_Toolbar Minus" : "d_Toolbar Plus");
      roomSizeFoldout.text = "Room Size Settings";
      showRoomSize = GUILayout.Toggle(showRoomSize, roomSizeFoldout, customFoldoutStyle, GUILayout.ExpandWidth(false));
    GUILayout.EndHorizontal();
    
    if(showRoomSize) {
      EditorGUI.indentLevel++;

      dungeonSettings.roomSize = Mathf.Max(1, EditorGUILayout.IntField(new GUIContent("Room Size", "X and Z size of every single room."), dungeonSettings.roomSize));
      dungeonSettings.minRooms = Mathf.Max(1, EditorGUILayout.IntField(new GUIContent("Min Rooms", "Minimum amount of room that should be generated."), dungeonSettings.minRooms));
      dungeonSettings.maxRooms = Mathf.Max(1, EditorGUILayout.IntField(new GUIContent("Max Rooms", "Maximum amount of room that should be generated."), dungeonSettings.maxRooms));


      EditorGUI.BeginDisabledGroup(true);
        dungeonSettings.nbRooms = EditorGUILayout.IntField(new GUIContent("Active Rooms: ", "How much room have been generated so far."), dungeonSettings.nbRooms);
      EditorGUI.EndDisabledGroup();

      EditorGUI.indentLevel--;
    }

    GUILayout.Space(10);

    GUILayout.BeginHorizontal();
      dungeonConnexionFoldout = EditorGUIUtility.TrIconContent(showDungeonConnexions ? "d_Toolbar Minus" : "d_Toolbar Plus");
      dungeonConnexionFoldout.text = "Dungeon Connexions Settings";
      showDungeonConnexions = GUILayout.Toggle(showDungeonConnexions, dungeonConnexionFoldout, customFoldoutStyle, GUILayout.ExpandWidth(false));
    GUILayout.EndHorizontal();

    if(showDungeonConnexions) {
      EditorGUI.indentLevel++;

      dungeonSettings.heightVariation = Mathf.Max(0, EditorGUILayout.IntField(new GUIContent("Height Variation: ", "How much units the next room will be generated on the Y axis."), dungeonSettings.heightVariation));
      dungeonSettings.heightVariationChance = EditorGUILayout.IntSlider(new GUIContent("Height Variation Chance: ", "Chances for the next room to generate higher based on 'heightVariation' variable."), dungeonSettings.heightVariationChance, 0, 100);
      dungeonSettings.dungeonSpread = EditorGUILayout.IntSlider(new GUIContent("Dungeon Spread: ", "How much the dungeon will spread on the X and Z axis. (0 = no gap between rooms | 3 = looks like tree branches (harder to generate))"), dungeonSettings.dungeonSpread, 0, 3);

      EditorGUI.indentLevel--;
    }
  }


  private void ShowTypes() {
    GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      GUILayout.Label("Different Room Types", EditorStyles.boldLabel);
      GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();

    GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      if(GUILayout.Button("New Room Type")) {
        ShowPopupRoomType();
      }
      GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();
    
    SeparatorLine(Color.white);

    List<RoomType> listRoomType = LoadRoomType();

    foreach (RoomType roomType in listRoomType) {
      GUILayout.BeginHorizontal();

      GUIStyle customFoldoutStyle = new(EditorStyles.foldoutHeader);
      customFoldoutStyle.padding.left = 0;

      GUIContent foldoutContent = EditorGUIUtility.TrIconContent(roomType.isExpanded ? "d_Toolbar Minus@2x" : "d_Toolbar Plus@2x");
      foldoutContent.text = "";

      roomType.isExpanded = GUILayout.Toggle(roomType.isExpanded, foldoutContent, customFoldoutStyle, GUILayout.Width(50));

      GUILayout.BeginVertical();

      if(roomType.isExpanded) DisplayExpandedElement(roomType);
      else DisplayClosedElement(roomType);

      GUILayout.EndVertical();
      GUILayout.EndHorizontal();

      SeparatorLine(Color.gray);
    }
  }

  void DisplayExpandedElement(RoomType roomType) {
    GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      GUILayout.Label("Editor Display Name: ", EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
      roomType.displayName = GUILayout.TextField(roomType.displayName, GUILayout.Width(150));
      GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();

    GUILayout.Space(20);

    GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      GUILayout.Label("Required Room: ", EditorStyles.boldLabel);
      roomType.requiredRoom = GUILayout.Toggle(roomType.requiredRoom, "");
      GUILayout.FlexibleSpace();
      GUILayout.Label("Optional Room: ", EditorStyles.boldLabel);
      roomType.optionalRoom = GUILayout.Toggle(roomType.optionalRoom, "");
      GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();
  }

  void DisplayClosedElement(RoomType roomType) {
    GUILayout.BeginHorizontal();
      GUILayout.Label(roomType.displayName, EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
      GUILayout.FlexibleSpace();
      if(GUILayout.Button("Delete", EditorStyles.boldLabel)) ShowPopupDelete(roomType);
      GUILayout.Space(50);
    GUILayout.EndHorizontal();
  }

  void SeparatorLine(Color color) {
    GUILayout.Space(25);
    Handles.color = color;
    Handles.DrawLine(new Vector3(0, GUILayoutUtility.GetLastRect().yMax), new Vector3(Screen.width, GUILayoutUtility.GetLastRect().yMax));
    GUILayout.Space(25);
  }

  private void ShowInformation() {
    GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      GUILayout.Label("How does the generation works?", EditorStyles.boldLabel);
      GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();
  }

  private void ShowPopupRoomType() {
    PopupWindow.Show(new Rect(Screen.width/2 - 100, 180, 200, 0), new CreateRoomTypePopup(this));
  }

  private class CreateRoomTypePopup : PopupWindowContent {
    private DungeonEditor dungeonEditorPopup;
    private string assetName = "NewRoomType";

    public CreateRoomTypePopup(DungeonEditor dungeonEditor) {
      dungeonEditorPopup = dungeonEditor;
    }

    public override void OnGUI(Rect rect) {
      GUILayout.Label("Enter Asset Name:");
      assetName = EditorGUILayout.TextField(assetName);

      Event e = Event.current;
      if(GUILayout.Button("Create")
      || (e.isKey && e.keyCode == KeyCode.Return)) {
        dungeonEditorPopup.CreateRoomType(assetName);
        dungeonEditorPopup.Repaint();

        editorWindow.Close();
      }
    }
  }
  
  private void CreateRoomType(string assetName) {
    assetName = assetName.Replace(" ", "");
    RoomType roomType = ScriptableObject.CreateInstance<RoomType>();

    string ResourcesfolderPath = "Assets/Resources";
    if (!AssetDatabase.IsValidFolder(ResourcesfolderPath)) {
        AssetDatabase.CreateFolder("Assets", "Resources");
    }

    string RoomTypefolderPath = "Assets/Resources/RoomType";
    if (!AssetDatabase.IsValidFolder(RoomTypefolderPath)) {
        AssetDatabase.CreateFolder("Assets/Resources", "RoomType");
    }

    string assetPath = $"Assets/Resources/RoomType/{assetName}.asset";
    AssetDatabase.CreateAsset(roomType, assetPath);
    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh();
  }

  private void ShowPopupDelete(RoomType roomType) {
    PopupWindow.Show(new Rect(Screen.width/2 - 100, 180, 200, 0), new DeletePopup(roomType));
  }

  private class DeletePopup : PopupWindowContent {
    private RoomType roomType;

    public DeletePopup(RoomType roomType) {
      this.roomType = roomType;
    }

    public override void OnGUI(Rect rect) {
      GUIStyle redButton = new(GUI.skin.button);
      redButton.normal.textColor = new(1, 0.5f, 0.5f);

      GUILayout.FlexibleSpace();
      GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      Event e = Event.current;
      if(GUILayout.Button("CONFIRM DELETE ???", redButton, GUILayout.Width(150), GUILayout.Height(150))
      || (e.isKey && e.keyCode == KeyCode.Return)) {
        string assetPath = AssetDatabase.GetAssetPath(roomType);
        AssetDatabase.DeleteAsset(assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        editorWindow.Close();
      }
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();
      GUILayout.FlexibleSpace();
    }
  }

  private List<RoomType> LoadRoomType() {
    List<RoomType> list = new();
    string folderPath = "RoomType";
    RoomType [] r_roomType = Resources.LoadAll<RoomType>(folderPath);
    foreach(RoomType roomType in r_roomType) {
      string assetPath = AssetDatabase.GetAssetPath(roomType);
      if(roomType.displayName == "" || roomType.displayName == null) roomType.displayName = assetPath.Substring(26).Remove(assetPath[26..].Length - 6);
      list.Add(roomType);
    }
    return list;
  }
}