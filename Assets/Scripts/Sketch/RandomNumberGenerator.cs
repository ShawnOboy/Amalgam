using System.Collections.Generic;
using UnityEngine;

public class RandomNumberGenerator : MonoBehaviour
{
    [Tooltip("Minimum amount of rooms to be spawned.")]
    public int minIteration;
    [Tooltip("Maximum amount of rooms to be spawned.")]
    public int maxIteration;
    [Min(0)]
    [Tooltip("Scale X of rooms spawned. All rooms should have an equal X.")]
    public int spacingValueX;
    [Min(0)]
    [Tooltip("Scale Z of rooms spawned. All rooms should have an equal Z.")]
    public int spacingValueZ;
    [Min(1)]
    [Tooltip("Limits how big the dungeon can be. Should be in range of Minimum and Maximum number of rooms divided by lowest Spacing Value.")]
    public int spreadQuantity;
    private RoomTemplates templates;

    private void Start() {
      templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();

      int firstBranch = Random.Range(minIteration, maxIteration);
      int secondBranch = Random.Range(minIteration, maxIteration);
      int thirdBranch = Random.Range(minIteration, maxIteration);

      Debug.Log("Iteration Branche Gauche: " + firstBranch);
      Debug.Log("Iteration Branche Haut: " + secondBranch);
      Debug.Log("Iteration Branche Droite: " + thirdBranch);

      GenerateRooms(firstBranch, secondBranch, thirdBranch);
    }

    // Si jamais je veux plus de contrôle (Retirer du Start)
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Mouse0)) {

    //         int firstBranch = Random.Range(minIteration, maxIteration);
    //         int secondBranch = Random.Range(minIteration, maxIteration);
    //         int thirdBranch = Random.Range(minIteration, maxIteration);

    //         // Si jamais les branches doivent être mélangés
    //         // ShuffleNumbers(ref firstBranch, ref secondBranch, ref thirdBranch);

    //         Debug.Log("First Number: " + firstBranch);
    //         Debug.Log("Second Number: " + secondBranch);
    //         Debug.Log("Third Number: " + thirdBranch);

    //         GenerateRooms(firstBranch, secondBranch, thirdBranch);
    //     }
    // }

    private void ShuffleNumbers(ref int a, ref int b, ref int c)
    {
        int[] numbers = { a, b, c };
        for (int i = numbers.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = numbers[i];
            numbers[i] = numbers[randomIndex];
            numbers[randomIndex] = temp;
        }

        a = numbers[0];
        b = numbers[1];
        c = numbers[2];
    }

    private void DestroyExistingRooms() {
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");
        for (int i = 0; i < rooms.Length; i++) {
            if (rooms[i].name != "StartingRoom") {
                Destroy(rooms[i]);
            }
        }
    }

    private void GenerateRooms(int firstBranch, int secondBranch, int thirdBranch) {

        DestroyExistingRooms();

        GenerateBranch(0, firstBranch);
        GenerateBranch(1, secondBranch);
        GenerateBranch(2, thirdBranch);

    }

    private void GenerateBranch(int firstGeneration, int branchIteration) {
      switch (firstGeneration) {
        case 0:
          // templates.firstSpawnerLeft.SetActive(true);
          // templates.firstSpawnerTop.SetActive(false);
          // templates.firstSpawnerRight.SetActive(false);
          break;
        case 1:
          // templates.firstSpawnerLeft.SetActive(false);
          // templates.firstSpawnerTop.SetActive(true);
          // templates.firstSpawnerRight.SetActive(false);
          break;
        case 2:
          // templates.firstSpawnerLeft.SetActive(false);
          // templates.firstSpawnerTop.SetActive(false);
          // templates.firstSpawnerRight.SetActive(true);
          break;
      }

      for (int i = 0; i < branchIteration; i++) {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        foreach (var spawnPoint in spawnPoints) {
          spawnPoint.GetComponent<RoomSpawner>().StartCoroutine("Spawn", (i == branchIteration - 1));
        }
      }
    }


// --------------------------------- Non Utilisé -------------------------------------------------

    // private void MakeRandomDirection(ref int x, ref int z, ref Vector2 addedPosition) {
    //     int rnd = Random.Range(0, 4);

    //     switch (rnd)
    //     {
    //         case 0:
    //             x += spacingValueX;
    //             break;

    //         case 1:
    //             z -= spacingValueZ;
    //             break;

    //         case 2:
    //             x -= spacingValueX;
    //             break;

    //         case 3:
    //             z += spacingValueZ;
    //             break;
    //     }

    //     addedPosition = new Vector2(x, z);
    //     if (!cubePositions.Contains(addedPosition))
    //         return;

    //     // If the position is already occupied, try a different direction
    //     MakeRandomDirection(ref x, ref z, ref addedPosition);
    // }


    // Doit être dans la fonction qui génère les salles

    // for (int i = 0; i < cubePositions.Count; i++) {
    //   var cube = cubePositions[i];
    //   Vector3 position = new Vector3(cube.x, 0f, cube.y);
    //   GameObject instantiatedObject = Instantiate(cubePrefab, position, Quaternion.identity);
      
    //   if (i == 0) {
    //     Renderer renderer = instantiatedObject.GetComponent<Renderer>();
    //     renderer.material.color = Color.magenta;
    //   }
    //   if (i == cubePositions.Count-4) {
    //     Renderer renderer = instantiatedObject.GetComponent<Renderer>();
    //     renderer.material.color = Color.blue;
    //   }
    //   if (i == cubePositions.Count-3) {
    //     Renderer renderer = instantiatedObject.GetComponent<Renderer>();
    //     renderer.material.color = Color.green;
    //   }
    //   if (i == cubePositions.Count-2) {
    //     Renderer renderer = instantiatedObject.GetComponent<Renderer>();
    //     renderer.material.color = Color.red;
    //   }
    //   if (i == cubePositions.Count-1) {
    //     Renderer renderer = instantiatedObject.GetComponent<Renderer>();
    //     renderer.material.color = Color.black;
    //   }
      
    //   instantiatedObject.SetActive(true);

    //   if(instantiatedObject.transform.position.x > spreadQuantity * spacingValueX
    //   || instantiatedObject.transform.position.x < -spreadQuantity * spacingValueX
    //   || instantiatedObject.transform.position.z > spreadQuantity * spacingValueZ
    //   || instantiatedObject.transform.position.z < -spreadQuantity * spacingValueZ) {
    //     GenerateRooms(firstBranch, secondBranch, thirdBranch);
    //   }
    // }

}
