using System.Collections.Generic;
using UnityEngine;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0051")]

public class Rewind : MonoBehaviour
{

  private List<Vector3> objectsPostion = new();
  public bool canRewind = true;
  public bool isRewind = false;
  private readonly float readSpeed = 1/60f; // 60 times per second
  public float rewindEffectTime = 4f;
  public float rewindTime = 2f;
  private float rewindSpeed;
  public int rewindCD;
  public GameObject lastPositionObject;

  private void Start() {
    InvokeRepeating("AddNewPosition", 0f, readSpeed);
    rewindSpeed = 1 / (60 / rewindTime);
  }

  private void Update() {
    rewindSpeed = 1 / (60 / rewindTime);
    if(!isRewind) {
      if(Input.GetKey(KeyCode.W)) {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.5f);
      }
      if(Input.GetKey(KeyCode.A)) {
        transform.position = new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z);
      }
      if(Input.GetKey(KeyCode.S)) {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f);
      }
      if(Input.GetKey(KeyCode.D)) {
        transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z);
      }

      if(canRewind) {
        if(Input.GetKeyDown(KeyCode.Space)) {
          canRewind = false;
          isRewind = true;
          Invoke("RewindCooldown", rewindCD);
          InvokeRepeating("RewindEffect", 0f, rewindSpeed);
        }
      }
    }
    lastPositionObject.transform.position = objectsPostion[0];
  }

  private void AddNewPosition() {
    if(!isRewind) {
      if(objectsPostion.Count < rewindEffectTime * 60) {
        objectsPostion.Add(transform.position); // Can also add player's hp, energy, etc
      }

      else {
        objectsPostion.RemoveAt(0);
        objectsPostion.Add(transform.position);
      }
    }
  }

  private void RewindEffect() {

    if(objectsPostion.Count > 0) {
      Vector3 newPosition = objectsPostion[^1];
      objectsPostion.RemoveAt(objectsPostion.Count - 1);
      transform.position = newPosition;
    }

    if(objectsPostion.Count == 0) {
      CancelInvoke("RewindEffect");
      isRewind = false;
    } 
  }

  private void RewindCooldown() {
    canRewind = true;
  }

}
