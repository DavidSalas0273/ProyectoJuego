using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    // Referencias UI — se crean en Awake si no existen
    [HideInInspector] public GameObject gameOverPanel;
    [HideInInspector] public Button btnReiniciar;
    [HideInInspector] public Button btnMenu;

    private Vector3 posicionMuerte;
    private bool tienePosicionMuerte = false;
    public bool esGameOver = false;

    // Componentes UI internos
    private Image fondoNegro;
    private Text textoMuerte;
    private Text textoSubtitulo;
    private GameObject botonesPanel;

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Siempre reconstruir la UI al iniciar — garantiza que existe aunque
        // el GameManager haya persistido de una sesion anterior
        ConstruirUI();
    }

    void ConstruirUI()
    {
        // Destruir panel anterior si existe (evita duplicados entre sesiones)
        if (gameOverPanel != null)
            Destroy(gameOverPanel);

        // Buscar el Canvas ScreenSpaceOverlay principal
        Canvas canvas = null;
        foreach (var c in FindObjectsOfType<Canvas>())
        {
            if (c.renderMode == RenderMode.ScreenSpaceOverlay && c.transform.parent == null)
            {
                canvas = c;
                break;
            }
        }

        // Si no existe, crear uno
        if (canvas == null)
        {
            var cGO = new GameObject("Canvas");
            canvas = cGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            cGO.AddComponent<CanvasScaler>();
            cGO.AddComponent<GraphicRaycaster>();
        }

        // Asegurar GraphicRaycaster
        if (canvas.GetComponent<GraphicRaycaster>() == null)
            canvas.gameObject.AddComponent<GraphicRaycaster>();

        // Canvas debe estar en sorting order alto para estar encima de todo
        canvas.sortingOrder = 100;

        // Crear EventSystem si no existe
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        // ── Panel raiz ────────────────────────────────────────────────
        gameOverPanel = new GameObject("GameOverPanel");
        gameOverPanel.transform.SetParent(canvas.transform, false);
        var panelRT = gameOverPanel.AddComponent<RectTransform>();
        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;
        gameOverPanel.SetActive(false);

        // ── Fondo negro ───────────────────────────────────────────────
        var fondoGO = new GameObject("FondoNegro");
        fondoGO.transform.SetParent(gameOverPanel.transform, false);
        var fondoRT = fondoGO.AddComponent<RectTransform>();
        fondoRT.anchorMin = Vector2.zero; fondoRT.anchorMax = Vector2.one;
        fondoRT.offsetMin = Vector2.zero; fondoRT.offsetMax = Vector2.zero;
        fondoNegro = fondoGO.AddComponent<Image>();
        fondoNegro.color = new Color(0f, 0f, 0f, 0f);
        fondoNegro.raycastTarget = false; // NO bloquear clics

        // ── Texto HAS MUERTO ──────────────────────────────────────────
        var tmGO = new GameObject("TextoMuerte");
        tmGO.transform.SetParent(gameOverPanel.transform, false);
        var tmRT = tmGO.AddComponent<RectTransform>();
        tmRT.anchorMin = new Vector2(0.1f, 0.55f);
        tmRT.anchorMax = new Vector2(0.9f, 0.78f);
        tmRT.offsetMin = Vector2.zero; tmRT.offsetMax = Vector2.zero;
        textoMuerte = tmGO.AddComponent<Text>();
        textoMuerte.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textoMuerte.text      = "HAS MUERTO";
        textoMuerte.fontSize  = 72;
        textoMuerte.fontStyle = FontStyle.Bold;
        textoMuerte.alignment = TextAnchor.MiddleCenter;
        textoMuerte.color     = new Color(0.85f, 0.1f, 0.1f, 0f);
        textoMuerte.raycastTarget = false;
        var sombra = tmGO.AddComponent<Shadow>();
        sombra.effectColor    = new Color(0f, 0f, 0f, 0.8f);
        sombra.effectDistance = new Vector2(3f, -3f);

        // ── Subtitulo ─────────────────────────────────────────────────
        var subGO = new GameObject("TextoSubtitulo");
        subGO.transform.SetParent(gameOverPanel.transform, false);
        var subRT = subGO.AddComponent<RectTransform>();
        subRT.anchorMin = new Vector2(0.15f, 0.44f);
        subRT.anchorMax = new Vector2(0.85f, 0.55f);
        subRT.offsetMin = Vector2.zero; subRT.offsetMax = Vector2.zero;
        textoSubtitulo = subGO.AddComponent<Text>();
        textoSubtitulo.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textoSubtitulo.text      = "Tu aventura ha terminado... por ahora.";
        textoSubtitulo.fontSize  = 22;
        textoSubtitulo.alignment = TextAnchor.MiddleCenter;
        textoSubtitulo.color     = new Color(0.85f, 0.75f, 0.55f, 0f);
        textoSubtitulo.fontStyle = FontStyle.Italic;
        textoSubtitulo.raycastTarget = false;

        // ── Panel botones ─────────────────────────────────────────────
        botonesPanel = new GameObject("BotonesPanel");
        botonesPanel.transform.SetParent(gameOverPanel.transform, false);
        var bpRT = botonesPanel.AddComponent<RectTransform>();
        bpRT.anchorMin = new Vector2(0.25f, 0.25f);
        bpRT.anchorMax = new Vector2(0.75f, 0.42f);
        bpRT.offsetMin = Vector2.zero; bpRT.offsetMax = Vector2.zero;
        var layout = botonesPanel.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 20f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childForceExpandWidth  = true;
        layout.childForceExpandHeight = true;
        botonesPanel.SetActive(false);

        // ── Botones ───────────────────────────────────────────────────
        btnReiniciar = CrearBoton("BtnReiniciar", "Continuar aqui",
                                  new Color(0.15f, 0.5f, 0.15f), botonesPanel.transform);
        btnMenu      = CrearBoton("BtnMenu",      "Menu principal",
                                  new Color(0.5f, 0.15f, 0.15f), botonesPanel.transform);

        // Listeners directos — sin referencias externas
        btnReiniciar.onClick.AddListener(ReiniciarDondeMurio);
        btnMenu.onClick.AddListener(VolverAlMenu);
    }

    Button CrearBoton(string nombre, string etiqueta, Color color, Transform padre)
    {
        var btnGO = new GameObject(nombre);
        btnGO.transform.SetParent(padre, false);
        btnGO.AddComponent<RectTransform>();

        var img = btnGO.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = true; // SOLO los botones reciben raycast

        var btn = btnGO.AddComponent<Button>();

        // Colores de transicion
        var colors = btn.colors;
        colors.highlightedColor = color * 1.3f;
        colors.pressedColor     = color * 0.7f;
        btn.colors = colors;

        // Texto
        var txtGO = new GameObject("Texto");
        txtGO.transform.SetParent(btnGO.transform, false);
        var txtRT = txtGO.AddComponent<RectTransform>();
        txtRT.anchorMin = Vector2.zero; txtRT.anchorMax = Vector2.one;
        txtRT.offsetMin = Vector2.zero; txtRT.offsetMax = Vector2.zero;
        var txt = txtGO.AddComponent<Text>();
        txt.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.text      = etiqueta;
        txt.fontSize  = 20;
        txt.fontStyle = FontStyle.Bold;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color     = Color.white;
        txt.raycastTarget = false;

        return btn;
    }

    public void SetSpawnPoint(Vector3 posicion, string zona)
    {
        posicionMuerte = posicion;
        tienePosicionMuerte = true;
        Debug.Log("Spawn point actualizado: " + zona + " -> " + posicion);
    }

    public void ActivarGameOver(Vector3 posicion)
    {
        if (esGameOver) return;
        esGameOver = true;
        posicionMuerte = posicion;
        tienePosicionMuerte = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            StartCoroutine(AnimarEntrada());
        }
    }

    public void ActivarGameOver()
    {
        var j = GameObject.FindGameObjectWithTag("Player");
        ActivarGameOver(j != null ? j.transform.position : Vector3.zero);
    }

    IEnumerator AnimarEntrada()
    {
        // Reset visual
        SetAlpha(fondoNegro,     0f);
        SetAlpha(textoMuerte,    0f);
        SetAlpha(textoSubtitulo, 0f);
        textoMuerte.transform.localScale = Vector3.one * 0.5f;
        botonesPanel.SetActive(false);

        // 1. Fade fondo
        yield return StartCoroutine(FadeGraphic(fondoNegro, 0f, 0.85f, 1.2f));

        // 2. Pausar mundo
        Time.timeScale = 0f;

        // 3. Texto impacto
        yield return StartCoroutine(AnimarTextoImpacto());

        // 4. Subtitulo
        yield return new WaitForSecondsRealtime(0.2f);
        yield return StartCoroutine(FadeGraphic(textoSubtitulo, 0f, 1f, 0.5f));

        // 5. Botones
        yield return new WaitForSecondsRealtime(0.3f);
        botonesPanel.SetActive(true);
        yield return StartCoroutine(FadePanel(botonesPanel, 0f, 1f, 0.4f));
    }

    IEnumerator FadeGraphic(Graphic g, float desde, float hasta, float dur)
    {
        float t = 0f;
        Color c = g.color;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(desde, hasta, t / dur);
            g.color = c;
            yield return null;
        }
        c.a = hasta; g.color = c;
    }

    IEnumerator AnimarTextoImpacto()
    {
        float dur = 0.4f, t = 0f;
        Color c = textoMuerte.color;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float p = t / dur;
            float s = p < 0.7f ? Mathf.Lerp(0.5f, 1.08f, p / 0.7f)
                                : Mathf.Lerp(1.08f, 1f, (p - 0.7f) / 0.3f);
            textoMuerte.transform.localScale = Vector3.one * s;
            c.a = Mathf.Clamp01(p * 2f);
            textoMuerte.color = c;
            yield return null;
        }
        textoMuerte.transform.localScale = Vector3.one;
        c.a = 1f; textoMuerte.color = c;
    }

    IEnumerator FadePanel(GameObject panel, float desde, float hasta, float dur)
    {
        float t = 0f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(desde, hasta, t / dur);
            foreach (var g in panel.GetComponentsInChildren<Graphic>())
            {
                Color c = g.color; c.a = a; g.color = c;
            }
            yield return null;
        }
    }

    void SetAlpha(Graphic g, float a)
    { Color c = g.color; c.a = a; g.color = c; }

    public void ReiniciarDondeMurio()
    {
        esGameOver = false;
        Time.timeScale = 1f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        var jugadorGO = GameObject.FindGameObjectWithTag("Player");
        if (jugadorGO != null)
        {
            var cc = jugadorGO.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            jugadorGO.transform.position = tienePosicionMuerte ? posicionMuerte : jugadorGO.transform.position;
            if (cc != null) cc.enabled = true;

            var stats = jugadorGO.GetComponent<PlayerStats>();
            if (stats != null) stats.Respawnear();
        }
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void CargarEscena(string nombre)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombre);
    }

    void Update()
    {
        if (esGameOver && Keyboard.current != null)
        {
            if (Keyboard.current.rKey.wasPressedThisFrame)      ReiniciarDondeMurio();
            if (Keyboard.current.escapeKey.wasPressedThisFrame) VolverAlMenu();
        }
    }
}
