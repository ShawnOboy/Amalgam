using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using System;
using UnityEditor.ShaderGraph;
using Unity.VisualScripting;
using System.Reflection;

public class DungeonEditor : EditorWindow {

  public int tabsAmount = 3;
  readonly string[] tabsName = new string[] {"Dungeon Settings", "Room Types", "How it works?"};

  private enum WindowSection {Settings, Types, Information}
  private WindowSection currentSection = WindowSection.Settings;
  private DungeonSettings dungeonSettings;
  private DungeonSettings originalSettings;


  // -------------- FOLDOUT --------------
  private bool showRoomSize = true;
  private bool showDungeonConnexions = true;

  private Vector2 scrollPosition = Vector2.zero;
  private Vector2 settingsPresetScroll = Vector2.zero;


  // -------------- EDITOR --------------

  [MenuItem("Tools/3D Dungeon Generation")]
  private static void ShowWindow() {
    DungeonEditor dungeonEditor = (DungeonEditor)GetWindow(typeof(DungeonEditor));
    dungeonEditor.minSize = new Vector2(640, 525);
    dungeonEditor.maxSize = new Vector2(1920, 1080);

    dungeonEditor.titleContent = new GUIContent("3D Dungeon Generation");
  }

  private void OnEnable() {
    List<DungeonSettings> listSettingsPreset = LoadDungeonSettings();
    foreach(DungeonSettings settings in listSettingsPreset) {
      if(settings.activePreset) {
        dungeonSettings = settings;
        break;
      }
    }
    InitializeOriginalSettings();
  }

  private void InitializeOriginalSettings() {
    originalSettings = Instantiate(dungeonSettings);
  }

  private void OnGUI() {
    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
    GUILayout.Space(25);

    GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      if(GUILayout.Button(tabsName[0], GUILayout.Height(50), GUILayout.Width(position.width * 0.32f))) 
        currentSection = WindowSection.Settings;
      GUILayout.FlexibleSpace();
      if(GUILayout.Button(tabsName[1], GUILayout.Height(50), GUILayout.Width(position.width * 0.32f))) 
        currentSection = WindowSection.Types;
      GUILayout.FlexibleSpace();
      if(GUILayout.Button(tabsName[2], GUILayout.Height(50), GUILayout.Width(position.width * 0.32f))) 
        currentSection = WindowSection.Information;
      GUILayout.FlexibleSpace();

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
    EditorGUILayout.EndScrollView();
  }

