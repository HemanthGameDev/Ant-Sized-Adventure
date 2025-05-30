using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float patrolSpeed = 1.5f;
    public float chargeSpeed = 4f;
    public float rotationSpeed = 5f;
    public float directionChangeInterval = 3f;
    public float obstacleDetectDistance = 1.5f;

    [Header("Detection & Direction")]
    public Transform aggroRadius;
    public Transform attackTrigger;
    public Transform directionPointer;

    [Header("Death Settings")]
    public float bounceForce = 5f;
    public float destroyDelay = 0.5f;

    [Header("Managers")]
    public WaveManager waveManager;

    private Transform player;
    private Vector3 patrolDirection;
    private float directionChangeTimer;
    private bool isChasing = false;
    private bool isDead = false;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        patrolDirection = transform.forward;

        if (waveManager == null && WaveManager.Instance != null)
            waveManager = WaveManager.Instance;
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

        // Change direction after interval or if obstacle is detected
        if (directionChangeTimer >= directionChangeInterval || DetectObstacleAhead())
        {
            directionChangeTimer = 0f;
            float angle = Random.Range(90f, 270f); // wider angle to ensure decent direction changes
            patrolDirection = Quaternion.Euler(0f, angle, 0f) * transform.forward;
        }

        MoveInDirection(patrolDirection, patrolSpeed);
    }

    bool DetectObstacleAhead()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, transform.forward);
        return Physics.Raycast(ray, obstacleDetectDistance, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore);
    }

    void ChargeTowardsPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        MoveInDirection(direction, chargeSpeed);
    }

    void MoveInDirection(Vector3 direction, float speed)
    {
        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        Vector3 movement = transform.forward * speed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);
    }

    public void SetChasePlayer(Transform target)
    {
        if (target == null) return;
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
        }
        else if (!collision.collider.isTrigger &&
                 !collision.collider.CompareTag("Ground") &&
                 !collision.collider.CompareTag("Player"))
        {
            float angle = Random.Range(120f, 240f);
            patrolDirection = Quaternion.Euler(0f, angle, 0f) * transform.forward;

            if (rb != null)
            {
                Vector3 bounce = -collision.GetContact(0).normal * bounceForce;
                rb.AddForce(bounce, ForceMode.Impulse);
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
        waveManager?.OnEnemyKilled();
        ScoreManager.Instance?.AddScore(1);

        Destroy(gameObject, destroyDelay);
    }

    public void Die()
    {
        Kill();
    }
}
