using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float patrolSpeed = 1.5f;
    public float chargeSpeed = 4f;
    public float rotationSpeed = 5f;

    [Header("References")]
    public Transform aggroRadius;
    public Transform attackTrigger;
    public Transform directionPointer;

    [Header("Death Settings")]
    public float bounceForce = 5f;
    public float destroyDelay = 0.5f;

    [Header("Wave System")]
    public WaveManager waveManager;

    private Transform player;
    private bool isChasing = false;
    private Vector3 patrolDirection;
    private float directionChangeTimer;
    private float directionChangeInterval = 3f;
    private bool isDead = false;

    private Rigidbody rb;

    void Start()
    {
        patrolDirection = transform.forward;
        rb = GetComponent<Rigidbody>();

        // Assign WaveManager automatically if not set in prefab
        if (waveManager == null && WaveManager.Instance != null)
        {
            waveManager = WaveManager.Instance;
        }
    }

    void Update()
    {
        if (isDead) return;

        if (isChasing && player != null)
        {
            ChargeTowardsPlayer();
        }
        else
        {
            Patrol();
        }

        if (directionPointer != null)
        {
            directionPointer.position = transform.position + transform.forward;
        }
    }

    void Patrol()
    {
        directionChangeTimer += Time.deltaTime;
        if (directionChangeTimer >= directionChangeInterval)
        {
            directionChangeTimer = 0f;
            float angle = Random.Range(0f, 360f);
            patrolDirection = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
        }

        MoveInDirection(patrolDirection.normalized, patrolSpeed);
    }

    void ChargeTowardsPlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        MoveInDirection(dir, chargeSpeed);
    }

    void MoveInDirection(Vector3 dir, float speed)
    {
        if (dir == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        transform.position += transform.forward * speed * Time.deltaTime;
    }

    public void SetChasePlayer(Transform target)
    {
        isChasing = true;
        player = target;
    }

    public void StopChase()
    {
        isChasing = false;
        player = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;

        if (collision.collider.CompareTag("Weapon"))
        {
            Kill();
            return;
        }
        else if (!collision.collider.isTrigger && !collision.collider.CompareTag("Ground") && !collision.collider.CompareTag("Player"))
        {
            float randomAngle = Random.Range(120f, 240f);
            patrolDirection = Quaternion.Euler(0, randomAngle, 0) * transform.forward;

            if (rb != null)
            {
                Vector3 bounceBack = -collision.GetContact(0).normal * 0.5f;
                rb.AddForce(bounceBack, ForceMode.Impulse);
            }

            StartCoroutine(PauseMovement(0.2f));
        }
    }

    private System.Collections.IEnumerator PauseMovement(float duration)
    {
        float originalSpeed = patrolSpeed;
        patrolSpeed = 0f;
        yield return new WaitForSeconds(duration);
        patrolSpeed = originalSpeed;
    }

    public void Kill()
    {
        if (isDead) return;

        isDead = true;

        if (waveManager != null)
        {
            waveManager.OnEnemyKilled();
        }

        // Optionally add death animation or bounce effect here before destroy
        Destroy(gameObject, destroyDelay);
    }
}
