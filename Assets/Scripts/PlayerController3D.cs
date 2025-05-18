using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController3D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float rotationSpeed = 10f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private Animator animator;
    private Vector3 moveDirection;
    private bool isGrounded;
    private bool isJumpPressed;

    private string currentAnimation;

    private const string ANIM_IDLE = "Idle";
    private const string ANIM_RUN = "Run";
    private const string ANIM_JUMP = "Jumping";

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleInput();
        HandleAnimation();
    }

    void FixedUpdate()
    {
        CheckGround();
        Move();
        if (isJumpPressed && isGrounded)
        {
            Jump();
        }
    }

    void HandleInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector3(h, 0f, v).normalized;

        isJumpPressed = Input.GetKeyDown(KeyCode.Space);
    }

    void Move()
    {
        Vector3 velocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
        rb.linearVelocity = velocity;

        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        isGrounded = false; // Prevents re-jumping mid-air due to delay in raycast
    }

    void HandleAnimation()
    {
        if (!isGrounded)
        {
            ChangeAnimation(ANIM_JUMP);
        }
        else if (moveDirection.magnitude > 0.1f)
        {
            ChangeAnimation(ANIM_RUN);
        }
        else
        {
            ChangeAnimation(ANIM_IDLE);
        }
    }

    void ChangeAnimation(string newAnim)
    {
        if (currentAnimation == newAnim) return;
        animator.CrossFade(newAnim, 0.1f); // Smooth transition over 0.1s
        currentAnimation = newAnim;
    }


    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // Optional: debug ground check in scene view
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
