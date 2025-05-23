using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public PlayerHealth playerHealth; // Reference to your player health script
    public Image foregroundImage;

    void Update()
    {
        if (playerHealth != null && foregroundImage != null)
        {
            float fillAmount = (float)playerHealth.CurrentHealth / playerHealth.MaxHealth;
            foregroundImage.fillAmount = Mathf.Clamp01(fillAmount);
        }
    }
}
