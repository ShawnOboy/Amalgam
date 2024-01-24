using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{

  // [Range(1, 4)]
  // public int startDirection;
  // private int rnd;
  // private RoomTemplates templates;
  // private bool spawned;


  // private void OnTriggerEnter(Collider roomCollision) {
  //   Destroy(gameObject);
  //   Debug.LogWarning("A room already exist here, can't spawn one here");
  //   spawned = true;
  // }

  // public IEnumerator Spawn(bool lastIteration) {
  //   yield return new WaitForSeconds(0.1f);
  //   if (templates == null) {
  //     templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
  //     Debug.Log("Template Trouve -- Peut Spawn une salle");
  //   }
  //   if(!spawned) { // Si n'a pas encore spawn
  //     Debug.Log("Salle pas encore spawn");
  //     Debug.Log("Regarde quelle direction la porte doit avoir");
  //     switch (startDirection) {
  //       case 1: // Entré haut, besoin porte bas
  //         Debug.Log("Entré haut, besoin porte bas");
  //         if(!lastIteration) {
  //           Debug.Log("N'est pas une derniere iteration");
  //           rnd = Random.Range(0, templates.bottomRooms.Length);
  //           Instantiate(templates.bottomRooms[rnd], transform.position, Quaternion.identity);
  //           Debug.Log("A cree une salle");
  //         }
  //         else {
  //           Instantiate(templates.closedBottomRoom, transform.position, Quaternion.identity);
  //           Debug.Log("A cree une salle ferme");
  //         }
  //         break;

  //       case 2: // Entré droite, besoin porte gauche
  //         Debug.Log("Entré droite, besoin porte gauche");
  //         if(!lastIteration) {
  //           rnd = Random.Range(0, templates.leftRooms.Length);
  //           Instantiate(templates.leftRooms[rnd], transform.position, Quaternion.identity);
  //         }
  //         else {
  //           Instantiate(templates.closedLeftRoom, transform.position, Quaternion.identity);
  //         }
  //         break;

  //       case 3: // Entré bas, besoin porte haut
  //         Debug.Log("Entré bas, besoin porte haut");
  //         if(!lastIteration) {
  //           rnd = Random.Range(0, templates.topRooms.Length);
  //           Instantiate(templates.topRooms[rnd], transform.position, Quaternion.identity);
  //         }
  //         else {
  //           Instantiate(templates.closedTopRoom, transform.position, Quaternion.identity);
  //         }
  //         break;

  //       case 4: // Entré gauche, besoin porte droite
  //         Debug.Log("Entré gauche, besoin porte droite");
  //         if(!lastIteration) {
  //           rnd = Random.Range(0, templates.rightRooms.Length);
  //           Instantiate(templates.rightRooms[rnd], transform.position, Quaternion.identity);
  //         }
  //         else {
  //           Instantiate(templates.closedRightRoom, transform.position, Quaternion.identity);
  //         }
  //         break;
  //     }
  //     spawned = true;
  //   }
  // }
}
