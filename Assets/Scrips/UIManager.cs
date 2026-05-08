using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public PlayerStats player;

    public Slider healthBar;
    public Slider staminaBar;

    void Start()
    {
        // 🔥 IMPORTANTE: configurar sliders
        healthBar.minValue = 0;
        healthBar.maxValue = player.maxHealth;

        staminaBar.minValue = 0;
        staminaBar.maxValue = player.maxStamina;
    }

    void Update()
    {
        healthBar.value = Mathf.Lerp(healthBar.value, player.currentHealth, Time.deltaTime * 5f);
        staminaBar.value = Mathf.Lerp(staminaBar.value, player.currentStamina, Time.deltaTime * 5f);
    }
}