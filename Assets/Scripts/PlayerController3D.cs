using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController3D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float rotationSpeed = 10f;

    [Header("Inventory System Reference")]
    public InventorySystem inventorySystem;

    [Header("Weapon Placement")]
    public Transform weaponSpawnPoint;

    private Rigidbody rb;
    private Animator animator;
    private Vector3 moveDirection;
    private bool isJumpPressed;
    private bool isPickPressed;
    private bool wasGroundedLastFrame;
    private float lastYVelocity;

    private string currentAnimation;
    private bool jumpLocked;

    private GroundCheckTrigger groundTrigger;
    private FrontCheckTrigger frontTrigger;

    private const string ANIM_IDLE = "Idle";
    private const string ANIM_RUN = "Run";
    private const string ANIM_JUMP = "Jumping";
    private const string ANIM_UP_JUMP = "Up_Jump";
    private const string ANIM_DOWN_JUMP = "Down_Jump";
    private const string ANIM_PICKING = "Picking";

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        groundTrigger = GetComponentInChildren<GroundCheckTrigger>();
        frontTrigger = GetComponentInChildren<FrontCheckTrigger>();

        if (inventorySystem == null)
            inventorySystem = GetComponent<InventorySystem>();
    }

    private void Update()
    {
        HandleInput();
        HandleAnimation();

        if ((currentAnimation == ANIM_UP_JUMP || currentAnimation == ANIM_DOWN_JUMP) && jumpLocked)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName(currentAnimation) && state.normalizedTime >= 1f)
            {
                jumpLocked = false;
                HandleAnimation();
            }
        }

        if (isPickPressed && groundTrigger.isGrounded && !jumpLocked)
        {
            PlayPickingAnimation();
        }

        // 🔴 Weapon Drop (ALWAYS CALLABLE)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            inventorySystem?.DropWeapon();
        }
    }

    private void FixedUpdate()
    {
        if (!jumpLocked)
            Move();

        if (isJumpPressed && groundTrigger.isGrounded && !jumpLocked)
        {
            if (frontTrigger.hasObstacle)
            {
                Jump(ANIM_UP_JUMP);
                jumpLocked = true;
            }
            else
            {
                Jump(ANIM_JUMP);
            }
        }

        lastYVelocity = rb.linearVelocity.y;
        wasGroundedLastFrame = groundTrigger.isGrounded;
    }

    private void HandleInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector3(h, 0f, v).normalized;

        isJumpPressed = Input.GetKeyDown(KeyCode.Space);
        isPickPressed = Input.GetKeyDown(KeyCode.E);
    }

    private void Move()
    {
        Vector3 velocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
        rb.linearVelocity = velocity;

        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
        }
    }

    private void Jump(string animName)
    {
        ChangeAnimation(animName);
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
    }

    private void PlayPickingAnimation()
    {
        ChangeAnimation(ANIM_PICKING);
        jumpLocked = true;
        rb.linearVelocity = Vector3.zero;
        StartCoroutine(WaitForPickingAnimationToEnd());
    }

    private IEnumerator WaitForPickingAnimationToEnd()
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(ANIM_PICKING));
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        TryEquipNearbyWeapon();
        jumpLocked = false;
        HandleAnimation();
    }

    private void TryEquipNearbyWeapon()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 2f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Weapon"))
            {
                WeaponPickupInteract pickup = hit.GetComponent<WeaponPickupInteract>();
                if (pickup != null && !pickup.IsPickedUp)
                {
                    // Assuming the method to mark the weapon as picked up is 'SetPickedUp'
                    pickup.SetPickedUp(true);
                    pickup.gameObject.SetActive(false);
                    inventorySystem.PickupWeapon(pickup.gameObject, pickup.weaponName, pickup.weaponIcon);
                    
                    break;
                }
            }
        }
    }


    private void PlaceWeaponInFront(GameObject weapon)
    {
        if (weaponSpawnPoint == null)
        {
            Debug.LogWarning("[PlayerController3D] weaponSpawnPoint is not assigned!");
            return;
        }

        weapon.transform.position = weaponSpawnPoint.position;
        weapon.transform.rotation = weaponSpawnPoint.rotation;
        weapon.transform.SetParent(null);
        weapon.SetActive(true);

        Rigidbody weaponRb = weapon.GetComponent<Rigidbody>();
        if (weaponRb != null)
        {
            weaponRb.linearVelocity = Vector3.zero;
            weaponRb.angularVelocity = Vector3.zero;
            weaponRb.isKinematic = true;
        }

        WeaponPickupInteract weaponPickup = weapon.GetComponent<WeaponPickupInteract>();
        if (weaponPickup != null)
        {
            weaponPickup.SetPickedUp(true);
        }
    }


    private void HandleAnimation()
    {
        if (jumpLocked) return;

        if (!groundTrigger.isGrounded && lastYVelocity < -0.5f && wasGroundedLastFrame)
        {
            ChangeAnimation(ANIM_DOWN_JUMP);
            jumpLocked = true;
            return;
        }

        if (!groundTrigger.isGrounded)
        {
            if (currentAnimation != ANIM_JUMP &&
                currentAnimation != ANIM_UP_JUMP &&
                currentAnimation != ANIM_DOWN_JUMP)
            {
                ChangeAnimation(ANIM_JUMP);
            }
        }
        else
        {
            if (moveDirection.magnitude > 0.1f)
                ChangeAnimation(ANIM_RUN);
            else
                ChangeAnimation(ANIM_IDLE);
        }
    }

    private void ChangeAnimation(string newAnim, int layer = 0, float crossFadeTime = 0.1f)
    {
        if (currentAnimation == newAnim || animator == null) return;

        if (layer < 0 || layer >= animator.layerCount)
        {
            Debug.LogError($"[PlayerController3D] Animator layer index {layer} is out of range.");
            return;
        }

        int hash = Animator.StringToHash(newAnim);

        if (animator.HasState(layer, hash))
        {
            animator.CrossFade(hash, crossFadeTime, layer);
            currentAnimation = newAnim;
        }
        else
        {
            Debug.LogError($"[PlayerController3D] Animator state '{newAnim}' not found in layer {layer}.");
        }
    }
}
