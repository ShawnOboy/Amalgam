using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0051")]

public class ReplaceTemplates : MonoBehaviour
{
  public List<RoomBehavior> roomBehaviors;
  private RoomTemplates templates;
  private DungeonSettings _DS;
  private int roomRotation = 0;
  private int roomNumber = 0;
  private int doorNumber = 0;
  public List<GameObject> allNewDoors;
  public List<GameObject> allNewRooms;
  private RoomGenerator roomGenerator;
  public GameObject minimapCam;

  [System.Obsolete]
  private void Start() {
    roomGenerator = GameObject.FindGameObjectWithTag("RoomGenerator").GetComponent<RoomGenerator>();
    templates = GameObject.FindGameObjectWithTag("RoomTemplate").GetComponent<RoomTemplates>();
    _DS = templates.GetDungeonSettings();
    StartCoroutine(WaitCheckDisable());
  }

  [System.Obsolete]
  private IEnumerator WaitCheckDisable() {
    while(true) {
      bool canReplace = true;

      foreach(GameObject room in GameObject.FindGameObjectsWithTag("Room")) {
        RoomBehavior roomBehavior = room.GetComponent<RoomBehavior>();

        if(!roomBehavior.hasBeenCounted && roomGenerator.isEnabled) {
          canReplace = false;
          break;
        }
        else {
          StopCoroutine(WaitCheckDisable());
        }
      }

      if(canReplace) {
        ReplaceTemplate();
      }

      yield return null;
    }
  }

  [System.Obsolete]
  private void ReplaceTemplate() {

    if(_DS.nbRooms < _DS.minRooms) {
      templates.ReloadScene();
    }

    GameObject [] allDoors = GameObject.FindGameObjectsWithTag("Door");
    GameObject [] allRooms = GameObject.FindGameObjectsWithTag("Room");

    for(int i = 0; i < allRooms.Length; i++) {
      allRooms[i].transform.DetachChildren();

      if(i == allRooms.Length - 1) {
        allRooms[i].GetComponent<RoomBehavior>().isBossRoom = true;
      }
    }

    // Spawn a door with the template and check if it is not floating
    // Spawn a door tile at the location

    foreach(GameObject door in allDoors) {
      GameObject newDoor = Instantiate(templates.templateDoor, door.transform.position, door.transform.rotation);
      GameObject minimapTile = Instantiate(templates.doorTile, new Vector3(door.transform.position.x, door.transform.position.y + 0.01f, door.transform.position.z), Quaternion.Euler(0, 0, 0));
      newDoor.AddComponent<CheckFloatingDoor>();
      doorNumber++;
      newDoor.transform.parent = templates.doorParent.transform;
      newDoor.name = "NewDoor" + doorNumber;
      newDoor.tag = "Door";
      minimapTile.name = "Door Tile";
      minimapTile.layer = 6;
      Destroy(door);
      if(newDoor.GetComponent<CheckFloatingDoor>().isFloating) {
        Debug.Log("Door Destroyed");
        Destroy(newDoor);
        doorNumber--;
      }
    }

    allNewRooms = new List<GameObject>();

    // Spawn a room with the right template of door set
    // Spawn a room tile at the location

    foreach(GameObject room in allRooms) {
      GameObject roomPrefab = GenerateSpecificRoom(room);
      GameObject newRoomFloor = Instantiate(roomPrefab, room.transform.position, Quaternion.Euler(0, roomRotation, 0));
      GameObject minimapTile = Instantiate(templates.roomTile, room.transform.position, Quaternion.Euler(0, 0, 0));
      newRoomFloor.transform.parent = templates.floorParent.transform;
      roomNumber++;
      newRoomFloor.name += roomNumber;
      newRoomFloor.tag = "Room";

      allNewRooms.Add(newRoomFloor);

      minimapTile.name = "Room Tile";
      minimapTile.layer = 6;
      Color heightColor = new((room.transform.position.y / 255) * 2, (room.transform.position.y / 255) * 2, (room.transform.position.y / 255) * 2);
      minimapTile.GetComponent<Renderer>().material.color = heightColor;

      Destroy(room);
    }


    // Get informations for the minimap
    // X scale, Z scale and center point

    Collider[] colliders = templates.floorParent.GetComponentsInChildren<Collider>(); // Get colliders of all children
    Bounds totalBounds = new(transform.position, Vector3.zero); // Create an initial bounds

    foreach(Collider collider in colliders) { // Expand the bounds to include all colliders
        totalBounds.Encapsulate(collider.bounds);
    }

    // Calculate the size in X and Z dimensions
    minimapCam.GetComponent<MinimapCamera>().mapX = totalBounds.size.x; // bounds size X
    minimapCam.GetComponent<MinimapCamera>().mapZ = totalBounds.size.z; // bounds size Z
    minimapCam.GetComponent<MinimapCamera>().center = totalBounds.center; // bounds center point

  }

