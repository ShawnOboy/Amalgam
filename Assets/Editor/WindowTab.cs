using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEditor.UI;
using UnityEditor.ShortcutManagement;

public class WindowTab : EditorWindow {

  public int tabs = 3;
  bool firstLoad = false;
  readonly string[] tabOptions = new string[] {"Infos Structure", "Prefabs Rooms", "How To Generate"};

  private enum WindowSection { InfosStructure, PrefabsRoom, HowToGenerate }
  private WindowSection currentSection = WindowSection.InfosStructure;
  private RoomTemplates roomTemplates;

  [MenuItem("Dungeon/3D Dungeon Generation")]
  private static void ShowWindow() {
    WindowTab window = (WindowTab)GetWindow(typeof(WindowTab));
    window.minSize = new Vector2(400, 300);
    window.maxSize = new Vector2(1200, 720);

    window.titleContent = new GUIContent("3D Dungeon Generation");
  }

  private void OnGUI() {
    GUILayout.BeginHorizontal();

    GUILayout.BeginVertical(GUILayout.Width(100));

    if(GUILayout.Button("Infos Structure", GUILayout.Height(50))) {
      currentSection = WindowSection.InfosStructure;
    }
    if(GUILayout.Button("Prefabs Room", GUILayout.Height(50))) {
      currentSection = WindowSection.PrefabsRoom;
    }
    if(GUILayout.Button("How To Generate", GUILayout.Height(50))) {
      currentSection = WindowSection.HowToGenerate;
    }

    GUILayout.EndVertical();
    GUILayout.BeginVertical();

    switch (currentSection) {
      case WindowSection.InfosStructure:
        InfosStructure();
        break;
      case WindowSection.PrefabsRoom:
        PefabsRoom();
        break;
      case WindowSection.HowToGenerate:
        HowToGenerate();
        break;
    }

    GUILayout.EndVertical();
    GUILayout.EndHorizontal();
  }

  private void InfosStructure() {
    if (roomTemplates == null) {
      roomTemplates = FindObjectOfType<RoomTemplates>(); // Find the ReplaceTemplates script in the scene
      if (roomTemplates == null) {
        GUILayout.Label("ReplaceTemplates script not found in the scene.");
        return;
      }
    }

    GUILayout.Label("This is all the information needed to be able to generate the dungeon for your project");

    roomTemplates.minRooms = EditorGUILayout.IntField("Minimum Amount of Rooms", roomTemplates.minRooms);
    roomTemplates.maxRooms = EditorGUILayout.IntField("Maximum Amount of Rooms", roomTemplates.maxRooms);
    roomTemplates.doorSpawnHeight = EditorGUILayout.IntField("Height Variation for Spawning Rooms", roomTemplates.doorSpawnHeight);
    roomTemplates.spread = EditorGUILayout.IntSlider("Amount of spreading for the Dungeon (like a branch in a tree)", roomTemplates.spread, 0, 3);
    roomTemplates.roomX = EditorGUILayout.IntField("X Size for your Rooms Prefabs (All Must be the Same)", roomTemplates.roomX);
    roomTemplates.roomZ = EditorGUILayout.IntField("Z Size for your Rooms Prefabs (All Must be the Same)", roomTemplates.roomZ);
    roomTemplates.heightVariantPercentage = EditorGUILayout.IntSlider("Chances of Spawning a Room at a New Height", roomTemplates.heightVariantPercentage, 0, 100);

    GUILayout.FlexibleSpace();

    if (GUILayout.Button("Next Tab")) {
      tabs++;
    }
  }
  private void PefabsRoom() {
    GUILayout.Label("This is all the prefabs needed to be able to generate the dungeon's room with a specific template");
    GUILayout.FlexibleSpace();
    if(GUILayout.Button("Next Tab")) {
      tabs++;
    }
  }
  private void HowToGenerate() {
    GUILayout.Label("This is all you need to know be able to generate the dungeon");
    GUILayout.FlexibleSpace();
    if(GUILayout.Button("Close")) {
      WindowTab.focusedWindow.Close();
    }
  }
}
