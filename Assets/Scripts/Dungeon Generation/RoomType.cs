using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(fileName = "RoomType", menuName = "Amalgam/RoomType", order = 0)]
public class RoomType : ScriptableObject {

  // For Editor
  public string displayName;
  public bool isExpanded = false;

  // For Game

  public bool requiredRoom;

    public bool spawnRoom;
    public bool bossRoom;
      public string bossName;
      [Range (1, 5)] public int bossIndex;
    public bool storyRoom;
      [Range (1, 5)] public int storyIndex;
      public bool storyComplete;

  public bool optionalRoom;

    public bool hiddenRoom;
    public bool treasureRoom;
    public bool healPoolRoom;
    public bool craftingRoom;
    public bool upgradeRoom;



}
