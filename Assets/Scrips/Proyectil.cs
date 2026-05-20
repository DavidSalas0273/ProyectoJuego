using UnityEngine;

public class Proyectil : MonoBehaviour
{
    public Vector3 direccion;
    public float velocidad = 12f;
    public float danio = 20f;
    public float tiempoVida = 5f;
    public string tagJugador = "Player";

    void Start()
    {
        Destroy(gameObject, tiempoVida);
    }

    void Update()
    {
        transform.position += direccion * velocidad * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagJugador))
        {
            PlayerStats jugador = other.GetComponent<PlayerStats>();
            if (jugador == null) jugador = other.GetComponentInParent<PlayerStats>();

            if (jugador != null)
            {
                Debug.Log("🔮 Proyectil impacta al jugador: " + danio + " daño");
                jugador.TakeDamage(danio);
            }

            Destroy(gameObject);
        }
        else if (!other.isTrigger && !other.CompareTag("Enemy"))
        {
            // Destruir al chocar con cualquier cosa que no sea trigger ni enemigo
            Destroy(gameObject);
        }
    }
}
