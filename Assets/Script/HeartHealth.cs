using UnityEngine;
using UnityEngine.UI;

public class HeartHealth : MonoBehaviour
{
    [Header("Hearts UI")]
    [SerializeField] private Image[] hearts; 

    [Header("Player Health")]
    [SerializeField] private Health playerHealth; 

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHearts;
        }

        UpdateHearts();
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHearts;
        }
    }

    public void UpdateHearts()
    {
        if (playerHealth == null) return;

        float healthPerHeart = playerHealth.MaxHealth / hearts.Length;

        for (int i = 0; i < hearts.Length; i++)
        {
            // Shows heart if current health is greater than threshold for this heart
            hearts[i].enabled = playerHealth.currentHealth > i * healthPerHeart;
        }
    }
}
