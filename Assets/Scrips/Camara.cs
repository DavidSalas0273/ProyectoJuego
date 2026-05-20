using UnityEngine;

/// <summary>
/// Cámara third-person que sigue al personaje desde el hombro.
/// Se posiciona detrás y ligeramente arriba del objetivo, sin control del usuario.
/// </summary>
public class Camara : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform objetivo;

    [Header("Posición relativa")]
    public float distancia  = 4f;    // distancia detrás del personaje
    public float altura     = 1.8f;  // altura sobre el personaje
    public float offsetLateral = 0.5f; // desplazamiento lateral (hombro)
    public float suavizado  = 10f;   // qué tan suave sigue al personaje

    [Header("Punto de mira")]
    public float alturaMira = 1.6f;  // a qué altura del personaje mira la cámara
    public float offsetMiraAdelante = 2f; // mira un poco adelante del personaje

    void LateUpdate()
    {
        if (objetivo == null) return;

        // Posición deseada: detrás, arriba y ligeramente al lado del personaje
        Vector3 posicionDeseada = objetivo.position
                                - objetivo.forward * distancia
                                + Vector3.up * altura
                                + objetivo.right * offsetLateral;

        // Suavizar movimiento
        transform.position = Vector3.Lerp(
            transform.position,
            posicionDeseada,
            suavizado * Time.deltaTime
        );

        // Punto de mira: ligeramente adelante y arriba del personaje
        Vector3 puntoMira = objetivo.position 
                          + Vector3.up * alturaMira
                          + objetivo.forward * offsetMiraAdelante;

        // Mirar hacia el punto de mira
        transform.LookAt(puntoMira);
    }
}
