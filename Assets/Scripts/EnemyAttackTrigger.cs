using UnityEngine;

public class EnemyAttackTrigger : MonoBehaviour
{
    public int damage = 2;
    public float attackCooldown = 1f;

    private float lastAttackTime;
    private EnemyAI enemyAI;

    private void Start()
    {
        enemyAI = GetComponentInParent<EnemyAI>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Time.time >= lastAttackTime + attackCooldown)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                lastAttackTime = Time.time;
            }
        }
    }
}
