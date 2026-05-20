using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Salud del enemigo con barra elegante sobre la cabeza.
/// La barra se crea en runtime — no necesita referencias en el Inspector.
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    public float maxVida    = 100f;
    public float vidaActual;

    // barraVida y barraUI se mantienen por compatibilidad pero ya no se usan
    // La barra se crea automáticamente en runtime
    [HideInInspector] public UnityEngine.UI.Slider barraVida;
    [HideInInspector] public GameObject barraUI;

    [Header("Barra sobre la cabeza")]
    public float alturaOffset   = 2.2f;   // altura sobre el pivot del enemigo
    public float anchoBarraMundo = 1.2f;  // ancho en unidades de mundo
    public float altoBarraMundo  = 0.12f; // alto en unidades de mundo

    // ── Componentes internos ──────────────────────────────────────────
    private Canvas       canvasMundo;
    private Image        imgFondo;
    private Image        imgBorde;
    private Image        imgRelleno;
    private Image        imgBrillo;
    private Transform    barraTransform;
    private Camera       camPrincipal;

    private bool barraVisible = false;
    private float timerOcultar = 0f;
    private const float TIEMPO_OCULTAR = 3f;  // segundos sin daño para ocultar

    // Colores de la barra según % de vida
    private static readonly Color COLOR_LLENA   = new Color(0.85f, 0.15f, 0.15f, 1f); // rojo
    private static readonly Color COLOR_MEDIA   = new Color(0.95f, 0.55f, 0.05f, 1f); // naranja
    private static readonly Color COLOR_BAJA    = new Color(0.95f, 0.90f, 0.05f, 1f); // amarillo
    private static readonly Color COLOR_CRITICA = new Color(1.00f, 1.00f, 1.00f, 1f); // blanco parpadeante

    void Start()
    {
        vidaActual    = maxVida;
        camPrincipal  = Camera.main;
        ConstruirBarra();
        MostrarBarra(false);
    }

    void ConstruirBarra()
    {
        // ── Canvas en espacio de mundo ────────────────────────────────
        GameObject canvasGO = new GameObject("BarraVida_Canvas");
        canvasGO.transform.SetParent(transform, false);
        canvasGO.transform.localPosition = Vector3.up * alturaOffset;

        canvasMundo = canvasGO.AddComponent<Canvas>();
        canvasMundo.renderMode = RenderMode.WorldSpace;
        canvasMundo.sortingOrder = 10;

        RectTransform canvasRT = canvasGO.GetComponent<RectTransform>();
        canvasRT.sizeDelta = new Vector2(anchoBarraMundo, altoBarraMundo * 3f);
        canvasRT.localScale = Vector3.one * 0.01f; // escala para que quede en unidades de mundo

        barraTransform = canvasGO.transform;

        // ── Sombra / fondo exterior ───────────────────────────────────
        GameObject sombraGO = CrearImagen("Sombra", canvasGO.transform,
            new Color(0f, 0f, 0f, 0.7f),
            new Vector2(anchoBarraMundo * 102f, altoBarraMundo * 120f),
            Vector2.zero);
        // Offset leve para efecto sombra
        sombraGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(1.5f, -1.5f);

        // ── Fondo oscuro ──────────────────────────────────────────────
        GameObject fondoGO = CrearImagen("Fondo", canvasGO.transform,
            new Color(0.08f, 0.08f, 0.08f, 0.92f),
            new Vector2(anchoBarraMundo * 100f, altoBarraMundo * 100f),
            Vector2.zero);
        imgFondo = fondoGO.GetComponent<Image>();

        // ── Relleno de vida ───────────────────────────────────────────
        GameObject rellenoGO = new GameObject("Relleno");
        rellenoGO.transform.SetParent(canvasGO.transform, false);
        imgRelleno = rellenoGO.AddComponent<Image>();
        imgRelleno.color = COLOR_LLENA;
        imgRelleno.type  = Image.Type.Filled;
        imgRelleno.fillMethod = Image.FillMethod.Horizontal;
        imgRelleno.fillOrigin = (int)Image.OriginHorizontal.Left;
        imgRelleno.fillAmount = 1f;
        RectTransform rellenoRT = rellenoGO.GetComponent<RectTransform>();
        rellenoRT.sizeDelta       = new Vector2(anchoBarraMundo * 96f, altoBarraMundo * 80f);
        rellenoRT.anchoredPosition = Vector2.zero;

        // ── Brillo superior (highlight) ───────────────────────────────
        GameObject brilloGO = CrearImagen("Brillo", canvasGO.transform,
            new Color(1f, 1f, 1f, 0.18f),
            new Vector2(anchoBarraMundo * 96f, altoBarraMundo * 35f),
            new Vector2(0f, altoBarraMundo * 22f));
        imgBrillo = brilloGO.GetComponent<Image>();

        // ── Borde exterior ────────────────────────────────────────────
        GameObject bordeGO = CrearImagen("Borde", canvasGO.transform,
            new Color(0.6f, 0.6f, 0.6f, 0.5f),
            new Vector2(anchoBarraMundo * 104f, altoBarraMundo * 124f),
            Vector2.zero);
        imgBorde = bordeGO.GetComponent<Image>();
        // Mover borde al fondo del orden
        bordeGO.transform.SetAsFirstSibling();
        sombraGO.transform.SetAsFirstSibling();
    }

    GameObject CrearImagen(string nombre, Transform padre, Color color, Vector2 size, Vector2 pos)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(padre, false);
        Image img = go.AddComponent<Image>();
        img.color = color;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta       = size;
        rt.anchoredPosition = pos;
        return go;
    }

    void LateUpdate()
    {
        // Siempre mirar a la cámara
        if (barraTransform != null && camPrincipal != null)
        {
            barraTransform.LookAt(
                barraTransform.position + camPrincipal.transform.rotation * Vector3.forward,
                camPrincipal.transform.rotation * Vector3.up
            );
        }

        // Auto-ocultar tras TIEMPO_OCULTAR segundos sin daño
        if (barraVisible)
        {
            timerOcultar -= Time.deltaTime;
            if (timerOcultar <= 0f)
                MostrarBarra(false);

            // Parpadeo cuando vida crítica (< 20%)
            if (vidaActual / maxVida < 0.2f && imgRelleno != null)
            {
                float pulso = Mathf.PingPong(Time.time * 4f, 1f);
                imgRelleno.color = Color.Lerp(COLOR_CRITICA, COLOR_BAJA, pulso);
            }
        }
    }

    public void RecibirDanio(float dano)
    {
        vidaActual = Mathf.Max(0f, vidaActual - dano);

        ActualizarBarra();
        MostrarBarra(true);
        timerOcultar = TIEMPO_OCULTAR;

        if (vidaActual <= 0f)
            Morir();
    }

    void ActualizarBarra()
    {
        if (imgRelleno == null) return;

        float pct = vidaActual / maxVida;
        imgRelleno.fillAmount = pct;

        // Color según porcentaje
        if (pct > 0.6f)
            imgRelleno.color = Color.Lerp(COLOR_MEDIA, COLOR_LLENA, (pct - 0.6f) / 0.4f);
        else if (pct > 0.3f)
            imgRelleno.color = Color.Lerp(COLOR_BAJA, COLOR_MEDIA, (pct - 0.3f) / 0.3f);
        else
            imgRelleno.color = Color.Lerp(COLOR_CRITICA, COLOR_BAJA, pct / 0.3f);

        // Brillo proporcional al relleno
        if (imgBrillo != null)
        {
            RectTransform rt = imgBrillo.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(anchoBarraMundo * 96f * pct, rt.sizeDelta.y);
        }
    }

    void MostrarBarra(bool mostrar)
    {
        barraVisible = mostrar;
        if (canvasMundo != null)
            canvasMundo.gameObject.SetActive(mostrar);
    }

    void Morir()
    {
        Destroy(gameObject);
    }
}
