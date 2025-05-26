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

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        StartWave();
    }

    private void StartWave()
    {
        if (currentWave >= enemiesPerWave.Length)
        {
            Debug.Log("All waves complete!");
            uiManager.ShowVictory(); // ✅ Show Victory if no more waves
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

    private void SpawnEnemyAt(Transform spawnPoint)
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

        // ✅ Add score per kill
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(10); // Award 10 points per enemy
        }

        if (enemiesRemainingToSpawn > 0)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            SpawnEnemyAt(spawnPoint);
        }
        else if (enemiesAlive <= 0)
        {
            if (currentWave >= enemiesPerWave.Length - 1)
            {
                Debug.Log("Victory! All waves defeated.");
                uiManager.ShowVictory(); // ✅ Show victory UI
            }
            else
            {
                uiManager.ShowWaveCleared(); // ✅ Continue button for next wave
            }
        }
    }

    public void ContinueToNextWave()
    {
        uiManager.HideWaveCleared();
        currentWave++;
        StartWave();
    }

    private IEnumerator StartNextWaveDelayed()
    {
        yield return new WaitForSeconds(2f);
        StartWave();
    }
}
