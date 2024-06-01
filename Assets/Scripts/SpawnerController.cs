using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    public int initialZombiesPerWave = 5;
    public float spawnDelay = 0.5f;

    public List<Enemy> currentZombiesAlive;

    public GameObject zombiePrefab;

    private bool hasSpawned = false; // This flag ensures spawning occurs only once

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasSpawned)
        {
            hasSpawned = true; // Set the flag to true to prevent further spawning
            StartCoroutine(SpawnInitialZombies());
        }
    }

    private IEnumerator SpawnInitialZombies()
    {
        for (int i = 0; i < initialZombiesPerWave; i++)
        {
            Vector3 spawnOffSet = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
            Vector3 spawnPosition = transform.position + spawnOffSet;

            var zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);

            Enemy enemyScript = zombie.GetComponent<Enemy>();

            currentZombiesAlive.Add(enemyScript);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void Start()
    {
        currentZombiesAlive = new List<Enemy>();
    }
}
