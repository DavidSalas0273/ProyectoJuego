using UnityEngine;
using UnityEngine.UI;

public class TutorialProximidad : MonoBehaviour
{
    [Header("Configuración")]
    public float distanciaActivacion = 4f;
    public Transform jugador;

    [Header("UI")]
    public Canvas canvasMundo;       // Canvas en World Space hijo de este objeto
    public GameObject panelTutorial; // Panel con el texto

    private bool visible = false;

    void Start()
    {
        // Buscar jugador automáticamente si no está asignado
        if (jugador == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) jugador = go.transform;
        }

        if (panelTutorial != null)
            panelTutorial.SetActive(false);
    }

    void Update()
    {
        if (jugador == null || panelTutorial == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);
        bool deberiaVerse = distancia <= distanciaActivacion;

        if (deberiaVerse != visible)
        {
            visible = deberiaVerse;
            panelTutorial.SetActive(visible);
        }

        // El canvas siempre mira a la cámara
        if (canvasMundo != null && Camera.main != null)
        {
            canvasMundo.transform.LookAt(
                canvasMundo.transform.position + Camera.main.transform.rotation * Vector3.forward,
                Camera.main.transform.rotation * Vector3.up
            );
        }
    }
}
