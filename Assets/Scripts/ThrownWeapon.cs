using UnityEngine;

public class ThrownWeapon : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Optional: Add damage logic here
            Destroy(gameObject); // Destroy on impact with enemy
        }
    }
}
