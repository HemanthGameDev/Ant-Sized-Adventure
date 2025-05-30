using UnityEngine;
using UnityEngine.UI;

public class AutoAimController : MonoBehaviour
{
    [Header("Auto Aim Settings")]
    [Tooltip("Smoothness of rotation towards the target.")]
    public float aimSmoothSpeed = 5f;

    [Tooltip("Minimum distance to avoid jittery aiming on close targets.")]
    public float minTargetDistance = 2f;

    [Tooltip("Maximum range to detect enemies.")]
    public float aimRange = 20f;

    [Tooltip("Layer mask for enemy detection.")]
    public LayerMask enemyLayer;

    [Tooltip("UI icon to show over target.")]
    public Image aimIcon;

    private Transform currentTarget;
    private float minTargetDistanceSqr;
    private Camera mainCamera;

    void Start()
    {
        minTargetDistanceSqr = minTargetDistance * minTargetDistance;
        mainCamera = Camera.main;

        if (aimIcon != null)
        {
            aimIcon.enabled = false;
        }
    }

    void Update()
    {
        FindTarget();
        AimAtTarget();
        UpdateAimIcon();
    }

    private void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, aimRange, enemyLayer);
        float closestDistanceSqr = Mathf.Infinity;
        Transform bestTarget = null;

        foreach (Collider hit in hits)
        {
            Vector3 direction = hit.transform.position - transform.position;
            float distanceSqr = direction.sqrMagnitude;

            if (distanceSqr < closestDistanceSqr && distanceSqr > minTargetDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                bestTarget = hit.transform;
            }
        }

        currentTarget = bestTarget;
    }

    private void AimAtTarget()
    {
        if (currentTarget == null) return;

        Vector3 direction = currentTarget.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f) return;

        // Only aim if target is roughly in front
        float dot = Vector3.Dot(transform.forward, direction.normalized);
        if (dot < -0.5f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * aimSmoothSpeed);
    }

    private void UpdateAimIcon()
    {
        if (aimIcon == null || mainCamera == null) return;

        if (currentTarget != null)
        {
            aimIcon.enabled = true;
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(currentTarget.position);
            aimIcon.transform.position = screenPosition;
        }
        else
        {
            aimIcon.enabled = false;
        }
    }

    // Editor helper to visualize range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aimRange);
    }
}
