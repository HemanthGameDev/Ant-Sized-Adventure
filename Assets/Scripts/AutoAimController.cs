using UnityEngine;
using UnityEngine.UI;

public class AutoAimController : MonoBehaviour
{
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
            if (distance < closestDistance)
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

        Vector3 direction = (currentTarget.position - transform.position).normalized;
        direction.y = 0f; // keep the aim horizontal
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
}
