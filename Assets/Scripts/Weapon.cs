using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Inventory/Weapon")]
public class Weapon : ScriptableObject
{
    [Header("Basic Info")]
    public string weaponName;
    public Sprite icon;
    public GameObject weaponPrefab;
    public WeaponType weaponType;

    [Header("Hand Positioning (Local to PlayerHand)")]
    public Vector3 handLocalPosition = Vector3.zero;
    public Vector3 handLocalEulerRotation = Vector3.zero;

    public enum WeaponType
    {
        Stick,
        Rock
    }

    // Get hand rotation as Quaternion
    public Quaternion GetHandLocalRotation()
    {
        return Quaternion.Euler(handLocalEulerRotation);
    }
}