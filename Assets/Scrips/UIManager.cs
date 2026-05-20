using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public PlayerStats player;
    public Slider healthBar;   // no se usa, vida manejada por HUDVida
    public Slider staminaBar;
    public Image playerIcon;

    void Start()
    {
        if (player == null)
            player = FindObjectOfType<PlayerStats>();

        BuscarStaminaBar();

        if (staminaBar != null && player != null)
        {
            staminaBar.minValue = 0;
            staminaBar.maxValue = player.maxStamina;
            staminaBar.value    = player.maxStamina;
        }
    }

    void BuscarStaminaBar()
    {
        if (staminaBar != null) return;

        // Buscar en HUDPanel primero
        var hudPanel = GameObject.Find("HUDPanel");
        if (hudPanel != null)
        {
            var barraGO = hudPanel.transform.Find("StaminaBar");
            if (barraGO != null)
                staminaBar = barraGO.GetComponent<Slider>();
        }

        // Fallback por nombre
        if (staminaBar == null)
        {
            var go = GameObject.Find("StaminaBar");
            if (go != null) staminaBar = go.GetComponent<Slider>();
        }
    }

    void Update()
    {
        if (player == null) return;

        // Buscar barra si se perdio la referencia
        if (staminaBar == null) BuscarStaminaBar();

        if (staminaBar != null)
        {
            // unscaledDeltaTime para que la barra se actualice aunque el juego este pausado
            staminaBar.value = Mathf.Lerp(
                staminaBar.value,
                player.currentStamina,
                Time.unscaledDeltaTime * 8f
            );
        }
    }
}
