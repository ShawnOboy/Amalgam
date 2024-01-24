using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtatPorte : MonoBehaviour
{

  [HideInInspector] public bool porteOuverte;
  public bool porteEntree;
  public bool porteSortie;

  [Range(1, 4)]
  [Tooltip("Correspond à la position d'une porte dans la salle:\n1 = Porte haut\n2 = Porte droite\n3 = Porte bas\n4 = Porte gauche")]
  public int positionPorte;

  public void CreateRoom() {

    Debug.Log("CreateRoom Function was Called!");

  }

}
