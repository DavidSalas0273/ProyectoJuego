using UnityEngine;

// Poner este script en cada cubo de spawn (Inicio Villa, Inicio dungeon, Inicio jefe)
// Cuando el jugador entra al trigger, este punto se convierte en su spawn activo
public class CheckpointZona : MonoBehaviour
{
    [Header("Configuracion")]
    public string nombreZona = "Zona";

    void Start()
    {
        // Asegurarse que el collider es trigger
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        // Hacer el cubo invisible en el juego (solo es logico)
        var renderer = GetComponent<MeshRenderer>();
        if (renderer != null) renderer.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Registrar este punto como spawn activo en el GameManager
        if (GameManager.instancia != null)
        {
            GameManager.instancia.SetSpawnPoint(transform.position, nombreZona);
            Debug.Log("Checkpoint activado: " + nombreZona + " en " + transform.position);
        }
    }

    // Dibujar el area en el editor para verla
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0.5f, 0.3f);
        Gizmos.DrawCube(transform.position, transform.localScale);
        Gizmos.color = new Color(0f, 1f, 0.5f, 0.8f);
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
