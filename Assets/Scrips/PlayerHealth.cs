    using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 4;
    public int currentHealth;

    [Header("UI Elements")]
    public Image[] hearts; // Array of heart images representing health
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public Image playerIcon; // Small circular icon on HUD
    public GameObject deathScreen; // Black screen with death message

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHearts();
        deathScreen.SetActive(false); // Hide death screen at start
    }

    // Method to reduce health when player takes damage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        UpdateHearts();

        if (currentHealth == 0)
        {
            Die();
        }
    }

    // Update the heart images to match current health
    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }

    // Handle player death
    void Die()
    {
        deathScreen.SetActive(true);
        deathScreen.GetComponentInChildren<Text>().text = "El personaje ha muerto";
        // Stop time to freeze the game
        Time.timeScale = 0f;
        // Optional: Disable player controls here if needed
    }
}