using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerMovement {

  [Header ("Movements")]
  public float speed;
  public float groundDrag;
  public float airDragMultiplier;

  [Header ("Jump")]
  public float jumpStrength;
  public float jumpCooldown;
  [HideInInspector] public bool canJump;
  [HideInInspector] public bool isGround;

  [Header ("Direction Vector")]
  public Transform orientation;
  [HideInInspector] public Vector3 moveDirection;
  
}



public class PlayerBehavior : MonoBehaviour {

  [Header ("Movement")]
  public PlayerMovement player;
  private Rigidbody rb;


  [Header ("Keybinds")]
  public KeyCode jumpkey = KeyCode.Space;


  [Header ("Inputs")]
  private float horizontalInput;
  private float verticalInput;


  private void Start() {
    rb = GetComponent<Rigidbody>();
    rb.freezeRotation = true;
    player.canJump = true;
  }

  private void Update() {
    player.isGround = Physics.Raycast(transform.position, Vector3.down, 1.05f);

    MovementInput();
    SpeedControler();

    if(player.isGround) rb.drag = player.groundDrag;
    else rb.drag = 0;
  }

  private void FixedUpdate() {
    MovementPlayer();
  }

  private void MovementInput() {
    horizontalInput = Input.GetAxisRaw("Horizontal");
    verticalInput = Input.GetAxisRaw("Vertical");

    if(Input.GetKey(jumpkey) && player.canJump && player.isGround) {
      player.canJump = false;
      Jump();
      Invoke(nameof(ResetJump), player.jumpCooldown);
    }
  }

  private void MovementPlayer() {
    player.moveDirection = player.orientation.forward * verticalInput + player.orientation.right * horizontalInput;

    if(player.isGround) rb.AddForce(player.moveDirection.normalized * player.speed * 10f, ForceMode.Force);
    else if(!player.isGround) rb.AddForce(player.moveDirection.normalized * player.speed * 10f * player.airDragMultiplier, ForceMode.Force);

  }

  private void SpeedControler() {
    Vector3 flatVel = new (rb.velocity.x, 0f, rb.velocity.z);

    if(flatVel.magnitude > player.speed) {
      Vector3 limitVel = flatVel.normalized * player.speed;
      rb.velocity = new Vector3(limitVel.x, rb.velocity.y, limitVel.z);
    }
  }

  private void Jump() {
    rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

    rb.AddForce(transform.up * player.jumpStrength, ForceMode.Impulse);
  }

  private void ResetJump() {
    player.canJump = true;
  }

}
