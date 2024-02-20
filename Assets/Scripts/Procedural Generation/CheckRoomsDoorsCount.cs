using UnityEngine;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0018")]

public class CheckRoomsDoorsCount : MonoBehaviour
{

  private RoomTemplates templates;
  private DungeonSettings _DS;

  [Header ("North")]
  public bool hasDoorNorth;
  public bool topDoorNorth;

  [Header ("East")]
  public bool hasDoorEast;
  public bool topDoorEast;

  [Header ("South")]
  public bool hasDoorSouth;
  public bool topDoorSouth;

  [Header ("West")]
  public bool hasDoorWest;
  public bool topDoorWest;

  public void ShootRaycasts() {
    templates = GameObject.FindGameObjectWithTag("RoomTemplate").GetComponent<RoomTemplates>();
    _DS = templates.GetDungeonSettings();
    ShootRaycast(Vector3.forward, 0); // North
    ShootRaycast(Vector3.right, 1);   // East
    ShootRaycast(Vector3.back, 2);    // South
    ShootRaycast(Vector3.left, 3);    // West

  }

  private void ShootRaycast(Vector3 direction, int intDirection) {
    RaycastAtPosition(direction, 0f, intDirection);
    RaycastAtPosition(direction, 41, intDirection); // 41 is more than half the height of starting pillar
  }

  private void RaycastAtPosition(Vector3 direction, float yOffset, int intDirection) {
    Vector3 origin = new(transform.position.x, transform.position.y + yOffset, transform.position.z);

    RaycastHit hit;

    if (Physics.Raycast(origin, direction, out hit, _DS.roomSize)) {
      if(hit.transform.tag == "Door") {

        switch(intDirection) {
          case 0 :
            hasDoorNorth = true;
            if(yOffset > 0) { topDoorNorth = true;}
            break;
          case 1 :
            hasDoorEast = true;
            if(yOffset > 0) { topDoorEast = true; }
            break;
          case 2 :
            hasDoorSouth = true;
            if(yOffset > 0) { topDoorSouth = true; }
            break;
          case 3 :
            hasDoorWest = true;
            if(yOffset > 0) { topDoorWest = true; }
            break;
        }
      }
    }
  }
}
