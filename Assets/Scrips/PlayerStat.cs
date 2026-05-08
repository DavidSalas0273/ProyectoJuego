using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Vida")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider barraVida; // 👈 NUEVO

    [Header("Estamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float regenStamina = 5f;

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        if (barraVida != null)
        {
            barraVida.maxValue = maxHealth;
            barraVida.value = currentHealth;
        }
    }

    void Update()
    {
        RegenerarStamina();
    }

    void RegenerarStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += regenStamina * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
    }

    public void RecibirDanio(float cantidad)
    {
        currentHealth -= cantidad;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Vida actual: " + currentHealth);

        if (barraVida != null)
        {
            barraVida.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Morir();
        }
    }

    void Morir()
    {
        Debug.Log("💀 Jugador muerto");
    }

    public bool UsarStamina(float cantidad)
    {
        if (currentStamina >= cantidad)
        {
            currentStamina -= cantidad;
            return true;
        }

        return false;
    }
}