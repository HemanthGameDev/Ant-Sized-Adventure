using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class CameraCollisionFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player;
    public Vector3 offset = new Vector3(0, 2.0f, -5.0f);

    [Header("Collision Settings")]
    public LayerMask collisionLayers;
    public float sphereCastRadius = 0.3f;
    public float minDistance = 1.0f;
    public float maxDistance = 10.0f;
    public float smoothing = 10f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 100f;
    public float mouseSensitivity = 2.0f;
    public float minVerticalAngle = -40f;
    public float maxVerticalAngle = 80f;

    private float currentYaw = 0f;
    private float currentPitch = 20f;
    private Vector3 currentVelocity;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void LateUpdate()
    {
        if (player == null) return;

        HandleRotationInput();
        Vector3 desiredCameraPosition = CalculateDesiredPosition();
        Vector3 correctedPosition = CollisionCorrectedPosition(desiredCameraPosition);

        transform.position = Vector3.SmoothDamp(transform.position, correctedPosition, ref currentVelocity, 1f / smoothing);
        transform.LookAt(player.position + Vector3.up * offset.y);
    }

    private void HandleRotationInput()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            currentYaw -= rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            currentYaw += rotationSpeed * Time.deltaTime;

        if (!IsPointerOverUI())
        {
            if (Input.GetMouseButton(0))
            {
                currentYaw += Input.GetAxis("Mouse X") * mouseSensitivity;
                currentPitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
                currentPitch = Mathf.Clamp(currentPitch, minVerticalAngle, maxVerticalAngle);
            }
        }
    }

    private Vector3 CalculateDesiredPosition()
    {
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        Vector3 direction = rotation * Vector3.back;
        return player.position + Vector3.up * offset.y + direction * offset.magnitude;
    }

    private Vector3 CollisionCorrectedPosition(Vector3 desiredPos)
    {
        Vector3 origin = player.position + Vector3.up * offset.y;

        if (Physics.Linecast(origin, desiredPos, out RaycastHit hit, collisionLayers))
        {
            Debug.Log("Blocked by: " + hit.collider.name);
            return hit.point + hit.normal * 0.3f;
        }

        return desiredPos;
    }


    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;
        return EventSystem.current.IsPointerOverGameObject();
    }
}
