using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    public float danioLigero = 10f;
    public float danioPesado = 25f;
    public string tagEnemigo = "Enemy";
    public float radioAtaque = 2.5f;

    private float danioActual;

    // Llamado desde PlayerControler al iniciar el ataque
    // Usa OverlapSphere para detectar enemigos en el momento exacto,
    // sin depender de movimiento físico (evita el bug de OnTriggerEnter)
    void AplicarDanio()
    {
        // Busca todos los colliders dentro del radio de ataque frente al jugador
        Collider[] golpeados = Physics.OverlapSphere(transform.position, radioAtaque);

        bool golpeoAlguien = false;

        foreach (Collider col in golpeados)
        {
            if (!col.CompareTag(tagEnemigo) && !col.transform.root.CompareTag(tagEnemigo))
                continue;

            EnemyHealth enemigo = col.GetComponentInParent<EnemyHealth>();
            if (enemigo == null)
                enemigo = col.GetComponent<EnemyHealth>();
            if (enemigo == null)
                continue;

            // Solo golpear enemigos que estén aproximadamente enfrente
            Vector3 dirAlEnemigo = (enemigo.transform.position - transform.position).normalized;
            float angulo = Vector3.Angle(transform.forward, dirAlEnemigo);

            if (angulo > 100f) // 100 grados de arco de ataque (50 a cada lado)
                continue;

            Debug.Log("🔥 GOLPE REAL al enemigo: " + enemigo.name + " daño: " + danioActual);
            enemigo.RecibirDanio(danioActual);
            golpeoAlguien = true;
        }

        if (!golpeoAlguien)
            Debug.Log("PlayerDamage: No se encontró enemigo en rango");
    }

    public void ActivarDanioLigero()
    {
        danioActual = danioLigero;
        Debug.Log("PlayerDamage: Activar daño ligero");
        AplicarDanio();
    }

    public void ActivarDanoPesado()
    {
        danioActual = danioPesado;
        Debug.Log("PlayerDamage: Activar daño pesado");
        AplicarDanio();
    }

    // Mantenido por compatibilidad con PlayerControler
    public void DesactivarDanio()
    {
        Debug.Log("PlayerDamage: Desactivar daño");
    }

    // Dibuja el radio de ataque en el editor para facilitar el ajuste
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioAtaque);
    }
}