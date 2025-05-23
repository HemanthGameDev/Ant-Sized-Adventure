using UnityEngine;

public class CameraFollowKeyboard : MonoBehaviour
{
    public Transform player; // Assign your player in the Inspector
    public float distance = 5.0f;
    public float height = 2.0f;
    public float rotationSpeed = 100.0f;
    public float zoomSpeed = 2.0f;
    public float minDistance = 2.0f;
    public float maxDistance = 10.0f;

    public float minAngle = -90f; // Minimum rotation angle in degrees
    public float maxAngle = 90f;  // Maximum rotation angle in degrees

    private float currentAngle = 0.0f;

    void LateUpdate()
    {
        if (player == null) return;

        // Rotate with left/right keys (or A/D)
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            currentAngle -= rotationSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            currentAngle += rotationSpeed * Time.deltaTime;
        }

        // Clamp the angle to avoid full 360 rotation
        currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);

        // Zoom with up/down keys (or W/S)
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            distance -= zoomSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            distance += zoomSpeed * Time.deltaTime;
        }

        // Clamp distance
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        // Calculate position
        Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
        Vector3 offset = rotation * new Vector3(0, height, -distance);
        Vector3 targetPosition = player.position + offset;

        // Smooth move
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);

        // Always look at the player (a bit above center)
        transform.LookAt(player.position + Vector3.up * height * 0.5f);
    }
}
