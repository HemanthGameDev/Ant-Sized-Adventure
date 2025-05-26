using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int MaxHealth = 100;
    public int CurrentHealth { get; private set; }

    private PlayerController3D playerController;
    private bool isDead = false;

    void Start()
    {
        CurrentHealth = MaxHealth;
        playerController = GetComponent<PlayerController3D>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

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
        if (isDead) return;
        isDead = true;

        Debug.Log("Player died!");

        // Trigger death animation or ragdoll
        if (playerController != null)
        {
            playerController.TriggerDeath();
        }

        // Show Game Over UI
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowGameOver();
        }
    }
}
