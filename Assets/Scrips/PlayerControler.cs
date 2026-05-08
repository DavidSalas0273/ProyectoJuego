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
    public float fuerzaRodar = 8f;
    public float costoStaminaRodar = 25f;
    public float cooldownRodar = 1f;
    public float duracionRodar = 0.4f;

    // ATAQUES
    public float costoStaminaLigero = 10f;
    public float costoStaminaPesado = 25f;
    public float duracionAtaque = 0.5f;

    private CharacterController controller;
    private Vector3 velocidadVertical;
    private Animator animator;
    private PlayerStats stats;
    public GameObject hitbox;

    private bool puedeRodar = true;
    private bool estaRodando = false;
    private bool estaAtacando = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
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
            float angulo = Mathf.Atan2(direccion.x, direccion.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;

            if (z >= 0)
            {
                float anguloSuave = Mathf.LerpAngle(transform.eulerAngles.y, angulo, suavizadoRotacion * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0f, anguloSuave, 0f);
            }

            Vector3 movimiento = Quaternion.Euler(0f, angulo, 0f) * Vector3.forward;
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
        {
            animator.SetTrigger("Roll");
        }

        Vector3 direccion = transform.forward;

        float tiempo = 0f;

        while (tiempo < duracionRodar)
        {
            controller.Move(direccion * fuerzaRodar * Time.deltaTime);
            tiempo += Time.deltaTime;
            yield return null;
        }

        estaRodando = false;

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

    IEnumerator RutinaAtaque(string tipo)
    {
        estaAtacando = true;

        if (animator != null)
        {
            animator.SetBool("IsAttacking", true);
            animator.SetTrigger(tipo);
        }

        yield return new WaitForSeconds(duracionAtaque);

        if (animator != null)
        {
            animator.SetBool("IsAttacking", false);
        }

        estaAtacando = false;
    }

    public void ActivarHitbox()
    {
        Debug.Log("HITBOX ACTIVADO");
        hitbox.SetActive(true);
    }

    public void DesactivarHitbox()
    {
        Debug.Log("HITBOX DESACTIVADO");
        hitbox.SetActive(false);
    }
}