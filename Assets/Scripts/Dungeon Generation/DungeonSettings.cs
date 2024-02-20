using UnityEngine;

[System.Serializable]
public class DungeonSettings : ScriptableObject {

  // For Editor
  [HideInInspector] public string presetName;
  public bool activePreset = false;

  // For Game

  [Header ("Room Size Settings")]
  [HideInInspector] public int roomSize;
  [HideInInspector] public int minRooms;
  [HideInInspector] public int maxRooms;
  [HideInInspector] public int nbRooms;

  [Header ("Dungeon Connexions Settings")]
  [HideInInspector] public string seed;
  [HideInInspector] public bool useRandomSeed;
  [HideInInspector] public int heightVariation;
  [HideInInspector] public int heightVariationChance;
  [HideInInspector] public int dungeonSpread;

  public bool CheckForUnsavedChanges(DungeonSettings other) {
    return !(roomSize == other.roomSize &&
           minRooms == other.minRooms &&
           maxRooms == other.maxRooms &&
           nbRooms == other.nbRooms &&
           seed == other.seed &&
           useRandomSeed == other.useRandomSeed &&
           heightVariation == other.heightVariation &&
           heightVariationChance == other.heightVariationChance &&
           dungeonSpread == other.dungeonSpread);
  }
  public void CopyValuesFrom(DungeonSettings other) {
    roomSize = other.roomSize;
    minRooms = other.minRooms;
    maxRooms = other.maxRooms;
    nbRooms = other.nbRooms;
    seed = other.seed;
    useRandomSeed = other.useRandomSeed;
    heightVariation = other.heightVariation;
    heightVariationChance = other.heightVariationChance;
    dungeonSpread = other.dungeonSpread;
  }
}
