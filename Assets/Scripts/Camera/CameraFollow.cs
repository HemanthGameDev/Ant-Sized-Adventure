using UnityEngine;
using UnityEngine.EventSystems; // Needed for UI detection

public class CameraFollowKeyboard : MonoBehaviour
{
    public Transform player;
    public float distance = 5.0f;
    public float height = 2.0f;
    public float rotationSpeed = 100.0f;
    public float mouseSensitivity = 2.0f;
    public float zoomSpeed = 2.0f;
    public float minDistance = 2.0f;
    public float maxDistance = 10.0f;

    public float minAngle = -90f;
    public float maxAngle = 90f;

    private float currentAngle = 0.0f;
    private Vector3 lastMousePosition;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void LateUpdate()
    {
        if (player == null) return;

        HandleRotationInput();
        HandleZoomInput();
        UpdateCameraPosition();
    }

    private void HandleRotationInput()
    {
        // Keyboard rotation
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            currentAngle -= rotationSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            currentAngle += rotationSpeed * Time.deltaTime;
        }

        // Touchpad/mouse move rotation — only if not clicking UI
        if (!IsPointerOverUI())
        {
            if (Input.GetMouseButton(0) || Input.touchCount == 1) // Basic safeguard
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;
                currentAngle += delta.x * mouseSensitivity * 0.02f; // Scale factor for sensitivity
            }
        }

        // Clamp rotation
        currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);

        lastMousePosition = Input.mousePosition;
    }

    private void HandleZoomInput()
    {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            distance -= zoomSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            distance += zoomSpeed * Time.deltaTime;
        }

        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
        Vector3 offset = rotation * new Vector3(0, height, -distance);
        Vector3 targetPosition = player.position + offset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
        transform.LookAt(player.position + Vector3.up * height * 0.5f);
    }

    // Prevent camera control when touching UI (like weapon icons)
    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;
        return EventSystem.current.IsPointerOverGameObject();
    }
}
