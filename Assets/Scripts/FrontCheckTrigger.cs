using UnityEngine;

public class FrontCheckTrigger : MonoBehaviour
{
    public bool hasObstacle { get; private set; }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            hasObstacle = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            hasObstacle = false;
        }
    }
}
