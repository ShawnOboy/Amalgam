using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player {

  [Header ("Player Attribute")]

  [Header ("Movement")]
  [Range (0, 1)]
  public float speedFactor;
  [HideInInspector] public float speedX;
  public float speedZ;
  public float maxSpeed;
  public float acceleration;
  public float mass = 1f;

  [HideInInspector] public LastDirection lastDirectionWS;
  [HideInInspector] public LastDirection lastDirectionAD;

  [Header ("Jump")]
  public float jumpStrength;
  [HideInInspector] public bool isGround;
  [HideInInspector] public float MovementSpeed {
    get {
      return new Vector2(speedX, speedZ).magnitude;
    }
  }

  // [Header ("Health Stats")]
  // [Header ("Combat Stats")]
  // [Header ("Mobility Stats")]

}

public enum LastDirection {
  Left,
  Right,
  Forward,
  Backward
}

public class PlayerMovements : MonoBehaviour {
  
  public Player player;
  private bool goingX;
  private bool goingZ;

  private void Start() {
    player.acceleration = 1 / player.mass * player.speedFactor;
  }

  private void Update() {
    player.acceleration = 1 / player.mass * player.speedFactor;
    Movement();
  }

  private void Movement() {

    if(Input.GetKeyDown(KeyCode.W)) { player.lastDirectionWS = LastDirection.Forward; }
    if(Input.GetKeyDown(KeyCode.S)) { player.lastDirectionWS = LastDirection.Backward; }
    if(Input.GetKeyDown(KeyCode.A)) { player.lastDirectionAD = LastDirection.Left; }
    if(Input.GetKeyDown(KeyCode.D)) { player.lastDirectionAD = LastDirection.Right; }


    // Forward and Backward Directions
    if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)) {

      goingZ = true;

      if(player.lastDirectionWS == LastDirection.Forward) {
        if(player.speedZ > 0) { player.speedZ += player.acceleration; }
        else { player.speedZ += player.acceleration * 2; }
        if(player.speedZ > player.maxSpeed) { player.speedZ = player.maxSpeed; }
      }

      if(player.lastDirectionWS == LastDirection.Backward) {
        if(player.speedZ > 0) { player.speedZ -= player.acceleration; }
        else { player.speedZ -= player.acceleration * 2; }
        if(player.speedZ < -player.maxSpeed) {player.speedZ = -player.maxSpeed;}
      }
    }
    else {
      if(player.speedZ > 0) {
        player.speedZ -= player.acceleration;
        if(player.speedZ < 0) { player.speedZ = 0; goingZ = false; }
      }
      else if(player.speedZ < 0) {
        player.speedZ += player.acceleration;
        if(player.speedZ > 0) { player.speedZ = 0; goingZ = false; }
      }
    }

    // Left and Right Directions
    if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) {

      goingX = true;

      if(player.lastDirectionAD == LastDirection.Right) {
        if(player.speedX > 0) { player.speedX += player.acceleration; }
        else { player.speedX += player.acceleration * 2; }
        if(player.speedX > player.maxSpeed) { player.speedX = player.maxSpeed; }
      }

      if(player.lastDirectionAD == LastDirection.Left) {
        if(player.speedX > 0) { player.speedX -= player.acceleration; }
        else { player.speedX -= player.acceleration * 2; }
        if(player.speedX < -player.maxSpeed) { player.speedX = -player.maxSpeed; }
      }
    }
    else {
      if(player.speedX > 0) {
        player.speedX -= player.acceleration;
        if(player.speedX < 0) { player.speedX = 0; goingX = false; }
      }
      else if(player.speedX < 0) {
        player.speedX += player.acceleration;
        if(player.speedX > 0) { player.speedX = 0; goingX = false; }
      }
    }

    if(Input.GetKeyDown(KeyCode.Space)) {
      Debug.Log(new Vector3(player.speedX, player.jumpStrength, player.speedZ));
      Debug.Log(new Vector3(player.speedX, player.jumpStrength, player.speedZ).normalized);
    }

    GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(player.speedX, player.jumpStrength, player.speedZ).normalized * player.MovementSpeed);

    // GetComponent<Rigidbody>().AddRelativeForce(player.speedX, player.jumpStrength, player.speedZ, ForceMode.VelocityChange);

    // if(goingX && goingZ) {
    //   transform.position += new Vector3(player.speedX, player.jumpStrength, player.speedZ);
    // }
    // else {
    //   transform.position += new Vector3(player.speedX * 1.41f, player.jumpStrength, player.speedZ * 1.41f);
    // }

  }  

}
