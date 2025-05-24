using UnityEngine;

public class EnemyAggroTrigger : MonoBehaviour
{
    private EnemyAI enemyAI;

    private void Start()
    {
        enemyAI = GetComponentInParent<EnemyAI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyAI?.SetChasePlayer(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyAI?.StopChase();
        }
    }
}
