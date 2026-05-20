using UnityEngine;

/// <summary>
/// Círculo rojo en el suelo con partículas que cura al jugador al pisarlo.
/// Se coloca automáticamente en la escena. Tiene cooldown para no curar infinito.
/// </summary>
public class ZonaCuracion : MonoBehaviour
{
    [Header("Curación")]
    public float cantidadCuracion = 20f;   // vida que restaura por activación
    public float cooldown         = 5f;    // segundos entre curaciones

    [Header("Visual")]
    public float radioCirculo     = 1.5f;  // radio del trigger

    private float tiempoUltimaCuracion = -999f;
    private ParticleSystem particulas;

    void Awake()
    {
        // Collider trigger
        SphereCollider col = gameObject.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius    = radioCirculo;

        // Sistema de partículas
        particulas = gameObject.AddComponent<ParticleSystem>();
        ConfigurarParticulas();
    }

    void ConfigurarParticulas()
    {
        // Módulo principal
        var main = particulas.main;
        main.loop            = true;
        main.startLifetime   = 1.5f;
        main.startSpeed      = 1.5f;
        main.startSize       = 0.15f;
        main.startColor      = new Color(1f, 0.1f, 0.1f, 0.9f); // rojo
        main.maxParticles    = 80;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        // Emisión
        var emission = particulas.emission;
        emission.rateOverTime = 30f;

        // Forma: disco en el suelo
        var shape = particulas.shape;
        shape.enabled    = true;
        shape.shapeType  = ParticleSystemShapeType.Circle;
        shape.radius     = radioCirculo;
        shape.rotation   = new Vector3(-90f, 0f, 0f); // apunta hacia arriba

        // Velocidad sobre tiempo (sube y desaparece)
        var vel = particulas.velocityOverLifetime;
        vel.enabled = true;
        vel.space   = ParticleSystemSimulationSpace.Local;
        vel.y       = new ParticleSystem.MinMaxCurve(1.0f);

        // Tamaño sobre tiempo (se encoge al final)
        var size = particulas.sizeOverLifetime;
        size.enabled = true;
        AnimationCurve curva = new AnimationCurve();
        curva.AddKey(0f, 1f);
        curva.AddKey(1f, 0f);
        size.size = new ParticleSystem.MinMaxCurve(1f, curva);

        // Color sobre tiempo (fade out)
        var color = particulas.colorOverLifetime;
        color.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(1f, 0.2f, 0.2f), 0f),
                new GradientColorKey(new Color(1f, 0.5f, 0.5f), 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        color.color = new ParticleSystem.MinMaxGradient(grad);

        particulas.Play();
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time - tiempoUltimaCuracion < cooldown) return;

        PlayerStats stats = other.GetComponent<PlayerStats>();
        if (stats == null) stats = other.GetComponentInParent<PlayerStats>();
        if (stats == null) return;

        // Solo cura si no está al máximo
        if (stats.currentHealth >= stats.maxHealth) return;

        float curacion = Mathf.Min(cantidadCuracion, stats.maxHealth - stats.currentHealth);
        // Curar: TakeDamage con valor negativo no funciona, usamos el campo directamente
        stats.currentHealth = Mathf.Clamp(stats.currentHealth + curacion, 0, stats.maxHealth);

        // Actualizar barra de vida si existe
        if (stats.barraVida != null)
            stats.barraVida.value = stats.currentHealth;

        tiempoUltimaCuracion = Time.time;

        Debug.Log($"[ZonaCuracion] Curado {curacion} HP. Vida actual: {stats.currentHealth}");
    }

    // Dibuja el círculo en el editor
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, radioCirculo);
    }
}
