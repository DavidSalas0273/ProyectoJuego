using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    public float danioLigero = 10f;
    public float danioPesado = 25f;

    private bool puedeHacerDanio = false;
    private float danioActual;
    private bool yaGolpeo = false;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("HITBOX TOCÓ: " + other.name);

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("🔥 GOLPE REAL");

            EnemyHealth enemigo = other.GetComponent<EnemyHealth>();

            if (enemigo != null)
            {
                enemigo.RecibirDanio(10f);
            }
        }
    }

    // 🔥 ANIMACIÓN
    public void ActivarDanioLigero()
    {
        danioActual = danioLigero;
        puedeHacerDanio = true;
        yaGolpeo = false;
    }

    public void ActivarDanoPesado()
    {
        danioActual = danioPesado;
        puedeHacerDanio = true;
        yaGolpeo = false;
    }

    public void DesactivarDanio()
    {
        puedeHacerDanio = false;
    }
}