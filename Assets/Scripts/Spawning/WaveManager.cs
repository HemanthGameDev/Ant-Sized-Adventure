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
    private int totalEnemiesThisWave = 0;

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
        totalEnemiesThisWave = enemiesRemainingToSpawn;
        enemiesAlive = 0;

        uiManager.UpdateWave(currentWave + 1);
        uiManager.UpdateEnemies(enemiesAlive, totalEnemiesThisWave);

        int initialSpawnCount = Mathf.Min(enemiesRemainingToSpawn, spawnPoints.Length);
        for (int i = 0; i < initialSpawnCount; i++)
        {
            SpawnEnemyAt(spawnPoints[i]);
        }
    }

    void SpawnEnemyAt(Transform spawnPoint)
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.waveManager = this;
        }

        enemiesRemainingToSpawn--;
        enemiesAlive++;
        uiManager.UpdateEnemies(enemiesAlive, totalEnemiesThisWave);
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;
        uiManager.UpdateEnemies(enemiesAlive, totalEnemiesThisWave);

        if (enemiesRemainingToSpawn > 0)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            SpawnEnemyAt(spawnPoint);
        }
        else if (enemiesAlive <= 0)
        {
            uiManager.ShowWaveCleared();
        }

    }
    public void ContinueToNextWave()
    {
        uiManager.HideWaveCleared();
        currentWave++;
        StartWave();
    }


    IEnumerator StartNextWaveDelayed()
    {
        yield return new WaitForSeconds(2f);
        StartWave();
    }
}
