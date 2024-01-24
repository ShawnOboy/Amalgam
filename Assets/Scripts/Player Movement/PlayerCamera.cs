using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

  public float sensX;
  public float sensY;
  public Transform orientation;
  public Transform cameraPos;
  private float rotationX;
  private float rotationY;
  private GameObject player;

  private void Start() {
    player = GameObject.FindGameObjectWithTag("Player");
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }

  private void Update() {

    transform.position = cameraPos.position;
    
    float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX; 
    float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

    rotationX -= mouseY;
    rotationY += mouseX;

    rotationX = Mathf.Clamp(rotationX, -90, 90);

    transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
    orientation.rotation = Quaternion.Euler(0, rotationY, 0);
    
  }

}
