using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    [Header("Weapon References")]
    public Transform weaponAttachPoint;   // Assign PlayerHand
    public GameObject stickPrefab;
    public GameObject rockPrefab;

    [Header("UI Elements")]
    public InventorySlot stickSlot;
    public InventorySlot rockSlot;

    private GameObject equippedWeapon;
    private string equippedWeaponName;
    private Dictionary<string, GameObject> weaponPrefabs;

    private void Start()
    {
        weaponPrefabs = new Dictionary<string, GameObject>
        {
            { "Stick", stickPrefab },
            { "Rock", rockPrefab }
        };

        stickSlot.HideIcon();
        rockSlot.HideIcon();
    }

    public void PickupWeapon(GameObject weaponPrefab, string weaponName, Sprite icon)
    {
        if (!weaponPrefabs.ContainsKey(weaponName)) return;

        // Prevent duplicate pickup of same weapon
        if (weaponName == "Stick" && stickSlot.hasWeapon) return;
        if (weaponName == "Rock" && rockSlot.hasWeapon) return;

        if (weaponName == "Stick")
        {
            stickSlot.AssignWeapon(weaponPrefab, weaponName, icon);
        }
        else if (weaponName == "Rock")
        {
            rockSlot.AssignWeapon(weaponPrefab, weaponName, icon);
        }
    }

    public void EquipWeapon(string weaponName)
    {
        if (!weaponPrefabs.ContainsKey(weaponName)) return;

        if (equippedWeapon != null)
        {
            DropWeapon(); // Drop current before equipping new one
        }

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

        // Hide the corresponding icon
        if (weaponName == "Stick") stickSlot.HideIcon();
        else if (weaponName == "Rock") rockSlot.HideIcon();
    }

    public void DropWeapon()
    {
        if (equippedWeapon == null) return;

        equippedWeapon.transform.SetParent(null);
        Rigidbody rb = equippedWeapon.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(weaponAttachPoint.forward * 5f + Vector3.up * 2f, ForceMode.Impulse);
        }

        foreach (Collider col in equippedWeapon.GetComponentsInChildren<Collider>())
            col.enabled = true;

        // Re-enable pickup on the dropped object
        WeaponPickupInteract interact = equippedWeapon.GetComponent<WeaponPickupInteract>();
        if (interact != null)
        {
            interact.SetPickedUp(false);
            equippedWeapon.SetActive(true);
        }

        string droppedWeaponName = equippedWeaponName;

        equippedWeapon = null;
        equippedWeaponName = null;

        // Reactivate the correct UI icon
        if (droppedWeaponName == "Stick") stickSlot.ShowIcon();
        else if (droppedWeaponName == "Rock") rockSlot.ShowIcon();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropWeapon();
        }
    }
}
