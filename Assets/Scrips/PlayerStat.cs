using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Vida")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider barraVida;

    [Header("UI Elements")]
    public GameObject deathScreen;

    [Header("Estamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float regenStamina = 5f;

    private bool muerto = false;

    void Start()
    {
        muerto = false;
        currentHealth  = maxHealth;
        currentStamina = maxStamina;

        // Buscar referencias automaticamente si no estan asignadas
        BuscarReferencias();

        if (barraVida != null)
        {
            barraVida.maxValue = maxHealth;
            barraVida.value    = currentHealth;
        }

        if (deathScreen != null)
            deathScreen.SetActive(false);
    }

    void BuscarReferencias()
    {
        // Buscar HUDVida si no hay barraVida asignada
        if (barraVida == null)
        {
            // El nuevo HUD no usa slider, buscar por nombre legacy
            var sliders = GameObject.FindObjectsOfType<Slider>();
            foreach (var s in sliders)
            {
                if (s.gameObject.name.ToLower().Contains("vida") ||
                    s.gameObject.name.ToLower().Contains("health") ||
                    s.gameObject.name.ToLower().Contains("hp"))
                {
                    barraVida = s;
                    break;
                }
            }
        }

        // Buscar deathScreen si no esta asignado
        if (deathScreen == null)
        {
            var canvas = GameObject.Find("Canvas");
            if (canvas != null)
                foreach (Transform hijo in canvas.transform)
                    if (hijo.name == "GameOverPanel")
                    { deathScreen = hijo.gameObject; break; }

            if (deathScreen == null)
            {
                var go = GameObject.Find("GameOverPanel");
                if (go != null) deathScreen = go;
            }
        }

        Debug.Log("PlayerStats referencias: barraVida=" + (barraVida != null ? barraVida.name : "NULL (usando HUDVida)")
                  + " | deathScreen=" + (deathScreen != null ? deathScreen.name : "NULL"));
    }

    void Update()
    {
        RegenerarStamina();
    }

    void RegenerarStamina()
    {
        if (currentStamina < maxStamina)
        {
            // unscaledDeltaTime para que regenere aunque Time.timeScale sea 0
            currentStamina += regenStamina * Time.unscaledDeltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
    }

    public void TakeDamage(float damage)
    {
        if (muerto) return;

        currentHealth -= damage;
        currentHealth  = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Vida actual: " + currentHealth + " / " + maxHealth);

        // Actualizar slider — buscar referencia si se perdio
        if (barraVida == null) BuscarReferencias();
        if (barraVida != null)
            barraVida.value = currentHealth;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (muerto) return;
        muerto = true;

        Debug.Log("PlayerStats.Die() activado");

        // Buscar deathScreen si se perdio la referencia
        if (deathScreen == null) BuscarReferencias();

        if (deathScreen != null)
            deathScreen.SetActive(true);

        if (GameManager.instancia != null)
            GameManager.instancia.ActivarGameOver(transform.position);
        else
            Time.timeScale = 0f;
    }

    // Llamado por GameManager al respawnear
    public void Respawnear()
    {
        muerto         = false;
        currentHealth  = maxHealth;
        currentStamina = maxStamina;

        if (barraVida != null)
            barraVida.value = maxHealth;
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
