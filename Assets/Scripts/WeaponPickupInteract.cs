using UnityEngine;

public class WeaponPickupInteract : MonoBehaviour
{
    public string weaponName;        // Must be "Stick" or "Rock"
    public Sprite weaponIcon;
    public GameObject weaponPrefab;// UI icon

    private bool isPlayerNearby = false;
    private GameObject player;
   
    public bool IsPickedUp { get; private set; }

    public void SetPickedUp(bool pickedUp)
    {
        IsPickedUp = pickedUp;
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !IsPickedUp)
        {
            InventorySystem inventory = player.GetComponent<InventorySystem>();
            if (inventory != null)
            {
                inventory.PickupWeapon(gameObject, weaponName, weaponIcon);
                IsPickedUp = true;
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            player = null;
        }
    }
}
