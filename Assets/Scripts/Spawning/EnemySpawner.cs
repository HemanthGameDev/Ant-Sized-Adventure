using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;
    public int maxSpawnAttempts = 100;

    [Header("Spawn Area Settings")]
    public float spawnRadius = 30f; // adjustable in Inspector
    public float groundCheckHeight = 100f;
    public float groundRayLength = 200f;
    public float navMeshSampleRadius = 3f;
    public LayerMask groundMask;
    public LayerMask obstacleMask;

    [Header("Debug")]
    public bool showSpawnGizmos = true;

    public void SpawnEnemies(int count)
    {
        int spawned = 0;
        int attempts = 0;

        while (spawned < count && attempts < maxSpawnAttempts)
        {
            Vector3 candidate = GetRandomSpawnPosition();

            if (IsValidSpawnPoint(candidate))
            {
                Instantiate(enemyPrefab, candidate, Quaternion.identity);
                spawned++;
            }

            attempts++;
        }

        if (spawned < count)
        {
            Debug.LogWarning($"Only spawned {spawned}/{count} enemies after {attempts} attempts.");
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 topRayPos = new Vector3(transform.position.x + randomCircle.x, groundCheckHeight, transform.position.z + randomCircle.y);
        return topRayPos;
    }

    private bool IsValidSpawnPoint(Vector3 fromAbove)
    {
        // Raycast down to hit ground
        if (Physics.Raycast(fromAbove, Vector3.down, out RaycastHit hit, groundRayLength, groundMask))
        {
            Vector3 point = hit.point;

            // Avoid obstacles
            Collider[] nearbyObstacles = Physics.OverlapSphere(point, 1.5f, obstacleMask);
            if (nearbyObstacles.Length > 0)
            {
                Debug.Log("Spawn blocked by obstacle.");
                return false;
            }

            // Check NavMesh
            if (NavMesh.SamplePosition(point, out NavMeshHit navHit, navMeshSampleRadius, NavMesh.AllAreas))
            {
                return true;
            }
            else
            {
                Debug.Log("Spawn point not on NavMesh.");
            }
        }
        else
        {
            Debug.Log("No ground detected below spawn point.");
        }

        return false;
    }


    private void OnDrawGizmosSelected()
    {
        if (!showSpawnGizmos) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