  private void ShowSettings() {
    
    if(dungeonSettings) Undo.RecordObject(dungeonSettings, "Modify Dungeon Settings");

    GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      GUILayout.Label("Dungeon Settings", EditorStyles.boldLabel);
      GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();

    GUILayout.Space(10);
    
    if(dungeonSettings) {
      GUILayout.BeginHorizontal();
        GUIContent roomSizeFoldout = new() {
          text = "Room Settings"
        };
        showRoomSize = GUILayout.Toggle(showRoomSize, roomSizeFoldout, EditorStyles.foldoutHeader, GUILayout.ExpandWidth(true));
      GUILayout.EndHorizontal();
      
      if(showRoomSize) {
        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Space(10);
        EditorGUI.indentLevel++;

        dungeonSettings.roomSize = Mathf.Max(1, EditorGUILayout.IntField(new GUIContent("Room Size", "X and Z size of every single room."), dungeonSettings.roomSize));
        dungeonSettings.minRooms = Mathf.Max(1, EditorGUILayout.IntField(new GUIContent("Min Rooms", "Minimum amount of room that should be generated."), dungeonSettings.minRooms));
        dungeonSettings.maxRooms = Mathf.Max(dungeonSettings.minRooms + 1, EditorGUILayout.IntField(new GUIContent("Max Rooms", "Maximum amount of room that should be generated."), dungeonSettings.maxRooms));

        EditorGUI.BeginDisabledGroup(true);
          dungeonSettings.nbRooms = EditorGUILayout.IntField(new GUIContent("Room Count: ", "How much room have been generated so far."), dungeonSettings.nbRooms);
        EditorGUI.EndDisabledGroup();

        EditorGUI.indentLevel--;
        GUILayout.Space(10);
        GUILayout.EndVertical();
      }

      GUILayout.Space(10);

      GUILayout.BeginHorizontal();
        GUIContent dungeonConnexionFoldout = new() {
          text = "Generation Settings"
        };
        showDungeonConnexions = GUILayout.Toggle(showDungeonConnexions, dungeonConnexionFoldout, EditorStyles.foldoutHeader, GUILayout.ExpandWidth(true));
      GUILayout.EndHorizontal();

      if(showDungeonConnexions) {
        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Space(10);
        EditorGUI.indentLevel++;

        EditorGUI.BeginDisabledGroup(dungeonSettings.useRandomSeed);
          dungeonSettings.seed = EditorGUILayout.TextField(new GUIContent("Seed: ", "Specific seed to use when generating the dungeon. Useful if you want to generate the same dungeon with the same settings."), dungeonSettings.seed);
        EditorGUI.EndDisabledGroup();
        dungeonSettings.useRandomSeed = EditorGUILayout.Toggle(new GUIContent("Use Random Seed: ", "If checked, the generation will ignore the specific seed and generate a random one."), dungeonSettings.useRandomSeed);
        dungeonSettings.heightVariation = Mathf.Max(0, EditorGUILayout.IntField(new GUIContent("Height Variation: ", "How much units the next room will be generated on the Y axis."), dungeonSettings.heightVariation));
        dungeonSettings.heightVariationChance = EditorGUILayout.IntSlider(new GUIContent("Height Variation Chance: ", "Chances for the next room to generate higher based on 'heightVariation' variable."), dungeonSettings.heightVariationChance, 0, 100);
        dungeonSettings.dungeonSpread = EditorGUILayout.IntSlider(new GUIContent("Dungeon Spread: ", "How much the dungeon will spread on the X and Z axis. (0 = no gap between rooms | 3 = looks like tree branches (harder to generate))"), dungeonSettings.dungeonSpread, 0, 3);

        EditorGUI.indentLevel--;
        GUILayout.Space(10);
        GUILayout.EndVertical();
      }
    }
    else {
      GUILayout.BeginVertical(EditorStyles.helpBox);
      GUILayout.Space(10);
      GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
          GUILayout.Label("No Dungeon Settings has been found!", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
          GUILayout.Label("Try creating a new settings preset or select an existing one.");
        GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();
      GUILayout.Space(10);
      GUILayout.EndVertical();
    }
    

    SeparatorLine(Color.white);

    GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      if(GUILayout.Button("New Settings Preset", GUILayout.Height(25), GUILayout.Width(150))) {
        ShowPopupPresetSettings();
      }
      GUILayout.FlexibleSpace();
      if(dungeonSettings){
        if(GUILayout.Button(dungeonSettings.CheckForUnsavedChanges(originalSettings) ? "Save Settings*" : "Save Settings", GUILayout.Width(150), GUILayout.Height(25))) {
          SaveDungeonSettings();
        }
      }
      else {
        GUILayout.Space(150);
      }
      GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();

    List<DungeonSettings> listSettingsPreset = LoadDungeonSettings();

    GUILayout.Space(25);
    Handles.color = Color.gray;
    Handles.DrawLine(new Vector3(0, GUILayoutUtility.GetLastRect().yMax), new Vector3(Screen.width, GUILayoutUtility.GetLastRect().yMax));

    settingsPresetScroll = EditorGUILayout.BeginScrollView(settingsPresetScroll, GUILayout.Height(210));

    if(listSettingsPreset.Count == 0) {
      GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("No settings found", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();
    }

    foreach(DungeonSettings settings in listSettingsPreset) {
      GUILayout.Space(25);
      GUILayout.BeginHorizontal();

      GUIStyle greenLabelStyle = new(EditorStyles.boldLabel);
      greenLabelStyle.normal.textColor = new Color(90/255f, 170/255f, 100/255f);

      GUILayout.Space(50);
      GUILayout.Label(settings.presetName, settings.activePreset ? greenLabelStyle : EditorStyles.boldLabel, GUILayout.Width(150));

      GUILayout.FlexibleSpace();

      if(GUILayout.Button("Select Preset", EditorStyles.miniButton, GUILayout.Width(100))) {
        SetActivePreset(settings, listSettingsPreset);
      }

      GUILayout.FlexibleSpace();

      GUILayout.Space(100);
      if(GUILayout.Button("Delete", EditorStyles.boldLabel, GUILayout.Width(50))) DeleteSettingsPopup(settings);
      GUILayout.Space(50);

      GUILayout.EndHorizontal();

      GUILayout.Space(25);
      Handles.color = new Color(0.3f, 0.3f, 0.3f);
      Handles.DrawLine(new Vector3(0, GUILayoutUtility.GetLastRect().yMax), new Vector3(Screen.width, GUILayoutUtility.GetLastRect().yMax));
    }

    EditorGUILayout.EndScrollView();
    Handles.color = Color.gray;
    Handles.DrawLine(new Vector3(0, GUILayoutUtility.GetLastRect().yMax), new Vector3(Screen.width, GUILayoutUtility.GetLastRect().yMax));
  }

  void SetActivePreset(DungeonSettings selectedPreset, List<DungeonSettings> listSettingsPreset) {
    foreach(DungeonSettings settings in listSettingsPreset) {
      settings.activePreset = settings == selectedPreset;
      dungeonSettings = selectedPreset;
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
        GUIStyle customStyle = new(EditorStyles.foldoutHeader) {
          alignment = TextAnchor.MiddleCenter,
        };
        customStyle.padding.left = 0;
        GUIContent foldoutContent = new() {
          text = roomType.displayName
        };
        roomType.isExpanded = GUILayout.Toggle(roomType.isExpanded, foldoutContent, customStyle, GUILayout.ExpandWidth(true));
      GUILayout.EndHorizontal();

      GUILayout.Space(20);

      GUILayout.BeginVertical();
        if(roomType.isExpanded) DisplayExpandedElement(roomType);
        else DisplayClosedElement(roomType);
      GUILayout.EndVertical();

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

      EditorGUI.BeginDisabledGroup(roomType.optionalRoom || roomType.spawnRoom || roomType.storyRoom || roomType.bossRoom);
        GUILayout.Label("Required Room ", EditorStyles.boldLabel);
        roomType.requiredRoom = GUILayout.Toggle(roomType.requiredRoom, "");
      EditorGUI.EndDisabledGroup();

      GUILayout.FlexibleSpace();

      EditorGUI.BeginDisabledGroup(roomType.requiredRoom || roomType.hiddenRoom || roomType.treasureRoom || roomType.healPoolRoom || roomType.craftingRoom || roomType.upgradeRoom);
        GUILayout.Label("Optional Room ", EditorStyles.boldLabel);
        roomType.optionalRoom = GUILayout.Toggle(roomType.optionalRoom, "");
      EditorGUI.EndDisabledGroup();

      GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();

    if(roomType.requiredRoom) ShowRequiredSettings(roomType);
    if(roomType.optionalRoom) ShowOptionalSettings(roomType);
  }

  private void ShowRequiredSettings(RoomType roomType) {
    GUILayout.Space(20);
    GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();

      EditorGUI.BeginDisabledGroup(roomType.storyRoom || roomType.bossRoom);
        GUILayout.Label("Spawn Room ", EditorStyles.boldLabel);
        roomType.spawnRoom = GUILayout.Toggle(roomType.spawnRoom, "");
      EditorGUI.EndDisabledGroup();

      GUILayout.FlexibleSpace();

      EditorGUI.BeginDisabledGroup(roomType.spawnRoom || roomType.bossRoom);
        GUILayout.Label("Story Room ", EditorStyles.boldLabel);
        roomType.storyRoom = GUILayout.Toggle(roomType.storyRoom, "");
      EditorGUI.EndDisabledGroup();

      GUILayout.FlexibleSpace();

      EditorGUI.BeginDisabledGroup(roomType.spawnRoom || roomType.storyRoom);
        GUILayout.Label("Boss Room ", EditorStyles.boldLabel);
        roomType.bossRoom = GUILayout.Toggle(roomType.bossRoom, "");
      EditorGUI.EndDisabledGroup();

      GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();
  }

  private void ShowOptionalSettings(RoomType roomType) {
    GUILayout.Space(20);
    GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();

      EditorGUI.BeginDisabledGroup(roomType.treasureRoom || roomType.healPoolRoom || roomType.craftingRoom || roomType.upgradeRoom);
        GUILayout.Label("Hidden Room ", EditorStyles.boldLabel);
        roomType.hiddenRoom = GUILayout.Toggle(roomType.hiddenRoom, "");
      EditorGUI.EndDisabledGroup();

      GUILayout.FlexibleSpace();

      EditorGUI.BeginDisabledGroup(roomType.hiddenRoom || roomType.healPoolRoom || roomType.craftingRoom || roomType.upgradeRoom);
        GUILayout.Label("Treasure Room ", EditorStyles.boldLabel);
        roomType.treasureRoom = GUILayout.Toggle(roomType.treasureRoom, "");
      EditorGUI.EndDisabledGroup();

      GUILayout.FlexibleSpace();

      EditorGUI.BeginDisabledGroup(roomType.treasureRoom || roomType.hiddenRoom || roomType.craftingRoom || roomType.upgradeRoom);
        GUILayout.Label("Heal Room ", EditorStyles.boldLabel);
        roomType.healPoolRoom = GUILayout.Toggle(roomType.healPoolRoom, "");
      EditorGUI.EndDisabledGroup();

      GUILayout.FlexibleSpace();

      EditorGUI.BeginDisabledGroup(roomType.treasureRoom || roomType.healPoolRoom || roomType.hiddenRoom || roomType.upgradeRoom);
        GUILayout.Label("Crafting Room ", EditorStyles.boldLabel);
        roomType.craftingRoom = GUILayout.Toggle(roomType.craftingRoom, "");
      EditorGUI.EndDisabledGroup();

      GUILayout.FlexibleSpace();

      EditorGUI.BeginDisabledGroup(roomType.treasureRoom || roomType.healPoolRoom || roomType.craftingRoom || roomType.hiddenRoom);
        GUILayout.Label("Upgrade Room ", EditorStyles.boldLabel);
        roomType.upgradeRoom = GUILayout.Toggle(roomType.upgradeRoom, "");
      EditorGUI.EndDisabledGroup();

      GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();
  }

  void DisplayClosedElement(RoomType roomType) {
    GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      if(GUILayout.Button("Delete", EditorStyles.boldLabel)) DeleteRoomTypePopup(roomType);
      GUILayout.FlexibleSpace();
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
  private void ShowPopupPresetSettings() {
    PopupWindow.Show(new Rect(Screen.width/2 - 100, 180, 200, 0), new CreatePresetSettingsPopup(this));
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

    string existingAssetPath = $"Assets/Resources/RoomType/{assetName}.asset";
    RoomType existingroomType = AssetDatabase.LoadAssetAtPath<RoomType>(existingAssetPath);
    
    if(existingroomType != null) {
      Debug.LogWarning($"A preset with the name '{assetName}' already exists in your assets. Please choose a different name. In Assets/Resources/RoomType/{assetName}.asset");
      return;
    }

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

  private void DeleteRoomTypePopup(RoomType roomType) {
    PopupWindow.Show(new Rect(Screen.width/2 - 100, 180, 200, 0), new DeletePopup<RoomType>(roomType));
  }
  private void DeleteSettingsPopup(DungeonSettings settings) {
    PopupWindow.Show(new Rect(Screen.width/2 - 100, 180, 200, -400), new DeletePopup<DungeonSettings>(settings));
  }

  private class DeletePopup<T> : PopupWindowContent where T : UnityEngine.Object {
    private T targetElement;

    public DeletePopup(T targetElement) {
      this.targetElement = targetElement;
    }

    public override void OnGUI(Rect rect) {
      GUIStyle redButton = new(GUI.skin.button);
      redButton.normal.textColor = new(1, 0.5f, 0.5f);

      GUILayout.FlexibleSpace();
      GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      Event e = Event.current;
      if(GUILayout.Button("CONFIRM DELETE ?", redButton, GUILayout.Width(150), GUILayout.Height(150))
      || (e.isKey && e.keyCode == KeyCode.Return)) {
        string assetPath = AssetDatabase.GetAssetPath(targetElement);
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

  private class CreatePresetSettingsPopup : PopupWindowContent {
    private DungeonEditor dungeonEditorPopup;
    private string assetName = "NewSettingsPreset";

    public CreatePresetSettingsPopup(DungeonEditor dungeonEditor) {
      dungeonEditorPopup = dungeonEditor;
    }

    public override void OnGUI(Rect rect) {
      GUILayout.Label("Enter Preset Name:");
      assetName = EditorGUILayout.TextField(assetName);

      Event e = Event.current;
      if(GUILayout.Button("Create")
      || (e.isKey && e.keyCode == KeyCode.Return)) {
        dungeonEditorPopup.CreateSettingsPreset(assetName);
        dungeonEditorPopup.Repaint();

        editorWindow.Close();
      }
    }
  }

  private void CreateSettingsPreset(string assetName) {
    assetName = assetName.Replace(" ", "");

    string existingAssetPath = $"Assets/Resources/DungeonSettings/{assetName}.asset";
    DungeonSettings existingSettings = AssetDatabase.LoadAssetAtPath<DungeonSettings>(existingAssetPath);
    
    if(existingSettings != null) {
      Debug.LogWarning($"A preset with the name '{assetName}' already exists in your assets. Please choose a different name. In Assets/Resources/DungeonSettings/{assetName}.asset");
      return;
    }

    DungeonSettings dungeonSettings = ScriptableObject.CreateInstance<DungeonSettings>();

    string scriptableObjectfolderPath = "Assets/Resources";
    if (!AssetDatabase.IsValidFolder(scriptableObjectfolderPath)) {
      AssetDatabase.CreateFolder("Assets", "Resources");
    }

    string dungeonSettingsfolderPath = "Assets/Resources/DungeonSettings";
    if (!AssetDatabase.IsValidFolder(dungeonSettingsfolderPath)) {
      AssetDatabase.CreateFolder("Assets/Resources", "DungeonSettings");
    }

    string assetPath = $"Assets/Resources/DungeonSettings/{assetName}.asset";
    AssetDatabase.CreateAsset(dungeonSettings, assetPath);
    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh();

    SetActivePreset(dungeonSettings, LoadDungeonSettings());
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

  private List<DungeonSettings> LoadDungeonSettings() {
    List<DungeonSettings> list = new();
    string folderPath = "DungeonSettings";
    DungeonSettings [] r_dungeonSettings = Resources.LoadAll<DungeonSettings>(folderPath);
    foreach(DungeonSettings settings in r_dungeonSettings) {
      string assetPath = AssetDatabase.GetAssetPath(settings);
      if(settings.presetName == "" || settings.presetName == null) settings.presetName = assetPath.Substring(33).Remove(assetPath[33..].Length - 6);
      list.Add(settings);
    }
    return list;
  }

  private void SaveDungeonSettings() {
    if(dungeonSettings != null) {
      EditorUtility.SetDirty(dungeonSettings);
      AssetDatabase.SaveAssets();
      InitializeOriginalSettings();
    }
  }

  private void OnDestroy() {
    if(dungeonSettings.CheckForUnsavedChanges(originalSettings)) {
      bool userWantsToSave = EditorUtility.DisplayDialog("Unsaved Changes", "There are unsaved changes. Do you want to save them before closing?", "Save", "Don't Save");
      if(userWantsToSave) {
        SaveDungeonSettings();
      }
      else dungeonSettings.CopyValuesFrom(originalSettings);
    }
  }
}