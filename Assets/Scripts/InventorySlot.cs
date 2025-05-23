using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [Header("UI Components")]
    public Image icon;
    public Button equipButton;

    [Header("Weapon Data")]
    public string weaponType; // e.g., "Stick" or "Rock"
    public GameObject weaponPrefab;
    public bool hasWeapon;

    private InventorySystem inventorySystem;

    private void Awake()
    {
        inventorySystem = FindFirstObjectByType<InventorySystem>();
        if (equipButton != null)
            equipButton.onClick.AddListener(OnClick);

        HideIcon();
    }

    /// <summary>
    /// Called when this inventory slot's button is clicked.
    /// Equips the corresponding weapon via InventorySystem.
    /// </summary>
    public void OnClick()
    {
        if (inventorySystem != null && hasWeapon)
        {
            inventorySystem.EquipWeapon(weaponType);
            HideIcon(); // Hide the icon once the weapon is equipped
        }
    }

    /// <summary>
    /// Assigns weapon data to the slot and updates the icon.
    /// </summary>
    /// <param name="prefab">Weapon prefab</param>
    /// <param name="weaponName">Weapon type identifier</param>
    /// <param name="weaponSprite">UI icon</param>
    public void AssignWeapon(GameObject prefab, string weaponName, Sprite weaponSprite)
    {
        weaponPrefab = prefab;
        weaponType = weaponName;
        hasWeapon = true;

        icon.sprite = weaponSprite;
        icon.enabled = true;
        icon.gameObject.SetActive(true);
        equipButton.interactable = true;
    }

    /// <summary>
    /// Directly sets sprite and type without assigning prefab.
    /// </summary>
    public void SetWeaponData(Sprite sprite, string type)
    {
        weaponType = type;
        icon.sprite = sprite;
        icon.enabled = true;
        icon.gameObject.SetActive(true);
        equipButton.interactable = true;
        hasWeapon = true;
    }

    /// <summary>
    /// Shows the icon and enables the equip button.
    /// </summary>
    public void ShowIcon()
    {
        icon.enabled = true;
        icon.gameObject.SetActive(true);
        equipButton.interactable = true;
    }

    /// <summary>
    /// Hides the icon and disables the equip button.
    /// </summary>
    public void HideIcon()
    {
        icon.enabled = false;
        icon.gameObject.SetActive(false);
        equipButton.interactable = false;
    }
}