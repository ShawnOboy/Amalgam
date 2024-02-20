using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom : MonoBehaviour {

  private GameObject childObject = new();
  private int distanceFromCenter;

  public void InitializeRoom(int roomSize) {
    distanceFromCenter = roomSize/2;
    CreateChild(Vector3.up);
    CreateChild(Vector3.right);
    CreateChild(Vector3.down);
    CreateChild(Vector3.left);
  }

  void CreateChild(Vector3 direction) {
    GameObject child = Instantiate(childObject, transform.position + direction * distanceFromCenter, Quaternion.identity);
    child.transform.parent = transform;
  }
}
