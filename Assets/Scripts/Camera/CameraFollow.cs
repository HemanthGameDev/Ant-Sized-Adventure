using UnityEngine;
using UnityEngine.EventSystems;

public class CameraFollowCollision : MonoBehaviour
{
    public Transform player;
    public float distance = 5.0f;
    public float height = 2.0f;
    public float mouseSensitivity = 2.0f;
    public float zoomSpeed = 2.0f;
    public float minDistance = 2.0f;
    public float maxDistance = 10.0f;
    public float minPitch = -40f;
    public float maxPitch = 80f;

    public LayerMask collisionLayers;
    public float collisionBuffer = 0.3f;

    private float yaw = 0f;
    private float pitch = 20f;
    private Vector3 lastMousePos;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (player != null)
        {
            Vector3 offset = transform.position - player.position;
            yaw = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
            pitch = Mathf.Asin(offset.y / offset.magnitude) * Mathf.Rad2Deg;
        }
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
        if (!IsPointerOverUI() && Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            yaw += delta.x * mouseSensitivity * 0.1f;
            pitch -= delta.y * mouseSensitivity * 0.1f;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        lastMousePos = Input.mousePosition;
    }

    private void HandleZoomInput()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }
    }

    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPos = player.position - rotation * Vector3.forward * distance + Vector3.up * height;
        Vector3 correctedPos = CollisionCorrectedPosition(desiredPos);

        transform.position = correctedPos;
        transform.LookAt(player.position + Vector3.up * height * 0.5f);
    }

    private Vector3 CollisionCorrectedPosition(Vector3 desiredPos)
    {
        Vector3 origin = player.position + Vector3.up * height;

        if (Physics.Linecast(origin, desiredPos, out RaycastHit hit, collisionLayers))
        {
            return hit.point + hit.normal * collisionBuffer;
        }

        return desiredPos;
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;
        return EventSystem.current.IsPointerOverGameObject();
    }
}
