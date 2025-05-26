using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("Wave Settings")]
    public int[] enemiesPerWave; // How many enemies to spawn per wave
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    [Header("UI")]
    public UIManager uiManager;

    private int currentWave = 0;
    private int enemiesRemainingToSpawn = 0;
    private int enemiesAlive = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        StartWave();
    }

    void StartWave()
    {
        if (currentWave >= enemiesPerWave.Length)
        {
            Debug.Log("All waves complete!");
            return;
        }

        enemiesRemainingToSpawn = enemiesPerWave[currentWave];
        enemiesAlive = 0;

        uiManager.UpdateWave(currentWave + 1);

        // Spawn as many as possible initially (limited by spawn points)
        int initialSpawnCount = Mathf.Min(enemiesRemainingToSpawn, spawnPoints.Length);
        for (int i = 0; i < initialSpawnCount; i++)
        {
            SpawnEnemyAt(spawnPoints[i]);
        }
        

    }

    void SpawnEnemyAt(Transform spawnPoint)
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        // Set waveManager reference if EnemyAI is used
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.waveManager = this;
        }

        enemiesRemainingToSpawn--;
        enemiesAlive++;
        uiManager.UpdateEnemies(enemiesAlive, enemiesRemainingToSpawn + enemiesAlive);

    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;
        uiManager.UpdateEnemies(enemiesAlive, enemiesRemainingToSpawn + enemiesAlive);


        if (enemiesRemainingToSpawn > 0)
        {
            // Spawn from any random spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            SpawnEnemyAt(spawnPoint);
        }
        else if (enemiesAlive <= 0)
        {
            currentWave++;
            StartCoroutine(StartNextWaveDelayed());
        }
    }

    IEnumerator StartNextWaveDelayed()
    {
        yield return new WaitForSeconds(2f); // small delay before next wave
        StartWave();
    }
}
