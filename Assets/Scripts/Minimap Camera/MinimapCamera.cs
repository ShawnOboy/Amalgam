using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapCamera : MonoBehaviour{

  public float mapX;
  public float mapZ;
  public Vector3 center;
  private float zoom;
  public GameObject fullScreenMap;
  public RawImage fullScreenMapImage;
  public GameObject minimap;
  private bool fullMapOn;
  private RoomTemplates templates;
  
  private float dragSpeed;
  private bool isDragging;
  private Vector2 dragStartPos;
  private Vector3 camStartPos;

  private void Awake() {
    templates = GameObject.FindGameObjectWithTag("RoomTemplate").GetComponent<RoomTemplates>();
  }

  private void Update() {

    if(Input.GetKeyDown(KeyCode.Tab)) {

      fullMapOn = !fullMapOn;

      if(fullMapOn) {

        fullScreenMap.SetActive(true);
        minimap.SetActive(false);

        if(mapX > mapZ) {zoom = (mapX/2) + 1;}
        if(mapX < mapZ) {zoom = (mapZ/2) + 1;}
        transform.position = new Vector3(center.x, transform.position.y, center.z);

      }

      else if(!fullMapOn) {

        fullScreenMap.SetActive(false);
        minimap.SetActive(true);

      }

    }

    if(fullMapOn) {

      float scrollInput = Input.GetAxis("Mouse ScrollWheel");
      float mouseX = Input.GetAxis("Mouse X");
      float mouseY = Input.GetAxis("Mouse Y");

      if(scrollInput != 0) {
        if(scrollInput > 0) {zoom--;}
        else if(scrollInput < 0) {zoom++;}

        if(zoom < templates.roomX * 2) {zoom = templates.roomX * 2;}
        if(zoom > templates.roomX * 10) {zoom = templates.roomX * 10;}
      }
      GetComponent<Camera>().orthographicSize = zoom;

      if(Input.GetKeyDown(KeyCode.Mouse0)) {

        // Check is mousePosition is in RectTransform of Fullscreen Map
        if(RectTransformUtility.RectangleContainsScreenPoint(fullScreenMapImage.rectTransform, Input.mousePosition)) {
          isDragging = true;
          dragStartPos = Input.mousePosition;
          camStartPos = transform.position;
        }
      }

      if(Input.GetKeyUp(KeyCode.Mouse0)) {
        isDragging = false;
      }

      if(isDragging) {

        dragSpeed = (0.0025f * zoom) - 0.0125f;
        Vector2 dragDeltaRaw = ((Vector2)Input.mousePosition - dragStartPos);
        Vector2 dragDelta = dragDeltaRaw * dragSpeed;
        Vector3 newPosition = camStartPos + transform.right * -dragDelta.x + transform.up * -dragDelta.y;
        transform.position = newPosition;

        float newX = transform.position.x;
        float newZ = transform.position.z;

        if(transform.position.x > (center.x + (mapX/2))) {newX = (center.x + (mapX/2));}
        else if(transform.position.x < (center.x - (mapX/2))) {newX = (center.x - (mapX/2));}

        if(transform.position.z > (center.z + (mapZ/2))) {newZ = (center.z + (mapZ/2));}
        else if(transform.position.z < (center.z - (mapZ/2))) {newZ = (center.z - (mapZ/2));}

        transform.position = new Vector3(newX, transform.position.y, newZ);

      }

      if(Input.GetKeyDown(KeyCode.Space) && !isDragging) {
        if(mapX > mapZ) {zoom = (mapX/2) + 1;}
        if(mapX < mapZ) {zoom = (mapZ/2) + 1;}
        transform.position = new Vector3(center.x, transform.position.y, center.z);
      }
    }
  }
}
