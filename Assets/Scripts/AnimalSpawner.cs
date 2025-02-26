using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    public GameObject animalPrefab;  // Assign the animal prefab in the inspector
    public BoxCollider triggerBox;   // Use BoxCollider for more accurate bounds
    public float spawnHeight = 1.0f;  // Height at which animals will be spawned

    private bool playerInside = false;  // Flag to check if the player is inside the trigger box

    private void Start()
    {
        StartCoroutine(SpawnAnimalPeriodically());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;  // Set flag to true when player enters the trigger box
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;  // Reset flag when player exits the trigger box
        }
    }

    private IEnumerator SpawnAnimalPeriodically()
    {
        while (true)
        {
            if (playerInside)
            {
                Debug.Log("Spawning animal...");
                SpawnAnimal();
            }
            yield return new WaitForSeconds(1f); // Corrected to actually wait for 10 seconds
        }
    }

    private void SpawnAnimal()
    {
        Vector3 boxCenter = triggerBox.bounds.center;
        Vector3 boxSize = triggerBox.bounds.size;
        Vector3 spawnPosition = new Vector3(
            Random.Range(boxCenter.x - boxSize.x / 2, boxCenter.x + boxSize.x / 2),
            spawnHeight,
            Random.Range(boxCenter.z - boxSize.z / 2, boxCenter.z + boxSize.z / 2)
        );

        GameObject newAnimal = Instantiate(animalPrefab, spawnPosition, Quaternion.identity);
        Debug.Log($"Spawned animal at: {spawnPosition}");
    }
}