using UnityEngine;

public class DestroyRoom : MonoBehaviour
{
  private void OnCollisionEnter(Collision roomCollision) {
    if(roomCollision.gameObject.name != "StartingRoom") {
      Destroy(roomCollision.gameObject);
      Debug.Log("Room Destroyed");
    }
  }
}