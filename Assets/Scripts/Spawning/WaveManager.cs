using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    public EnemySpawner spawner;
    public UIManager uiManager;

    public float[] waveDurations = { 30f, 60f, 90f }; // Example times
    public int[] enemiesPerWave = { 1, 3, 5 }; // Number of enemies per wave

    private int currentWave = 0;
    private int enemiesRemaining = 0;
    private float waveTimer = 0f;
    private bool waveInProgress = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        StartCoroutine(StartNextWave());
    }

    private void Update()
    {
        if (!waveInProgress) return;

        waveTimer -= Time.deltaTime;
        uiManager.UpdateTimer(waveTimer);

        if (waveTimer <= 0 && enemiesRemaining > 0)
        {
            Debug.Log("Time's up! Wave failed.");
            waveInProgress = false;
            // Handle wave failure logic here
        }
    }

    private IEnumerator StartNextWave()
    {
        yield return new WaitForSeconds(2f); // Small delay before next wave

        if (currentWave >= enemiesPerWave.Length)
        {
            Debug.Log("All waves completed!");
            yield break;
        }

        waveInProgress = true;
        waveTimer = waveDurations[Mathf.Min(currentWave, waveDurations.Length - 1)];
        enemiesRemaining = enemiesPerWave[currentWave];

        uiManager.UpdateWave(currentWave + 1);
        uiManager.UpdateEnemies(enemiesRemaining);
        uiManager.UpdateTimer(waveTimer);

        spawner.SpawnEnemies(enemiesRemaining);

        currentWave++;
    }

    public void OnEnemyKilled()
    {
        enemiesRemaining--;
        uiManager.UpdateEnemies(enemiesRemaining);

        if (enemiesRemaining <= 0 && waveInProgress)
        {
            waveInProgress = false;
            StartCoroutine(StartNextWave());
        }
    }
}
