using UnityEngine;

// [CreateAssetMenu(fileName = "RoomType", menuName = "Amalgam/RoomType", order = 0)]
[System.Serializable]
public class RoomType : ScriptableObject {

  // For Editor
  [HideInInspector] public string displayName;
  [HideInInspector] public bool isExpanded = false;

  // For Game

  [HideInInspector] public bool requiredRoom;

    [HideInInspector] public bool spawnRoom;
    [HideInInspector] public bool bossRoom;
      [HideInInspector] public string bossName;
      [Range (1, 5)] [HideInInspector] public int bossIndex;
    [HideInInspector] public bool storyRoom;
      [Range (1, 5)] [HideInInspector] public int storyIndex;
      [HideInInspector] public bool storyComplete;

  [HideInInspector] public bool optionalRoom;

    [HideInInspector] public bool hiddenRoom;
    [HideInInspector] public bool treasureRoom;
    [HideInInspector] public bool healPoolRoom;
    [HideInInspector] public bool craftingRoom;
    [HideInInspector] public bool upgradeRoom;



}
