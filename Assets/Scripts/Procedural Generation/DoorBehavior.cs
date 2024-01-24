using UnityEngine;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0051")]

public class DoorBehavior : MonoBehaviour
{
  public bool canSpawnRoom;
  public bool hasSpawnedRoom;


  private void OnTriggerEnter(Collider other) {
    if(!hasSpawnedRoom) {

      if(other.gameObject.tag == "Room"
      && !transform.IsChildOf(other.transform)) {
        Destroy(this.gameObject);
      }

      else if(other.gameObject.name == "FakeRoom") {
        Destroy(this.gameObject);
      }
      
      else {
        canSpawnRoom = true;
      }
    }
  }
}
