using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Maneja el retrato animado del personaje en el HUD
public class HUDRetrato : MonoBehaviour
{
    [Header("Referencias")]
    public RawImage imagenRetrato;   // RawImage que muestra la RenderTexture
    public Transform objetivo;       // Transform del jugador

    [Header("Camara retrato")]
    public Vector3 offsetCamara  = new Vector3(0f, 1.6f, -2.0f); // frente al personaje
    public float   campoCamara   = 25f; // zoom para ver solo la cara

    private Camera camaraRetrato;
    private RenderTexture renderTex;
    private GameObject camGO;

    void Start()
    {
        if (objetivo == null)
        {
            var j = GameObject.FindGameObjectWithTag("Player");
            if (j != null) objetivo = j.transform;
        }

        CrearCamaraRetrato();
        StartCoroutine(AnimarBorde());
    }

    void CrearCamaraRetrato()
    {
        // RenderTexture circular
        renderTex = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
        renderTex.antiAliasing = 2;
        renderTex.Create();

        // Camara dedicada al retrato
        camGO = new GameObject("CamaraRetrato");
        camaraRetrato = camGO.AddComponent<Camera>();
        camaraRetrato.targetTexture    = renderTex;
        camaraRetrato.fieldOfView      = campoCamara;
        camaraRetrato.clearFlags       = CameraClearFlags.SolidColor;
        camaraRetrato.backgroundColor  = new Color(0.08f, 0.08f, 0.12f, 1f);
        camaraRetrato.nearClipPlane    = 0.1f;
        camaraRetrato.farClipPlane     = 20f;
        camaraRetrato.depth            = -2; // Renderiza antes que la camara principal

        // Asignar a la RawImage
        if (imagenRetrato != null)
            imagenRetrato.texture = renderTex;
    }

    void LateUpdate()
    {
        if (objetivo == null || camaraRetrato == null) return;

        // Posicionar la camara ENFRENTE del personaje a altura de la cara
        Vector3 posicion = objetivo.position
            + objetivo.forward * Mathf.Abs(offsetCamara.z)  // delante
            + Vector3.up * offsetCamara.y;                   // altura cara

        camGO.transform.position = posicion;
        // Mirar hacia la cara del personaje
        camGO.transform.LookAt(objetivo.position + Vector3.up * offsetCamara.y);
    }

    // Animacion del borde del circulo (pulso suave)
    IEnumerator AnimarBorde()
    {
        var borde = GetComponentInChildren<Image>();
        if (borde == null) yield break;

        while (true)
        {
            yield return StartCoroutine(PulsarAlpha(borde, 0.7f, 1f, 1.2f));
            yield return StartCoroutine(PulsarAlpha(borde, 1f, 0.7f, 1.2f));
        }
    }

    IEnumerator PulsarAlpha(Graphic g, float desde, float hasta, float dur)
    {
        float t = 0f;
        Color c = g.color;
        while (t < dur)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(desde, hasta, t / dur);
            g.color = c;
            yield return null;
        }
    }

    void OnDestroy()
    {
        if (renderTex != null) renderTex.Release();
        if (camGO    != null) Destroy(camGO);
    }
}
