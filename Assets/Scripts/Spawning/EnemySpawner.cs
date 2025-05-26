using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    private bool[] isOccupied;
    private Queue<int> spawnQueue = new Queue<int>();
    private int totalEnemiesToSpawn;
    private int spawnedEnemies;

    private void Awake()
    {
        isOccupied = new bool[spawnPoints.Length];
    }

    public void BeginSpawningWave(int count)
    {
        totalEnemiesToSpawn = count;
        spawnedEnemies = 0;
        spawnQueue.Clear();

        for (int i = 0; i < count; i++)
        {
            spawnQueue.Enqueue(i);
        }

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (spawnedEnemies < totalEnemiesToSpawn)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (!isOccupied[i] && spawnQueue.Count > 0)
                {
                    SpawnAt(i);
                    yield return new WaitForSeconds(0.2f);
                }
            }
            yield return null;
        }
    }

    private void SpawnAt(int index)
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoints[index].position, Quaternion.identity);
        isOccupied[index] = true;
        spawnedEnemies++;
        spawnQueue.Dequeue();

        EnemyTracker tracker = enemy.GetComponent<EnemyTracker>();
        if (tracker != null)
        {
            tracker.Init(this, index);
        }

        Debug.Log($"Spawned enemy at spawn point {index}");
    }

    public void NotifyEnemyDied(int spawnIndex)
    {
        if (spawnIndex >= 0 && spawnIndex < isOccupied.Length)
        {
            isOccupied[spawnIndex] = false;
            Debug.Log($"Spawn point {spawnIndex} is now free");
        }
    }
}