  // ██████╗  ██████╗  ██████╗ ███╗   ███╗    ████████╗███████╗███╗   ███╗██████╗ ██╗      █████╗ ████████╗███████╗
  // ██╔══██╗██╔═══██╗██╔═══██╗████╗ ████║    ╚══██╔══╝██╔════╝████╗ ████║██╔══██╗██║     ██╔══██╗╚══██╔══╝██╔════╝
  // ██████╔╝██║   ██║██║   ██║██╔████╔██║       ██║   █████╗  ██╔████╔██║██████╔╝██║     ███████║   ██║   █████╗  
  // ██╔══██╗██║   ██║██║   ██║██║╚██╔╝██║       ██║   ██╔══╝  ██║╚██╔╝██║██╔═══╝ ██║     ██╔══██║   ██║   ██╔══╝  
  // ██║  ██║╚██████╔╝╚██████╔╝██║ ╚═╝ ██║       ██║   ███████╗██║ ╚═╝ ██║██║     ███████╗██║  ██║   ██║   ███████╗
  // ╚═╝  ╚═╝ ╚═════╝  ╚═════╝ ╚═╝     ╚═╝       ╚═╝   ╚══════╝╚═╝     ╚═╝╚═╝     ╚══════╝╚═╝  ╚═╝   ╚═╝   ╚══════╝

