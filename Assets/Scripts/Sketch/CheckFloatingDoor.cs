using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CheckFloatingDoor : MonoBehaviour {

  [HideInInspector] public bool isFloating;

  private void Update() {
    // Check if the object is not touching any other colliders
    if (!IsTouchingAnyCollider()) {
      // If it's not touching, destroy the object
      isFloating = true;
    }
  }

  private bool IsTouchingAnyCollider() {
    // Get all the colliders attached to this object
    Collider[] colliders = GetComponents<Collider>();

    // Check each collider for overlaps with other colliders
    foreach (Collider collider in colliders) {
      if(CheckForOverlap(collider)) {
        // If any collider is touching another collider, return true
        return true;
      }
    }

    // If no collider is touching any other collider, return false
    return false;
  }

  private bool CheckForOverlap(Collider collider) {
    // Adjust the size of the overlap box based on the collider type (BoxCollider, SphereCollider, etc.)
    // You might need to fine-tune these values based on the size of your object
    Vector3 size = collider.bounds.size * 1.1f; // Slightly larger than the collider's size
    Vector3 center = collider.bounds.center;

    // Check if there's any overlap with other colliders using Physics.OverlapBox
    Collider[] overlappingColliders = Physics.OverlapBox(center, size / 2f);

    // Ignore the collider itself from the list of overlapping colliders
    overlappingColliders = overlappingColliders.Where(c => c != collider).ToArray();

    return overlappingColliders.Length > 0;
  }

  // void OnDrawGizmos() {
  //   Renderer renderer = GetComponent<Renderer>();
  //   Bounds bounds = renderer.bounds;
  //   Vector3 center = bounds.center;
  //   Vector3 size = bounds.size;
  //   Vector3 halfExtents = size / 2f;
  //   Gizmos.color = Color.red;
  //   Gizmos.DrawWireCube(center, size);
  // }

}
