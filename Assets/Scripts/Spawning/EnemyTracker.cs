using UnityEngine;

public class EnemyTracker : MonoBehaviour
{
    private EnemySpawner spawner;
    private int spawnPointIndex;
    private bool isInitialized = false;
    private bool isDead = false;

    public void Init(EnemySpawner spawnerRef, int index)
    {
        spawner = spawnerRef;
        spawnPointIndex = index;
        isInitialized = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isInitialized || isDead) return;

        if (other.CompareTag("PlayerWeapon"))
        {
            isDead = true;
            spawner?.NotifyEnemyDied(spawnPointIndex);
            WaveManager.Instance?.OnEnemyKilled();
            gameObject.SetActive(false);
        }
    }
}
