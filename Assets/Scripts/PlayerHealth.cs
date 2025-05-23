using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int MaxHealth = 100;
    public int CurrentHealth { get; private set; }

    private PlayerController3D playerController;

    void Start()
    {
        CurrentHealth = MaxHealth;
        playerController = GetComponent<PlayerController3D>();
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        Debug.Log($"Player took damage: {damage}, Health: {CurrentHealth}");

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died!");
        if (playerController != null)
        {
            playerController.TriggerDeath();
        }
    }
}
