using UnityEngine;
using System.Collections;

public class EnemyDamage : MonoBehaviour
{
    public float danio = 15f;
    public string tagJugador = "Player";
    public float radioAtaque = 2f;
    public float cooldownGolpe = 1f;

    private bool puedeGolpear = true;

    // Llamado desde el Animation Event "ActivarHitbox" del esqueleto
    public void ActivarHitbox()
    {
        Debug.Log("🟥 ENEMIGO ATACA");
        if (puedeGolpear)
            AplicarDanio();
    }

    // Mantenido por compatibilidad con Animation Events existentes
    public void DesactivarHitbox()
    {
        Debug.Log("🟥 ENEMIGO DEJA DE ATACAR");
    }

    void AplicarDanio()
    {
        // OverlapSphere desde la posición del enemigo — no depende de movimiento
        Collider[] golpeados = Physics.OverlapSphere(transform.position, radioAtaque);

        foreach (Collider col in golpeados)
        {
            if (!col.CompareTag(tagJugador))
                continue;

            PlayerStats jugador = col.GetComponent<PlayerStats>();
            if (jugador == null)
                jugador = col.GetComponentInParent<PlayerStats>();

            if (jugador != null)
            {
                Debug.Log("💥 GOLPE AL JUGADOR: " + danio + " daño");
                jugador.TakeDamage(danio);
                StartCoroutine(CooldownGolpe());
                return; // un golpe por ataque
            }
        }

        Debug.Log("🟥 Enemigo atacó pero el jugador no está en rango (" + radioAtaque + "u)");
    }

    private IEnumerator CooldownGolpe()
    {
        puedeGolpear = false;
        yield return new WaitForSeconds(cooldownGolpe);
        puedeGolpear = true;
    }
}