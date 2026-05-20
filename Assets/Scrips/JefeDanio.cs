using UnityEngine;

public class JefeDanio : MonoBehaviour
{
    public float danio = 25f;
    public float radioAtaque = 2.5f;
    public string tagJugador = "Player";

    private bool puedeGolpear = false;

    // Llamado por Animation Event al inicio del golpe
    public void ActivarHitbox()
    {
        puedeGolpear = true;
        AplicarDanio();
        Debug.Log("💀 JEFE: Hitbox activado");
    }

    // Llamado por Animation Event al final del golpe
    public void DesactivarHitbox()
    {
        puedeGolpear = false;
        Debug.Log("💀 JEFE: Hitbox desactivado");
    }

    // También llamado directamente desde JefeIA
    public void AplicarDanio()
    {
        Collider[] golpeados = Physics.OverlapSphere(transform.position, radioAtaque);

        foreach (Collider col in golpeados)
        {
            if (!col.CompareTag(tagJugador)) continue;

            PlayerStats jugador = col.GetComponent<PlayerStats>();
            if (jugador == null) jugador = col.GetComponentInParent<PlayerStats>();

            if (jugador != null)
            {
                Debug.Log("💥 JEFE golpea al jugador: " + danio + " daño");
                jugador.TakeDamage(danio);
                return;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, radioAtaque);
    }
}
