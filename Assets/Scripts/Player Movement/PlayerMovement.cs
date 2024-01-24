using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

  [Header ("Movement")]
  public float moveSpeed;
  public float groundDrag;
  public float jumpForce;
  public float jumpCooldown;
  public float airMultiplier;
  private bool canJump = true;

  [Header ("Keybinds")]
  public KeyCode jumpkey = KeyCode.Space;

  [Header ("Ground Check")]

  public float playerHeight;
  public LayerMask theGround;
  private bool isGrounded;

  public Transform orientation;

  private float horizontalInput;
  private float verticalInput;

  private Vector3 moveDirection;

  private Rigidbody rb;

  private void Start() {
    rb = GetComponent<Rigidbody>();
    rb.freezeRotation = true;
  }

  private void Update() {
    
    isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight/2 + playerHeight/5, theGround);

    MyInput();
    SpeedControl();

    if(isGrounded) rb.drag = groundDrag;
    else rb.drag = 0;
  }

  private void FixedUpdate() {
    MovePlayer();
  }

  private void MyInput() {
    horizontalInput = Input.GetAxisRaw("Horizontal");
    verticalInput = Input.GetAxisRaw("Vertical");

    Debug.Log("Jump" + canJump);
    Debug.Log("Ground" + isGrounded);

    if(Input.GetKey(jumpkey) && canJump && isGrounded) {
      Debug.Log("BleBlaBlou");
      canJump = false;
      Jump();
      Invoke(nameof(ResetJump), jumpCooldown);
    }
  }

  private void MovePlayer() {
    moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

    if(isGrounded) rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    else if(!isGrounded) rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

  }

  private void SpeedControl() {
    Vector3 flatVel = new (rb.velocity.x, 0f, rb.velocity.z);

    if(flatVel.magnitude > moveSpeed) {
      Vector3 limitVel = flatVel.normalized * moveSpeed;
      rb.velocity = new Vector3(limitVel.x, rb.velocity.y, limitVel.z);
    }
  }

  private void Jump() {
    rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

    rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
  }

  private void ResetJump() {
    canJump = true;
  }

}
