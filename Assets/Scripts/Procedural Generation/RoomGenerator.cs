using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0047")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0051")]

public class RoomGenerator : MonoBehaviour
{

  public List<GameObject> doors;
  public List<GameObject> rooms;
  private RoomTemplates templates;
  public bool isEnabled = true;

  private void Awake() {  // On awakening, start up all value and start the Spawning Loop
    isEnabled = true;
    templates = GameObject.FindGameObjectWithTag("RoomTemplate").GetComponent<RoomTemplates>();
    rooms = new List<GameObject>(GameObject.FindGameObjectsWithTag("Room"));
    templates.nbRooms = rooms.Count;
    StartCoroutine(SpawnRoomsCoroutine());
  }

  private IEnumerator SpawnRoomsCoroutine() { // While the script isEnabled, SpawnRoom every X seconds
    while (isEnabled) {
      SpawnRoom();
      yield return new WaitForSeconds(0.0000001f);
      // yield return new WaitForSeconds(0.01f);
    }
    Debug.Log("Generation Completed");
  }

  public void SpawnRoom() {

    if(templates.nbRooms > templates.maxRooms) {  // Reload if more room than the limit
      templates.ReloadScene();
    }

    doors = new List<GameObject>(); // New Empty List of doors

    foreach(GameObject room in GameObject.FindGameObjectsWithTag("Room")) {  // Get all doors that hasn't spawn a room yet
      if(!room.GetComponent<RoomBehavior>().hasBeenCounted) {
        room.GetComponent<RoomBehavior>().hasBeenCounted = true;
        foreach(Transform childDoor in room.transform) {
          if(childDoor.CompareTag("Door") && !childDoor.GetComponent<DoorBehavior>().hasSpawnedRoom) {
            doors.Add(childDoor.gameObject);
          }
        }
      }
      else {
        rooms.Add(room);  // Add those room to a List for potential future usage
      }
    }

    if(doors.Count == 0) { isEnabled = false; } // Disable the script if no more door to spawn room

    if(isEnabled) {

      foreach(GameObject door in doors) { // Execute the following for all door

        DoorBehavior doorBehavior = door.GetComponent<DoorBehavior>();  // Get some value for clean code and efficiency
        Vector3 doorParent = door.transform.parent.transform.position;
        float doorHeight = door.transform.position.y;

        if(doorBehavior.canSpawnRoom && !doorBehavior.hasSpawnedRoom) { // Execute if the door can spawn a room

          Vector3 position = Vector3.zero;  // Set a position to (0, 0, 0)
          Vector3 direction = door.transform.position - doorParent; // The direction is based on the door position - Ex: (-0.5, 0, 0)


          // Depending on the direction, we will change the position value
          // If direction is positive X, we add the length X of room prefab from the previous room (parent of the door)
          // Execute for all 4 directions

          // string roomName = "";

          if(direction.x > 0) {
            position = new Vector3((doorParent.x + templates.roomX), doorHeight, doorParent.z);
            // roomName = "X Positif ";
          }
          else if(direction.x < 0) {
            position = new Vector3((doorParent.x - templates.roomX), doorHeight, doorParent.z);
            // roomName = "X Negatif ";
          }
          else if(direction.z > 0) {
            position = new Vector3(doorParent.x, doorHeight, (doorParent.z + templates.roomZ));
            // roomName = "Z Positif ";
          }
          else if(direction.z < 0) {
            position = new Vector3(doorParent.x, doorHeight, (doorParent.z - templates.roomZ));
            // roomName = "Z Negatif ";
          }

          bool canGenerate = true;

          Collider[] colliders = Physics.OverlapSphere(position, 0.1f);
          foreach (Collider collider in colliders) {
            if(collider.CompareTag("Room")) {
              canGenerate = false;
            }
          }

          // Instantiate a new room template at the location (position) and give it a name and tag
          // Set hasSpawnedRoom to true to the door

          if(canGenerate) {
            GameObject spawnedRoom = Instantiate(templates.templateRoom, position, Quaternion.identity);
            doorBehavior.hasSpawnedRoom = true;
            templates.nbRooms++;
            spawnedRoom.name = templates.nbRooms.ToString();
            spawnedRoom.tag = "Room";

            // ----------------------------------------------------------------------------------------------
            // ----------------------------------------------------------------------------------------------
            // ----------------------------------------------------------------------------------------------

            if(templates.nbRooms < templates.minRooms) {
              int doorToRemove = Random.Range(templates.spread, 4); // Choose number between 0 and 3
              int childCount = spawnedRoom.transform.childCount;  // Get amount of child for the spawned room

              for(int i = 0; i < doorToRemove; i++) { // Destroy a random door "doorToRemove" times
                int rand = Random.Range(0, childCount);
                Destroy(spawnedRoom.transform.GetChild(rand).gameObject);
              }
            }

            else {
              for (int i = 0; i < spawnedRoom.transform.childCount; i++) {
                Destroy(spawnedRoom.transform.GetChild(i).gameObject);
              }
            }

            // ----------------------------------------------------------------------------------------------
            // ----------------------------------------------------------------------------------------------
            // ----------------------------------------------------------------------------------------------

            for (int i = 0; i < spawnedRoom.transform.childCount; i++) {
              Transform doorTransform = spawnedRoom.transform.GetChild(i).transform;
              int randHeightPercentage = Random.Range(1, 101);

              if(templates.heightVariantPercentage == 0) {
                doorTransform.position = doorTransform.position; // Stay the same
              }

              else if(randHeightPercentage <= templates.heightVariantPercentage) {
                doorTransform.position = new Vector3(doorTransform.position.x, doorTransform.position.y + templates.doorSpawnHeight, doorTransform.position.z);
              }

              DoorBehavior childDoorBehavior = spawnedRoom.transform.GetChild(i).gameObject.GetComponent<DoorBehavior>();
              childDoorBehavior.canSpawnRoom = true;
              childDoorBehavior.hasSpawnedRoom = false;
            }
            doorBehavior.hasSpawnedRoom = true;
          }

        }
      }
    }    
  }
}
