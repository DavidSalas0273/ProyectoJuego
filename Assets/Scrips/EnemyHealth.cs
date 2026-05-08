using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxVida = 100f;
    public float vidaActual;

    public Slider barraVida;
    public GameObject barraUI;

    void Start()
    {
        vidaActual = maxVida;

        barraVida.maxValue = maxVida;
        barraVida.value = vidaActual;

        barraUI.SetActive(false);
    }

    public void RecibirDanio(float dano)
    {
        vidaActual -= dano;

        Debug.Log("💀 Vida enemigo: " + vidaActual);
        Debug.Log("Se activó barra");

        barraUI.SetActive(true);
        barraVida.value = vidaActual;

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    void Morir()
    {
        Debug.Log("☠️ Enemigo muerto");
        Destroy(gameObject);
    }
}