  [System.Obsolete]
  private GameObject GenerateSpecificRoom(GameObject room) {

    room.AddComponent<CheckRoomsDoorsCount>();

    CheckRoomsDoorsCount doorLocation = room.GetComponent<CheckRoomsDoorsCount>();

    doorLocation.ShootRaycasts();

    // Check For End Room ------------------------------------------------------

    if(doorLocation.hasDoorNorth
    && !doorLocation.hasDoorEast
    && !doorLocation.hasDoorSouth
    && !doorLocation.hasDoorWest) {

      roomRotation = 0;
      if(room.GetComponent<RoomBehavior>().isBossRoom) {
        return templates.endRoomBoss[(int)Mathf.Floor(Random.RandomRange(0, templates.endRoomBoss.Length))];
      }
      else {
        return templates.endRoom[(int)Mathf.Floor(Random.RandomRange(0, templates.endRoom.Length))];
      }


    }
    else if(!doorLocation.hasDoorNorth
    && doorLocation.hasDoorEast
    && !doorLocation.hasDoorSouth
    && !doorLocation.hasDoorWest) {

      roomRotation = 90;
      if(room.GetComponent<RoomBehavior>().isBossRoom) {
        return templates.endRoomBoss[(int)Mathf.Floor(Random.RandomRange(0, templates.endRoomBoss.Length))];
      }
      else {
        return templates.endRoom[(int)Mathf.Floor(Random.RandomRange(0, templates.endRoom.Length))];
      }

    }
    else if(!doorLocation.hasDoorNorth
    && !doorLocation.hasDoorEast
    && doorLocation.hasDoorSouth
    && !doorLocation.hasDoorWest) {

      roomRotation = 180;
      if(room.GetComponent<RoomBehavior>().isBossRoom) {
        return templates.endRoomBoss[(int)Mathf.Floor(Random.RandomRange(0, templates.endRoomBoss.Length))];
      }
      else {
        
        
        return templates.endRoom[(int)Mathf.Floor(Random.RandomRange(0, templates.endRoom.Length))];
      }

    }
    else if(!doorLocation.hasDoorNorth
    && !doorLocation.hasDoorEast
    && !doorLocation.hasDoorSouth
    && doorLocation.hasDoorWest) {

      roomRotation = 270;
      if(room.GetComponent<RoomBehavior>().isBossRoom) {
        return templates.endRoomBoss[(int)Mathf.Floor(Random.RandomRange(0, templates.endRoomBoss.Length))];
      }
      else {
        return templates.endRoom[(int)Mathf.Floor(Random.RandomRange(0, templates.endRoom.Length))];
      }

    }

    // Check For Tuunel Room ---------------------------------------------------

    if(doorLocation.hasDoorNorth
    && !doorLocation.hasDoorEast
    && doorLocation.hasDoorSouth
    && !doorLocation.hasDoorWest) {

      if(doorLocation.topDoorNorth) {

        roomRotation = 180;
        return templates.tunnelTop[(int)Mathf.Floor(Random.RandomRange(0, templates.tunnelTop.Length))];

      }

      else if(doorLocation.topDoorSouth) {

        roomRotation = 0;
        return templates.tunnelTop[(int)Mathf.Floor(Random.RandomRange(0, templates.tunnelTop.Length))];

      }

      else {
        roomRotation = 0;
        return templates.tunnelMiddle[(int)Mathf.Floor(Random.RandomRange(0, templates.tunnelMiddle.Length))];
      }
    }

    else if(!doorLocation.hasDoorNorth // Should Rotate Room 90deg
    && doorLocation.hasDoorEast
    && !doorLocation.hasDoorSouth
    && doorLocation.hasDoorWest) {

      if(doorLocation.topDoorEast) {

        roomRotation = 270;
        return templates.tunnelTop[(int)Mathf.Floor(Random.RandomRange(0, templates.tunnelTop.Length))];

      }

      else if(doorLocation.topDoorWest) {

        roomRotation = 90;
        return templates.tunnelTop[(int)Mathf.Floor(Random.RandomRange(0, templates.tunnelTop.Length))];

      }

      else {
        roomRotation = 90;
        
        return templates.tunnelMiddle[(int)Mathf.Floor(Random.RandomRange(0, templates.tunnelMiddle.Length))];
      }

    }

    // Check For L Room --------------------------------------------------------

    if(doorLocation.hasDoorNorth
    && doorLocation.hasDoorEast
    && !doorLocation.hasDoorSouth
    && !doorLocation.hasDoorWest) {

      if(doorLocation.topDoorNorth) {

        roomRotation = 90;
        return templates.LRightTop[(int)Mathf.Floor(Random.RandomRange(0, templates.LRightTop.Length))];

      }

      else if(doorLocation.topDoorEast) {

        roomRotation = 90;
        return templates.LLeftTop[(int)Mathf.Floor(Random.RandomRange(0, templates.LLeftTop.Length))];

      }

      else {
        roomRotation = 90;
        
        return templates.LMiddle[(int)Mathf.Floor(Random.RandomRange(0, templates.LMiddle.Length))];
      }

    }
    else if(!doorLocation.hasDoorNorth // Should Rotate Room 90deg
    && doorLocation.hasDoorEast
    && doorLocation.hasDoorSouth
    && !doorLocation.hasDoorWest) {

      if(doorLocation.topDoorEast) {

        roomRotation = 180;
        return templates.LRightTop[(int)Mathf.Floor(Random.RandomRange(0, templates.LRightTop.Length))];

      }

      else if(doorLocation.topDoorSouth) {

        roomRotation = 180;
        return templates.LLeftTop[(int)Mathf.Floor(Random.RandomRange(0, templates.LLeftTop.Length))];

      }

      else {
        roomRotation = 180;
        
        return templates.LMiddle[(int)Mathf.Floor(Random.RandomRange(0, templates.LMiddle.Length))];
      }

    }
    else if(!doorLocation.hasDoorNorth // Should Rotate Room 180deg
    && !doorLocation.hasDoorEast
    && doorLocation.hasDoorSouth
    && doorLocation.hasDoorWest) {

      if(doorLocation.topDoorSouth) {

        roomRotation = 270;
        return templates.LRightTop[(int)Mathf.Floor(Random.RandomRange(0, templates.LRightTop.Length))];

      }

      else if(doorLocation.topDoorWest) {

        roomRotation = 270;
        return templates.LLeftTop[(int)Mathf.Floor(Random.RandomRange(0, templates.LLeftTop.Length))];

      }

      else {
        roomRotation = 270;
        
        return templates.LMiddle[(int)Mathf.Floor(Random.RandomRange(0, templates.LMiddle.Length))];
      }

    }
    else if(doorLocation.hasDoorNorth // Should Rotate Room 270deg
    && !doorLocation.hasDoorEast
    && !doorLocation.hasDoorSouth
    && doorLocation.hasDoorWest) {

      Debug.Log("Coucou");

      if(doorLocation.topDoorNorth) {

        roomRotation = 0;
        return templates.LLeftTop[(int)Mathf.Floor(Random.RandomRange(0, templates.LLeftTop.Length))];

      }

      else if(doorLocation.topDoorWest) {

        roomRotation = 0;
        return templates.LRightTop[(int)Mathf.Floor(Random.RandomRange(0, templates.LRightTop.Length))];

      }

      else {
        roomRotation = 0;
        
        return templates.LMiddle[(int)Mathf.Floor(Random.RandomRange(0, templates.LMiddle.Length))];
      }

    }

    // Check For T Room --------------------------------------------------------

    if(doorLocation.hasDoorNorth
    && doorLocation.hasDoorEast
    && !doorLocation.hasDoorSouth
    && doorLocation.hasDoorWest) {

      if(doorLocation.topDoorNorth
      && !doorLocation.topDoorEast
      && !doorLocation.topDoorWest) {
        roomRotation = 0;
        return templates.TLeftRightMid[(int)Mathf.Floor(Random.RandomRange(0, templates.TLeftRightMid.Length))];
      }
      else if(!doorLocation.topDoorNorth
      && doorLocation.topDoorEast
      && !doorLocation.topDoorWest) {
        roomRotation = 0;
        return templates.TLeftTop[(int)Mathf.Floor(Random.RandomRange(0, templates.TLeftTop.Length))];
      }
      else if(!doorLocation.topDoorNorth
      && !doorLocation.topDoorEast
      && doorLocation.topDoorWest) {
        roomRotation = 90;
        return templates.TRightTop[(int)Mathf.Floor(Random.RandomRange(0, templates.TRightTop.Length))];
      }
      else if(doorLocation.topDoorNorth
      && doorLocation.topDoorEast
      && !doorLocation.topDoorWest) {
        roomRotation = 0;
        return templates.TLeftAndFrontTop[(int)Mathf.Floor(Random.RandomRange(0, templates.TLeftAndFrontTop.Length))];
      }
      else if(doorLocation.topDoorNorth
      && !doorLocation.topDoorEast
      && doorLocation.topDoorWest) {
        roomRotation = 180;
        return templates.TRightAndFrontTop[(int)Mathf.Floor(Random.RandomRange(0, templates.TRightAndFrontTop.Length))];
      }
      else if(!doorLocation.topDoorNorth
      && doorLocation.topDoorEast
      && doorLocation.topDoorWest) {
        roomRotation = 0;
        return templates.TLeftAndRightTop[(int)Mathf.Floor(Random.RandomRange(0, templates.TLeftAndRightTop.Length))];
      }
      else {
        roomRotation = 0;
        
        return templates.TMiddle[(int)Mathf.Floor(Random.RandomRange(0, templates.TMiddle.Length))];
      }

    }
    else if(doorLocation.hasDoorNorth
    && doorLocation.hasDoorEast
    && doorLocation.hasDoorSouth
    && !doorLocation.hasDoorWest) {

      if(doorLocation.topDoorNorth
      && !doorLocation.topDoorEast
      && !doorLocation.topDoorSouth) {
        roomRotation = 180;
        return templates.TRightTop[(int)Mathf.Floor(Random.RandomRange(0, templates.TRightTop.Length))];
      }
      else if(!doorLocation.topDoorNorth
      && doorLocation.topDoorEast
      && !doorLocation.topDoorSouth) {
        roomRotation = 90;
        return templates.TLeftRightMid[(int)Mathf.Floor(Random.RandomRange(0, templates.TLeftRightMid.Length))];
      }
      else if(!doorLocation.topDoorNorth
      && !doorLocation.topDoorEast
      && doorLocation.topDoorSouth) {
        roomRotation = 90;
        return templates.TLeftTop[(int)Mathf.Floor(Random.RandomRange(0, templates.TLeftTop.Length))];
      }
      else if(doorLocation.topDoorNorth
      && doorLocation.topDoorEast
      && !doorLocation.topDoorSouth) {
        roomRotation = 270;
        return templates.TRightAndFrontTop[(int)Mathf.Floor(Random.RandomRange(0, templates.TRightAndFrontTop.Length))];
      }
      else if(doorLocation.topDoorNorth
      && !doorLocation.topDoorEast
      && doorLocation.topDoorSouth) {
        roomRotation = 90;
        return templates.TLeftAndRightTop[(int)Mathf.Floor(Random.RandomRange(0, templates.TLeftAndRightTop.Length))];
      }
      else if(!doorLocation.topDoorNorth
      && doorLocation.topDoorEast
      && doorLocation.topDoorSouth) {
        roomRotation = 90;
        return templates.TLeftAndFrontTop[(int)Mathf.Floor(Random.RandomRange(0, templates.TLeftAndFrontTop.Length))];
      }
      else {
        roomRotation = 90;
        
        return templates.TMiddle[(int)Mathf.Floor(Random.RandomRange(0, templates.TMiddle.Length))];
      }

    }
    else if(!doorLocation.hasDoorNorth
    && doorLocation.hasDoorEast
    && doorLocation.hasDoorSouth
    && doorLocation.hasDoorWest) {

      if(doorLocation.topDoorSouth
      && !doorLocation.topDoorEast
      && !doorLocation.topDoorWest) {
        roomRotation = 180;
        return templates.TLeftRightMid[(int)Mathf.Floor(Random.RandomRange(0, templates.TLeftRightMid.Length))];
      }
      else if(!doorLocation.topDoorSouth
      && doorLocation.topDoorEast
      && !doorLocation.topDoorWest) {
        roomRotation = 270;
        return templates.TRightTop[(int)Mathf.Floor(Random.RandomRange(0, templates.TRightTop.Length))];
      }
      else if(!doorLocation.topDoorSouth
      && !doorLocation.topDoorEast
      && doorLocation.topDoorWest) {
        roomRotation = 180;
        return templates.TLeftTop[(int)Mathf.Floor(Random.RandomRange(0, templates.TLeftTop.Length))];
      }
      else if(doorLocation.topDoorSouth
      && doorLocation.topDoorEast
      && !doorLocation.topDoorWest) {
        roomRotation = 0;
        return templates.TRightAndFrontTop[(int)Mathf.Floor(Random.RandomRange(0, templates.TRightAndFrontTop.Length))];
      }
      else if(doorLocation.topDoorSouth
      && !doorLocation.topDoorEast
      && doorLocation.topDoorWest) {
        roomRotation = 180;
        return templates.TLeftAndFrontTop[(int)Mathf.Floor(Random.RandomRange(0, templates.TLeftAndFrontTop.Length))];
      }
      else if(!doorLocation.topDoorSouth
      && doorLocation.topDoorEast
      && doorLocation.topDoorWest) {
        roomRotation = 180;
        return templates.TLeftAndRightTop[(int)Mathf.Floor(Random.RandomRange(0, templates.TLeftAndRightTop.Length))];
      }
      else {
        roomRotation = 180;
        
        return templates.TMiddle[(int)Mathf.Floor(Random.RandomRange(0, templates.TMiddle.Length))];
      }

    }
    else if(doorLocation.hasDoorNorth
    && !doorLocation.hasDoorEast
    && doorLocation.hasDoorSouth
    && doorLocation.hasDoorWest) {

      if(doorLocation.topDoorNorth
      && !doorLocation.topDoorSouth
      && !doorLocation.topDoorWest) {
        roomRotation = 270;
        return templates.TLeftTop[(int)Mathf.Floor(Random.RandomRange(0, templates.TLeftTop.Length))];
      }
      else if(!doorLocation.topDoorNorth
      && doorLocation.topDoorSouth
      && !doorLocation.topDoorWest) {
        roomRotation = 0;
        return templates.TRightTop[(int)Mathf.Floor(Random.RandomRange(0, templates.TRightTop.Length))];
      }
      else if(!doorLocation.topDoorNorth
      && !doorLocation.topDoorSouth
      && doorLocation.topDoorWest) {
        roomRotation = 270;
        return templates.TLeftRightMid[(int)Mathf.Floor(Random.RandomRange(0, templates.TLeftRightMid.Length))];
      }
      else if(doorLocation.topDoorNorth
      && doorLocation.topDoorSouth
      && !doorLocation.topDoorWest) {
        roomRotation = 270;
        return templates.TLeftAndRightTop[(int)Mathf.Floor(Random.RandomRange(0, templates.TLeftAndRightTop.Length))];
      }
      else if(doorLocation.topDoorNorth
      && !doorLocation.topDoorSouth
      && doorLocation.topDoorWest) {
        roomRotation = 270;
        return templates.TLeftAndFrontTop[(int)Mathf.Floor(Random.RandomRange(0, templates.TLeftAndFrontTop.Length))];
      }
      else if(!doorLocation.topDoorNorth
      && doorLocation.topDoorSouth
      && doorLocation.topDoorWest) {
        roomRotation = 90;
        return templates.TRightAndFrontTop[(int)Mathf.Floor(Random.RandomRange(0, templates.TRightAndFrontTop.Length))];
      }
      else {
        roomRotation = 270;
        
        return templates.TMiddle[(int)Mathf.Floor(Random.RandomRange(0, templates.TMiddle.Length))];
      }

    }

    // Check For All Side Room -------------------------------------------------

    if(doorLocation.hasDoorNorth
    && doorLocation.hasDoorEast
    && doorLocation.hasDoorSouth
    && doorLocation.hasDoorWest) {

      if(doorLocation.topDoorNorth
      && !doorLocation.topDoorEast
      && !doorLocation.topDoorSouth
      && !doorLocation.topDoorWest) {
        roomRotation = 0;
        return templates.AllSideOneTop[(int)Mathf.Floor(Random.RandomRange(0, templates.AllSideOneTop.Length))];
      }
      else if(!doorLocation.topDoorNorth
      && doorLocation.topDoorEast
      && !doorLocation.topDoorSouth
      && !doorLocation.topDoorWest) {
        roomRotation = 90;
        return templates.AllSideOneTop[(int)Mathf.Floor(Random.RandomRange(0, templates.AllSideOneTop.Length))];
      }
      else if(!doorLocation.topDoorNorth
      && !doorLocation.topDoorEast
      && doorLocation.topDoorSouth
      && !doorLocation.topDoorWest) {
        roomRotation = 180;
        return templates.AllSideOneTop[(int)Mathf.Floor(Random.RandomRange(0, templates.AllSideOneTop.Length))];
      }
      else if(!doorLocation.topDoorNorth
      && !doorLocation.topDoorEast
      && !doorLocation.topDoorSouth
      && doorLocation.topDoorWest) {
        roomRotation = 270;
        return templates.AllSideOneTop[(int)Mathf.Floor(Random.RandomRange(0, templates.AllSideOneTop.Length))];
      }
      else if(doorLocation.topDoorNorth
      && !doorLocation.topDoorEast
      && doorLocation.topDoorSouth
      && !doorLocation.topDoorWest) {
        roomRotation = 0;
        return templates.AllSideTunnelTop[(int)Mathf.Floor(Random.RandomRange(0, templates.AllSideTunnelTop.Length))];
      }
      else if(!doorLocation.topDoorNorth
      && doorLocation.topDoorEast
      && !doorLocation.topDoorSouth
      && doorLocation.topDoorWest) {
        roomRotation = 90;
        return templates.AllSideTunnelTop[(int)Mathf.Floor(Random.RandomRange(0, templates.AllSideTunnelTop.Length))];
      }
      else if(doorLocation.topDoorNorth
      && doorLocation.topDoorEast
      && !doorLocation.topDoorSouth
      && !doorLocation.topDoorWest) {
        roomRotation = 0;
        return templates.AllSideLTop[(int)Mathf.Floor(Random.RandomRange(0, templates.AllSideLTop.Length))];
      }
      else if(!doorLocation.topDoorNorth
      && doorLocation.topDoorEast
      && doorLocation.topDoorSouth
      && !doorLocation.topDoorWest) {
        roomRotation = 90;
        return templates.AllSideLTop[(int)Mathf.Floor(Random.RandomRange(0, templates.AllSideLTop.Length))];
      }
      else if(!doorLocation.topDoorNorth
      && !doorLocation.topDoorEast
      && doorLocation.topDoorSouth
      && doorLocation.topDoorWest) {
        roomRotation = 180;
        return templates.AllSideLTop[(int)Mathf.Floor(Random.RandomRange(0, templates.AllSideLTop.Length))];
      }
      else if(doorLocation.topDoorNorth
      && !doorLocation.topDoorEast
      && !doorLocation.topDoorSouth
      && doorLocation.topDoorWest) {
        roomRotation = 270;
        return templates.AllSideLTop[(int)Mathf.Floor(Random.RandomRange(0, templates.AllSideLTop.Length))];
      }
      else if(doorLocation.topDoorNorth
      && doorLocation.topDoorEast
      && !doorLocation.topDoorSouth
      && doorLocation.topDoorWest) {
        roomRotation = 0;
        return templates.AllSideTTop[(int)Mathf.Floor(Random.RandomRange(0, templates.AllSideTTop.Length))];
      }
      else if(doorLocation.topDoorNorth
      && doorLocation.topDoorEast
      && doorLocation.topDoorSouth
      && !doorLocation.topDoorWest) {
        roomRotation = 90;
        return templates.AllSideTTop[(int)Mathf.Floor(Random.RandomRange(0, templates.AllSideTTop.Length))];
      }
      else if(!doorLocation.topDoorNorth
      && doorLocation.topDoorEast
      && doorLocation.topDoorSouth
      && doorLocation.topDoorWest) {
        roomRotation = 180;
        return templates.AllSideTTop[(int)Mathf.Floor(Random.RandomRange(0, templates.AllSideTTop.Length))];
      }
      else if(doorLocation.topDoorNorth
      && !doorLocation.topDoorEast
      && doorLocation.topDoorSouth
      && doorLocation.topDoorWest) {
        roomRotation = 270;
        return templates.AllSideTTop[(int)Mathf.Floor(Random.RandomRange(0, templates.AllSideTTop.Length))];
      }
      else {
        roomRotation = 0;
        
        return templates.AllSide[(int)Mathf.Floor(Random.RandomRange(0, templates.AllSide.Length))];
      }
    }

    // Return A Template If Nothing Was There ----------------------------------

    return templates.templateGround;

  }

}
