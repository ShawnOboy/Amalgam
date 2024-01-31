using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DungeonGenerator : MonoBehaviour {

  private DungeonSettings _DS;

  bool isGenerating = false;
  GameObject dungeon;

  int i = 0;

  private void Awake() {
    _DS = GetDungeonSettings();
  }

  void Update() {
    if(_DS == null) Debug.LogError("Missing Dungeon Settings");
    if(Input.GetKeyDown(KeyCode.S)) {
      StartCoroutine(Generator());
    }
  }

  IEnumerator Generator() {
    dungeon = new() {
      name = "Dungeon"
    };

    isGenerating = true;
    while(isGenerating) {
      NextRoomGen();
      yield return new WaitForEndOfFrame();
    }
    Debug.Log("Generation Completed");
  }

  void NextRoomGen() {
    // GameObject room = new() {
    //   name = "Room " + i,
    // };
    // room.transform.parent = dungeon.transform;
  }

  private DungeonSettings GetDungeonSettings() {
    string folderPath = "DungeonSettings";
    DungeonSettings [] r_dungeonSettings = Resources.LoadAll<DungeonSettings>(folderPath);
    foreach(DungeonSettings settings in r_dungeonSettings) {
      if(settings.activePreset) return settings;
    }
    return null;
  }
}
