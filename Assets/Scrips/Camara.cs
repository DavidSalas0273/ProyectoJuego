using UnityEngine;

/// <summary>
/// Cámara third-person que sigue al personaje automáticamente.
/// Se posiciona detrás y arriba del objetivo, sin control del usuario.
/// </summary>
public class Camara : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform objetivo;

    [Header("Posición relativa")]
    public float distancia  = 5f;    // distancia detrás del personaje
    public float altura     = 2.5f;  // altura sobre el personaje
    public float suavizado  = 8f;    // qué tan suave sigue al personaje

    [Header("Punto de mira")]
    public float alturaMira = 1.4f;  // a qué altura del personaje mira la cámara

    void LateUpdate()
    {
        if (objetivo == null) return;

        // Posición deseada: detrás y arriba del personaje según su rotación
        Vector3 posicionDeseada = objetivo.position
                                - objetivo.forward * distancia
                                + Vector3.up * altura;

        // Suavizar movimiento
        transform.position = Vector3.Lerp(
            transform.position,
            posicionDeseada,
            suavizado * Time.deltaTime
        );

        // Siempre mirar hacia la cabeza del personaje
        transform.LookAt(objetivo.position + Vector3.up * alturaMira);
    }
}
