using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class JefeIA : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 2.5f;
    public float distanciaDeteccion = 15f;
    public float distanciaAtaque = 2.2f;

    [Header("Ataque cuerpo a cuerpo")]
    public float tiempoEntreAtaques = 1.8f;

    [Header("Fase proyectiles")]
    public float vidaParaFaseProyectiles = 50f; // % de vida al que activa proyectiles
    public GameObject prefabProyectil;
    public Transform puntoDisparo;              // punto desde donde sale el proyectil
    public float tiempoEntreProyectiles = 2.5f;
    public int proyectilesEnRafaga = 3;         // cuántos lanza por ráfaga
    public float tiempoEntreRafaga = 0.3f;

    [Header("Referencias")]
    public JefeDanio jefeDanio;

    // Internos
    private Transform jugador;
    private CharacterController controller;
    private Animator animator;
    private EnemyHealth health;

    private float tiempoAtaque;
    private float tiempoProyectil;
    private bool faseProyectilesActiva = false;
    private bool estaAtacando = false;

    void Start()
    {
        controller  = GetComponent<CharacterController>();
        animator    = GetComponent<Animator>();
        health      = GetComponent<EnemyHealth>();
        jefeDanio   = GetComponent<JefeDanio>();

        var jugadorGO = GameObject.FindGameObjectWithTag("Player");
        if (jugadorGO != null) jugador = jugadorGO.transform;

        // Crear punto de disparo si no está asignado
        if (puntoDisparo == null)
        {
            var pd = new GameObject("PuntoDisparo");
            pd.transform.SetParent(transform);
            pd.transform.localPosition = new Vector3(0f, 1.5f, 0.8f);
            puntoDisparo = pd.transform;
        }
    }

    void Update()
    {
        if (jugador == null || health == null) return;
        if (health.vidaActual <= 0) return;

        // Activar fase proyectiles cuando baja de X% de vida
        float porcentajeVida = (health.vidaActual / health.maxVida) * 100f;
        if (!faseProyectilesActiva && porcentajeVida <= vidaParaFaseProyectiles)
        {
            faseProyectilesActiva = true;
            Debug.Log("💀 JEFE: Fase proyectiles activada");
        }

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia <= distanciaDeteccion)
        {
            Perseguir(distancia);

            // Proyectiles en paralelo cuando está en fase 2
            if (faseProyectilesActiva && prefabProyectil != null)
            {
                if (Time.time >= tiempoProyectil)
                {
                    tiempoProyectil = Time.time + tiempoEntreProyectiles;
                    StartCoroutine(LanzarRafaga());
                }
            }
        }
    }

    void Perseguir(float distancia)
    {
        Vector3 dir = (jugador.position - transform.position).normalized;
        dir.y = 0;
        transform.LookAt(new Vector3(jugador.position.x, transform.position.y, jugador.position.z));

        if (distancia > distanciaAtaque)
        {
            controller.Move(dir * velocidad * Time.deltaTime);
            if (animator != null) animator.SetFloat("Speed", 1f);
        }
        else
        {
            if (animator != null) animator.SetFloat("Speed", 0f);
            AtacarCuerpoACuerpo();
        }

        // Gravedad
        if (!controller.isGrounded)
            controller.Move(Vector3.down * 9.81f * Time.deltaTime);
    }

    void AtacarCuerpoACuerpo()
    {
        if (Time.time >= tiempoAtaque && !estaAtacando)
        {
            tiempoAtaque = Time.time + tiempoEntreAtaques;
            StartCoroutine(RutinaAtaque());
        }
    }

    IEnumerator RutinaAtaque()
    {
        estaAtacando = true;
        if (animator != null) animator.SetTrigger("Attack");

        // El daño lo aplican los Animation Events (ActivarHitbox/DesactivarHitbox)
        // Solo esperamos la duración del ataque antes de permitir el siguiente
        yield return new WaitForSeconds(tiempoEntreAtaques * 0.8f);
        estaAtacando = false;
    }

    IEnumerator LanzarRafaga()
    {
        for (int i = 0; i < proyectilesEnRafaga; i++)
        {
            LanzarProyectil();
            yield return new WaitForSeconds(tiempoEntreRafaga);
        }
    }

    void LanzarProyectil()
    {
        if (jugador == null || prefabProyectil == null) return;

        Vector3 origen = puntoDisparo != null ? puntoDisparo.position : transform.position + Vector3.up * 1.5f;
        Vector3 direccion = (jugador.position + Vector3.up * 1f - origen).normalized;

        GameObject p = Instantiate(prefabProyectil, origen, Quaternion.LookRotation(direccion));
        var proyectil = p.GetComponent<Proyectil>();
        if (proyectil != null) proyectil.direccion = direccion;

        Debug.Log("🔮 JEFE lanza proyectil");
    }
}
