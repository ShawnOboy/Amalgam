using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DungeonGenerator : MonoBehaviour {

  private DungeonSettings _DS;
  private Dictionary<Vector2, int> dungeonGrid = new();
  private List<DungeonRoom> dungeonRooms = new();

  bool isGenerating = false;
  GameObject dungeon;

  int i = 0;

  private void Awake() {
    _DS = GetDungeonSettings();
    StartCoroutine(Generator());
    DungeonRoom newRoom = new();
    newRoom.InitializeRoom(_DS.roomSize);
    dungeonRooms.Add(newRoom);
    dungeonGrid.Add(new Vector2(0, 0), 0);
    dungeonGrid.Add(new Vector2(0, 1), 0);
  }

  void Update() {
    if(_DS == null) Debug.LogError("Missing Dungeon Settings");
    if(Input.GetKeyDown(KeyCode.S)) {
    }
  }

  IEnumerator Generator() {
    if(_DS.useRandomSeed) {

    }
    else {
      
    }

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
