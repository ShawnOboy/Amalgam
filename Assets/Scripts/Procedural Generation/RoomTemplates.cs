using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomTemplates : MonoBehaviour
{

  [Header ("Values")]
  [Min(1)]
  public int minRooms;
  [Min(1)]
  public int maxRooms;
  public int nbRooms;
  [Min(1)]
  public int doorSpawnHeight;
  [Range(0, 3)]
  public int spread;
  [Min(1)]
  public int roomX;
  [Min(1)]
  public int roomZ;
  [Range(0, 100)]
  public int heightVariantPercentage;
  [HideInInspector] public bool canReplace;
  
  [Header ("Minimap")]
  public GameObject roomTile;
  public GameObject doorTile;

  [Header ("Rooms Prefabs")]
  
  [Header ("End Rooms")]
  public GameObject [] endRoom;
  public GameObject [] endRoomBoss;

  [Header ("Tunnel Rooms")]
  public GameObject [] tunnelMiddle;
  public GameObject [] tunnelTop;

  [Header ("L Rooms")]
  public GameObject [] LMiddle;
  public GameObject [] LRightTop;
  public GameObject [] LLeftTop;

  [Header ("T Rooms")]

  // All Rotation T Shape
  public GameObject [] TMiddle;

  // Left and Right Mid
  public GameObject [] TLeftRightMid;

  // Always Middle Front Door
  public GameObject [] TLeftTop; // 90 deg for Front Top
  public GameObject [] TRightTop; // 270 deg for Front Top

  // Left or Right Always Middle
  public GameObject [] TLeftAndRightTop;
  public GameObject [] TLeftAndFrontTop;
  public GameObject [] TRightAndFrontTop;

  [Header ("All Side Rooms")]

  // All sided room mid height
  public GameObject [] AllSide; // no turn

  // All sided room 1 Top door
  public GameObject [] AllSideOneTop; // turn all direction

  // All sided room 2 Top door
  public GameObject [] AllSideLTop; // turn all direction
  public GameObject [] AllSideTunnelTop; // 90 deg for left and right Top

  // All sided room 3 Top door
  public GameObject [] AllSideTTop; // turn all direction

  [Header ("Templates Prefabs")]
  public GameObject templateRoom;
  public GameObject templateGround;
  public GameObject templateDoor;

  [Header ("Parents for Instantiated Objects")]
  public GameObject doorParent;
  public GameObject floorParent;



// Debugging -------------------------------------------------------------------
  public void SpawnRooms() { // Spawn all possible rooms step by step
    GameObject roomGenerator = GameObject.FindGameObjectWithTag("RoomGenerator");
    roomGenerator.GetComponent<RoomGenerator>().Invoke("SpawnRoomsCoroutine", 0f);
  }

  public void ReloadScene() { // Reload the scene if not enough or too much rooms
    Scene currentScene = SceneManager.GetActiveScene();
    SceneManager.LoadScene(currentScene.buildIndex);
  }

}
