using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [Header("UI")]
    public Image icon;
    public Button equipButton;
    public string weaponType; // Set in inspector: "Stick" or "Rock"
    public GameObject weaponPrefab;
    public bool hasWeapon;
    private GameObject equippedWeapon;

    private InventorySystem inventorySystem;

    private void Awake()
    {
        inventorySystem = FindFirstObjectByType<InventorySystem>();
        equipButton.onClick.AddListener(OnClick);
        HideIcon();
    }

    public void SetWeaponData(Sprite sprite, string type)
    {
        weaponType = type;
        icon.sprite = sprite;
        icon.enabled = true;
        icon.gameObject.SetActive(true);
        equipButton.interactable = true;
    }

    public void OnClick()
    {
        if (inventorySystem != null)
        {
            inventorySystem.EquipWeapon(weaponType);
            HideIcon();
        }
    }

    public void ShowIcon()
    {
        icon.enabled = true;
        equipButton.interactable = true;
    }

    public void HideIcon()
    {
        icon.enabled = false;
        equipButton.interactable = false;
    }
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

}
