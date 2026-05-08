using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyAI : MonoBehaviour
{
    public float velocidad = 2f;
    public float distanciaDeteccion = 10f;
    public float distanciaAtaque = 2f;
    public float tiempoEntreAtaques = 1.5f;

    private Transform jugador;
    private CharacterController controller;
    private Animator animator;

    private float tiempoAtaque;

    void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player").transform;
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (jugador == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia <= distanciaDeteccion)
        {
            Perseguir(distancia);
        }
    }

    void Perseguir(float distancia)
    {
        Vector3 direccion = (jugador.position - transform.position).normalized;
        direccion.y = 0;

        if (distancia > distanciaAtaque)
        {
            controller.Move(direccion * velocidad * Time.deltaTime);

            transform.LookAt(new Vector3(jugador.position.x, transform.position.y, jugador.position.z));

            animator.SetFloat("Speed", 1f);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
            Atacar();
        }
    }

    void Atacar()
    {
        if (Time.time >= tiempoAtaque)
        {
            animator.SetTrigger("Attack");
            tiempoAtaque = Time.time + tiempoEntreAtaques;
        }
    }
}