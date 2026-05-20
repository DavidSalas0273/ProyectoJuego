using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour
{
    [Header("Destino")]
    public Transform destinoTeleport; // Punto donde aparecerá el jugador
    public Teleporter teleporterDestino; // Referencia al otro teletransportador para vincularlos

    [Header("Configuración")]
    public float delayTeletransporte = 0.5f;
    public bool conservarRotacion = true;
    public string tagJugador = "Player";

    [Header("Efectos Visuales")]
    public bool usarPartículas = true;
    public ParticleSystem particulasTeleport;
    public Color colorActivacion = Color.cyan;
    public float duracionCambioColor = 0.5f;

    [Header("Componentes")]
    public CharacterController characterController;
    public Rigidbody rigidbody;

    private Renderer rendererPortal;
    private Color colorOriginal;
    private bool enTeletransporte = false;
    private Collider triggerCollider;

    void Start()
    {
        // Obtener componente renderer para el efecto visual
        rendererPortal = GetComponent<Renderer>();
        if (rendererPortal != null)
        {
            colorOriginal = rendererPortal.material.color;
        }

        // Asegurar que el trigger está configurado correctamente
        triggerCollider = GetComponent<Collider>();
        if (triggerCollider != null && !triggerCollider.isTrigger)
        {
            triggerCollider.isTrigger = true;
        }

        // Crear partículas si no hay ninguna asignada
        if (usarPartículas && particulasTeleport == null)
        {
            CrearPartículas();
        }
    }

    private void CrearPartículas()
    {
        GameObject particulasGO = new GameObject("Particulas_Teleport");
        particulasGO.transform.SetParent(transform);
        particulasGO.transform.localPosition = Vector3.zero;

        particulasTeleport = particulasGO.AddComponent<ParticleSystem>();
        particulasTeleport.Stop();

        // Configurar sistema de partículas básico
        var mainModule = particulasTeleport.main;
        mainModule.duration = 0.5f;
        mainModule.loop = false;
        mainModule.startLifetime = 0.5f;
        mainModule.startSize = 0.2f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enTeletransporte)
            return;

        // Detectar si es el jugador
        if (!other.CompareTag(tagJugador) && !other.transform.root.CompareTag(tagJugador))
            return;

        // Validar que hay destino
        if (destinoTeleport == null && teleporterDestino == null)
        {
            Debug.LogWarning("Teleporter: No hay destino configurado.");
            return;
        }

        // Iniciar teletransporte
        Transform destino = destinoTeleport != null ? destinoTeleport : teleporterDestino.transform;
        StartCoroutine(EjecutarTeletransporte(other.gameObject, destino));
    }

    private IEnumerator EjecutarTeletransporte(GameObject jugador, Transform puntoDestino)
    {
        enTeletransporte = true;

        // Efecto visual al activarse
        EfectoActivacion();

        // Esperar un poco antes de teletransportar
        yield return new WaitForSeconds(delayTeletransporte);

        // Obtener componentes del jugador
        CharacterController cc = jugador.GetComponent<CharacterController>();
        Rigidbody rb = jugador.GetComponent<Rigidbody>();

        // Guardar rotación si es necesario
        Quaternion rotacionOriginal = conservarRotacion ? jugador.transform.rotation : puntoDestino.rotation;

        // Desactivar temporalmente el CharacterController para mover sin colisiones
        if (cc != null)
        {
            cc.enabled = false;
        }

        // Desactivar Rigidbody si existe
        bool rbEstabaActivo = false;
        if (rb != null)
        {
            rbEstabaActivo = !rb.isKinematic;
            rb.isKinematic = true;
        }

        // Mover el jugador al destino
        jugador.transform.position = puntoDestino.position;
        jugador.transform.rotation = rotacionOriginal;

        // Reactivar CharacterController
        if (cc != null)
        {
            cc.enabled = true;
        }

        // Reactivar Rigidbody
        if (rb != null && rbEstabaActivo)
        {
            rb.isKinematic = false;
        }

        // Efecto visual en el destino
        Teleporter teleporterDestino = puntoDestino.GetComponent<Teleporter>();
        if (teleporterDestino != null)
        {
            teleporterDestino.EfectoActivacion();
        }

        // Evitar teletransportes múltiples
        yield return new WaitForSeconds(1f);
        enTeletransporte = false;

        Debug.Log("Jugador teletransportado a: " + puntoDestino.name);
    }

    private void EfectoActivacion()
    {
        // Efecto de color
        if (rendererPortal != null)
        {
            StartCoroutine(CambiarColor());
        }

        // Efecto de partículas
        if (usarPartículas && particulasTeleport != null)
        {
            particulasTeleport.Play();
        }
    }

    private IEnumerator CambiarColor()
    {
        Material material = rendererPortal.material;
        Color colorInicial = colorOriginal;
        float tiempo = 0f;

        // Cambiar a color de activación
        while (tiempo < duracionCambioColor)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionCambioColor;
            material.color = Color.Lerp(colorInicial, colorActivacion, t);
            yield return null;
        }

        // Volver al color original
        tiempo = 0f;
        while (tiempo < duracionCambioColor)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionCambioColor;
            material.color = Color.Lerp(colorActivacion, colorInicial, t);
            yield return null;
        }

        material.color = colorOriginal;
    }

    // Método para vincular dos teletransportadores automáticamente
    public void VincularConOtroTeleportador(Teleporter otro)
    {
        this.teleporterDestino = otro;
        if (otro != null && otro.teleporterDestino == null)
        {
            otro.teleporterDestino = this;
        }
    }

    // Resetear estado si es necesario
    public void ResetearTeleporter()
    {
        enTeletransporte = false;
        if (rendererPortal != null)
        {
            rendererPortal.material.color = colorOriginal;
        }
    }

    // Dibujar gizmos en el editor para visualizar
    private void OnDrawGizmos()
    {
        // Dibujar el área del teletransportador
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, transform.localScale);

        // Dibujar línea hacia el destino
        if (destinoTeleport != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, destinoTeleport.position);
            Gizmos.DrawWireSphere(destinoTeleport.position, 0.5f);
        }
        else if (teleporterDestino != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, teleporterDestino.transform.position);
            Gizmos.DrawWireSphere(teleporterDestino.transform.position, 0.5f);
        }
    }
}
