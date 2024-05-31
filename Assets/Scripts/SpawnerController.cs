using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class SpawnerController : MonoBehaviour
{
    public int initialZombiesPerWave = 5;
    public int CurrentZombiesPerWave;

    public float spawnDelay = 0.5f;

    public int currentWave = 0;
    public float waveCooldown = 10.0f; //Time between waves

    public bool inCoolDown;
    public float coolDownCounter = 0; //Testing

    public List<Enemy> currentZombiesAlive;

    public GameObject zombiePrefab;

    private void Start()
    {
        CurrentZombiesPerWave = initialZombiesPerWave;
        StartNextWave();
    }

    private void StartNextWave()
    {
        currentZombiesAlive.Clear();
        currentWave++;
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < CurrentZombiesPerWave; i++)
        {
            Vector3 spawnOffSet = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
            Vector3 spawnPosition = transform.position + spawnOffSet;

            var zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
            
            Enemy enemyScript = zombie.GetComponent<Enemy>();

            currentZombiesAlive.Add(enemyScript);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void Update()
    {
        List<Enemy> zombiestoRemove = new List<Enemy>();
        foreach (Enemy Zombie in currentZombiesAlive)
        {
            
        }
    }
}

