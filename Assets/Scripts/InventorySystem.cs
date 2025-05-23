using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    [Header("Weapon References")]
    public Transform weaponAttachPoint;
    public GameObject stickPrefab;
    public GameObject rockPrefab;

    [Header("UI Elements")]
    public InventorySlot stickSlot;
    public InventorySlot rockSlot;

    [Header("Throw Settings")]
    public float throwForce = 5f;
    public float throwUpwardForce = 2f;
    public float weaponDespawnTime = 10f;
    public float throwAnimationDuration = 0.35f; // should match actual animation

    [Header("Animation")]
    public Animator playerAnimator; // assign Player Animator in Inspector

    private GameObject equippedWeapon;
    private string equippedWeaponName;

    private Dictionary<string, GameObject> weaponPrefabs;
    private Dictionary<string, InventorySlot> weaponSlots;

    private void Start()
    {
        weaponPrefabs = new Dictionary<string, GameObject>
        {
            { "Stick", stickPrefab },
            { "Rock", rockPrefab }
        };

        weaponSlots = new Dictionary<string, InventorySlot>
        {
            { "Stick", stickSlot },
            { "Rock", rockSlot }
        };

        foreach (var slot in weaponSlots.Values)
            slot.HideIcon();
    }

    public void PickupWeapon(GameObject weaponPrefab, string weaponName, Sprite icon)
    {
        if (!weaponPrefabs.ContainsKey(weaponName) || !weaponSlots.ContainsKey(weaponName)) return;

        InventorySlot slot = weaponSlots[weaponName];
        if (slot.hasWeapon) return;

        slot.AssignWeapon(weaponPrefab, weaponName, icon);
    }

    public void EquipWeapon(string weaponName)
    {
        if (!weaponPrefabs.ContainsKey(weaponName) || !weaponSlots.ContainsKey(weaponName)) return;

        if (equippedWeapon != null)
        {
            DropWeaponImmediate(); // quick swap (no throw)
        }

        StartCoroutine(EquipWeaponCoroutine(weaponName));
    }

    private IEnumerator EquipWeaponCoroutine(string weaponName)
    {
        yield return new WaitForSeconds(0.35f); // wait for equip animation if any

        GameObject prefab = weaponPrefabs[weaponName];
        GameObject newWeapon = Instantiate(prefab, weaponAttachPoint);
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;

        Rigidbody rb = newWeapon.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        foreach (Collider col in newWeapon.GetComponentsInChildren<Collider>())
            col.enabled = false;

        equippedWeapon = newWeapon;
        equippedWeaponName = weaponName;

        weaponSlots[weaponName].HideIcon();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (equippedWeapon != null)
                StartCoroutine(ThrowWeaponAfterAnimation());
        }
    }

    /// <summary>
    /// Plays animation, waits, then throws the weapon.
    /// </summary>
    private IEnumerator ThrowWeaponAfterAnimation()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Attack");
        }

        // Temporarily disable rendering until throw
        Renderer[] renderers = equippedWeapon.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
            r.enabled = false;

        yield return new WaitForSeconds(throwAnimationDuration); // wait for animation

        // Detach and throw weapon
        equippedWeapon.transform.SetParent(null);

        Rigidbody rb = equippedWeapon.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(weaponAttachPoint.forward * throwForce + Vector3.up * throwUpwardForce, ForceMode.Impulse);
        }

        foreach (Collider col in equippedWeapon.GetComponentsInChildren<Collider>())
            col.enabled = true;

        // Reactivate rendering
        foreach (var r in renderers)
            r.enabled = true;

        // Reactivate pickup logic
        WeaponPickupInteract interact = equippedWeapon.GetComponent<WeaponPickupInteract>();
        if (interact != null)
        {
            interact.SetPickedUp(false);
            equippedWeapon.SetActive(true);
        }

        // Schedule destruction
        Destroy(equippedWeapon, weaponDespawnTime);

        // Show correct icon again
        if (!string.IsNullOrEmpty(equippedWeaponName) && weaponSlots.ContainsKey(equippedWeaponName))
            weaponSlots[equippedWeaponName].ShowIcon();

        equippedWeapon = null;
        equippedWeaponName = null;
    }

    /// <summary>
    /// Immediate drop without animation.
    /// </summary>
    public void DropWeaponImmediate()
    {
        if (equippedWeapon == null) return;

        equippedWeapon.transform.SetParent(null);

        Rigidbody rb = equippedWeapon.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(weaponAttachPoint.forward * throwForce + Vector3.up * throwUpwardForce, ForceMode.Impulse);
        }

        foreach (Collider col in equippedWeapon.GetComponentsInChildren<Collider>())
            col.enabled = true;

        WeaponPickupInteract interact = equippedWeapon.GetComponent<WeaponPickupInteract>();
        if (interact != null)
        {
            interact.SetPickedUp(false);
            equippedWeapon.SetActive(true);
        }

        Destroy(equippedWeapon, weaponDespawnTime);

        if (!string.IsNullOrEmpty(equippedWeaponName) && weaponSlots.ContainsKey(equippedWeaponName))
            weaponSlots[equippedWeaponName].ShowIcon();

        equippedWeapon = null;
        equippedWeaponName = null;
    }

    public GameObject GetEquippedWeaponObject() => equippedWeapon;

    public void ClearEquippedSlot()
    {
        if (!string.IsNullOrEmpty(equippedWeaponName) && weaponSlots.ContainsKey(equippedWeaponName))
            weaponSlots[equippedWeaponName].ShowIcon();

        equippedWeapon = null;
        equippedWeaponName = null;
    }
}