    using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // MOVIMIENTO
    public float velocidadCaminar = 3f;
    public float velocidadCorrer = 6f;
    public float gravedad = -9.81f;
    public float suavizadoRotacion = 5f;


    // ROLL
    public float fuerzaRodar = 18f;
    public float costoStaminaRodar = 25f;
    public float cooldownRodar = 0.6f;
    public float duracionRodar = 0.5f;

    // ATAQUES
    public float costoStaminaLigero = 10f;
    public float costoStaminaPesado = 25f;
    public float duracionAtaque = 0.5f;
    public float radioDeteccionEnemigo = 5f; // Radio para buscar enemigos al atacar
    public string tagEnemigo = "Enemy";

    private CharacterController controller;
    private Vector3 velocidadVertical;
    private Animator animator;
    private PlayerStats stats;
    public GameObject hitbox;
    public PlayerDamage playerDamage;

    private bool puedeRodar = true;
    private bool estaRodando = false;
    private bool estaAtacando = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();

        if (hitbox != null)
        {
            hitbox.SetActive(false);
        }
    }

    void Update()
    {
        if (!estaRodando && !estaAtacando)
        {
            Mover();
        }

        Rodar();
        Atacar();
    }

    void Mover()
    {
        float x = 0f;
        float z = 0f;

        if (Keyboard.current.wKey.isPressed) z = 1;
        if (Keyboard.current.sKey.isPressed) z = -1;
        if (Keyboard.current.aKey.isPressed) x = -1;
        if (Keyboard.current.dKey.isPressed) x = 1;

        bool corriendo = Keyboard.current.leftShiftKey.isPressed;
        float velocidadActual = corriendo ? velocidadCorrer : velocidadCaminar;

        Vector3 direccion = new Vector3(x, 0f, z).normalized;

        if (direccion.magnitude >= 0.1f)
        {
            // Movimiento relativo al mundo, no a la cámara
            float angulo = Mathf.Atan2(direccion.x, direccion.z) * Mathf.Rad2Deg;

            // Solo rotar cuando se mueve hacia adelante
            if (z >= 0)
            {
                float anguloSuave = Mathf.LerpAngle(transform.eulerAngles.y, angulo, suavizadoRotacion * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0f, anguloSuave, 0f);
            }

            // Movimiento directo en las direcciones del mundo
            Vector3 movimiento = new Vector3(direccion.x, 0f, direccion.z);
            controller.Move(movimiento * velocidadActual * Time.deltaTime);
        }

        // GRAVEDAD
        if (controller.isGrounded)
        {
            velocidadVertical.y = -2f;
        }
        else
        {
            velocidadVertical.y += gravedad * Time.deltaTime;
        }

        controller.Move(velocidadVertical * Time.deltaTime);

        // ANIMACIONES
        if (animator != null)
        {
            animator.SetFloat("X", x, 0.1f, Time.deltaTime);
            animator.SetFloat("Y", z, 0.1f, Time.deltaTime);

            float speedAnim = new Vector2(x, z).magnitude;
            animator.SetFloat("Speed", speedAnim);
        }
    }

    void Rodar()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && puedeRodar && !estaRodando && !estaAtacando)
        {
            if (stats != null && stats.UsarStamina(costoStaminaRodar))
            {
                StartCoroutine(RutinaRodar());
            }
        }
    }

    IEnumerator RutinaRodar()
    {
        puedeRodar = false;
        estaRodando = true;

        if (animator != null)
            animator.SetTrigger("Roll");

        Vector3 direccion = transform.forward;

        // Desplazamiento con deceleración suave (empieza rápido, frena al final)
        float tiempo = 0f;
        while (tiempo < duracionRodar)
        {
            float progreso = tiempo / duracionRodar;
            float velocidadActual = Mathf.Lerp(fuerzaRodar, 0f, progreso);
            controller.Move(direccion * velocidadActual * Time.deltaTime);
            tiempo += Time.deltaTime;
            yield return null;
        }

        // El jugador puede moverse de nuevo inmediatamente al terminar el roll
        estaRodando = false;

        // El cooldown corre en paralelo — no bloquea el movimiento
        StartCoroutine(CooldownRodar());
    }

    IEnumerator CooldownRodar()
    {
        yield return new WaitForSeconds(cooldownRodar);
        puedeRodar = true;
    }

    void Atacar()
    {
        // ATAQUE LIGERO
        if (Mouse.current.leftButton.wasPressedThisFrame && !estaAtacando && !estaRodando)
        {
            if (stats != null && stats.UsarStamina(costoStaminaLigero))
            {
                StartCoroutine(RutinaAtaque("LightAttack"));
            }
        }

        // ATAQUE PESADO
        if (Mouse.current.rightButton.wasPressedThisFrame && !estaAtacando && !estaRodando)
        {
            if (stats != null && stats.UsarStamina(costoStaminaPesado))
            {
                StartCoroutine(RutinaAtaque("HeavyAttack"));
            }
        }
    }

    // Busca el enemigo más cercano dentro del radio y rota el jugador hacia él
    void RotarHaciaEnemigoMasCercano()
    {
        GameObject[] enemigos = GameObject.FindGameObjectsWithTag(tagEnemigo);
        GameObject masCercano = null;
        float menorDistancia = radioDeteccionEnemigo;

        foreach (GameObject enemigo in enemigos)
        {
            float distancia = Vector3.Distance(transform.position, enemigo.transform.position);
            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                masCercano = enemigo;
            }
        }

        if (masCercano != null)
        {
            Vector3 direccion = masCercano.transform.position - transform.position;
            direccion.y = 0f;
            if (direccion != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direccion);
                Debug.Log("🎯 Rotando hacia enemigo: " + masCercano.name);
            }
        }
    }

    IEnumerator RutinaAtaque(string tipo)
    {
        estaAtacando = true;

        // Rotar automáticamente hacia el enemigo más cercano al atacar
        RotarHaciaEnemigoMasCercano();

        if (animator != null)
        {
            animator.SetBool("IsAttacking", true);
            animator.SetTrigger(tipo);
        }

        if (hitbox != null)
        {
            hitbox.SetActive(true);
        }

        if (playerDamage != null)
        {
            if (tipo == "HeavyAttack")
                playerDamage.ActivarDanoPesado();
            else
                playerDamage.ActivarDanioLigero();
        }

        // Ataque activo durante gran parte de la animación
        float tiempoAtaque = duracionAtaque * 0.6f;
        yield return new WaitForSeconds(tiempoAtaque);

        if (hitbox != null)
        {
            hitbox.SetActive(false);
        }

        if (playerDamage != null)
        {
            playerDamage.DesactivarDanio();
        }

        yield return new WaitForSeconds(duracionAtaque - tiempoAtaque);

        if (animator != null)
        {
            animator.SetBool("IsAttacking", false);
        }

        estaAtacando = false;
    }

    public void ActivarHitbox()
    {
        Debug.Log("HITBOX ACTIVADO");
        if (hitbox != null)
            hitbox.SetActive(true);
    }

    public void DesactivarHitbox()
    {
        Debug.Log("HITBOX DESACTIVADO");
        if (hitbox != null)
            hitbox.SetActive(false);
    }
}