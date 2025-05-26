using UnityEngine;
using UnityEngine.UI;

public class AutoAimController : MonoBehaviour
{
    [Header("Smoothing & Distance")]
    public float aimSmoothSpeed = 5f;
    public float minTargetDistance = 2f; // avoid jitter on too-close targets
    public float aimRange = 20f;
    public LayerMask enemyLayer;
    public Image aimIcon; // UI icon to display when a target is acquired

    private Transform currentTarget;

    void Update()
    {
        FindTarget();
        UpdateAimIcon();
        AimAtTarget();
    }

    void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, aimRange, enemyLayer);
        float closestDistance = Mathf.Infinity;
        Transform bestTarget = null;

        foreach (Collider hit in hits)
        {
            float distance = Vector3.Distance(transform.position, hit.transform.position);
            if (distance < closestDistance && distance > minTargetDistance)
            {
                closestDistance = distance;
                bestTarget = hit.transform;
            }
        }

        currentTarget = bestTarget;
    }


    void UpdateAimIcon()
    {
        if (currentTarget != null)
        {
            aimIcon.enabled = true;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(currentTarget.position);
            aimIcon.transform.position = screenPos;
        }
        else
        {
            aimIcon.enabled = false;
        }
    }

    void AimAtTarget()
    {
        if (currentTarget == null) return;

        Vector3 toTarget = currentTarget.position - transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude < 0.01f) return; // too close to rotate

        // Prevent flipping: only aim if target is generally in front
        float dot = Vector3.Dot(transform.forward, toTarget.normalized);
        if (dot < -0.5f) return; // enemy is too far behind; ignore

        Quaternion lookRotation = Quaternion.LookRotation(toTarget.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aimRange);
    }

}
