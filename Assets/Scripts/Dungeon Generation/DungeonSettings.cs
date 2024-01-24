using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonSettings {

  [Header ("Room Size Settings")]
  public int roomSize;
  public int minRooms;
  public int maxRooms;
  public int nbRooms;

  [Header ("Dungeon Connexions Settings")]
  public int heightVariation;
  public int heightVariationChance;
  public int dungeonSpread;
}
