using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

/// <summary>
/// Adjunta este script a un GameObject vacio en la escena MainMenu.
/// En el Editor, haz clic en "Construir Menu" desde el Inspector (o llama BuildMenu()).
/// Solo necesitas ejecutarlo una vez para generar toda la jerarquia UI.
/// </summary>
[ExecuteInEditMode]
public class MainMenuSetup : MonoBehaviour
{
    [Header("Referencia al video")]
    public VideoClip videoClip;

    [ContextMenu("Construir Menu")]
    public void BuildMenu()
    {
        // ── Canvas principal ──────────────────────────────────────────
        Canvas canvas = FindObjectOfType<Canvas>();
        GameObject canvasGO;
        if (canvas == null)
        {
            canvasGO = new GameObject("Canvas_MainMenu");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGO.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        else
        {
            canvasGO = canvas.gameObject;
        }

        // ── Video de fondo ────────────────────────────────────────────
        GameObject videoGO = new GameObject("VideoFondo");
        videoGO.transform.SetParent(canvasGO.transform, false);
        RawImage rawImg = videoGO.AddComponent<RawImage>();
        rawImg.color = Color.white;
        RectTransform videoRT = videoGO.GetComponent<RectTransform>();
        videoRT.anchorMin = Vector2.zero;
        videoRT.anchorMax = Vector2.one;
        videoRT.offsetMin = Vector2.zero;
        videoRT.offsetMax = Vector2.zero;

        // RenderTexture para el video
        RenderTexture rt = new RenderTexture(1920, 1080, 0);
        rt.name = "VideoRT";
        rawImg.texture = rt;

        // VideoPlayer en camara
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camGO = new GameObject("Main Camera");
            cam = camGO.AddComponent<Camera>();
            camGO.tag = "MainCamera";
        }
        VideoPlayer vp = cam.GetComponent<VideoPlayer>();
        if (vp == null) vp = cam.gameObject.AddComponent<VideoPlayer>();
        vp.renderMode = VideoRenderMode.RenderTexture;
        vp.targetTexture = rt;
        vp.isLooping = true;
        if (videoClip != null) vp.clip = videoClip;

        // ── Overlay oscuro sobre el video ─────────────────────────────
        GameObject overlayGO = new GameObject("Overlay");
        overlayGO.transform.SetParent(canvasGO.transform, false);
        Image overlayImg = overlayGO.AddComponent<Image>();
        overlayImg.color = new Color(0f, 0f, 0f, 0.55f);
        RectTransform overlayRT = overlayGO.GetComponent<RectTransform>();
        overlayRT.anchorMin = Vector2.zero;
        overlayRT.anchorMax = Vector2.one;
        overlayRT.offsetMin = Vector2.zero;
        overlayRT.offsetMax = Vector2.zero;

        // ── Panel Menu ────────────────────────────────────────────────
        GameObject panelMenuGO = new GameObject("PanelMenu");
        panelMenuGO.transform.SetParent(canvasGO.transform, false);
        RectTransform panelMenuRT = panelMenuGO.AddComponent<RectTransform>();
        panelMenuRT.anchorMin = new Vector2(0f, 0f);
        panelMenuRT.anchorMax = new Vector2(1f, 1f);
        panelMenuRT.offsetMin = Vector2.zero;
        panelMenuRT.offsetMax = Vector2.zero;

        // Titulo
        GameObject tituloGO = new GameObject("Titulo");
        tituloGO.transform.SetParent(panelMenuGO.transform, false);
        CanvasGroup tituloCG = tituloGO.AddComponent<CanvasGroup>();
        TextMeshProUGUI tituloTMP = tituloGO.AddComponent<TextMeshProUGUI>();
        tituloTMP.text = "DUNGEONS & SWORD";
        tituloTMP.fontSize = 72;
        tituloTMP.fontStyle = FontStyles.Bold;
        tituloTMP.alignment = TextAlignmentOptions.Center;
        tituloTMP.color = new Color(1f, 0.85f, 0.3f, 1f); // dorado
        RectTransform tituloRT = tituloGO.GetComponent<RectTransform>();
        tituloRT.anchorMin = new Vector2(0.1f, 0.72f);
        tituloRT.anchorMax = new Vector2(0.9f, 0.92f);
        tituloRT.offsetMin = Vector2.zero;
        tituloRT.offsetMax = Vector2.zero;

        // Subtitulo
        GameObject subGO = new GameObject("Subtitulo");
        subGO.transform.SetParent(panelMenuGO.transform, false);
        TextMeshProUGUI subTMP = subGO.AddComponent<TextMeshProUGUI>();
        subTMP.text = "Una aventura de mazmorras te espera";
        subTMP.fontSize = 24;
        subTMP.alignment = TextAlignmentOptions.Center;
        subTMP.color = new Color(0.85f, 0.85f, 0.85f, 1f);
        RectTransform subRT = subGO.GetComponent<RectTransform>();
        subRT.anchorMin = new Vector2(0.1f, 0.65f);
        subRT.anchorMax = new Vector2(0.9f, 0.73f);
        subRT.offsetMin = Vector2.zero;
        subRT.offsetMax = Vector2.zero;

        // Botones
        string[] nombresBtn = { "JUGAR", "OPCIONES", "SALIR" };
        float[] posYBtn     = { 0.52f,   0.40f,      0.28f  };
        Color colorBtn      = new Color(0.12f, 0.08f, 0.04f, 0.85f);
        Color colorHover    = new Color(0.6f,  0.4f,  0.1f,  1f);

        Button botonJugar = null, botonOpciones = null, botonSalir = null;

        for (int i = 0; i < nombresBtn.Length; i++)
        {
            GameObject btnGO = new GameObject("Boton_" + nombresBtn[i]);
            btnGO.transform.SetParent(panelMenuGO.transform, false);

            Image btnImg = btnGO.AddComponent<Image>();
            btnImg.color = colorBtn;

            Button btn = btnGO.AddComponent<Button>();
            ColorBlock cb = btn.colors;
            cb.normalColor      = colorBtn;
            cb.highlightedColor = colorHover;
            cb.pressedColor     = new Color(0.8f, 0.5f, 0.1f, 1f);
            btn.colors = cb;

            RectTransform btnRT = btnGO.GetComponent<RectTransform>();
            btnRT.anchorMin = new Vector2(0.35f, posYBtn[i]);
            btnRT.anchorMax = new Vector2(0.65f, posYBtn[i] + 0.09f);
            btnRT.offsetMin = Vector2.zero;
            btnRT.offsetMax = Vector2.zero;

            // Texto del boton
            GameObject txtGO = new GameObject("Texto");
            txtGO.transform.SetParent(btnGO.transform, false);
            TextMeshProUGUI txtTMP = txtGO.AddComponent<TextMeshProUGUI>();
            txtTMP.text = nombresBtn[i];
            txtTMP.fontSize = 28;
            txtTMP.fontStyle = FontStyles.Bold;
            txtTMP.alignment = TextAlignmentOptions.Center;
            txtTMP.color = Color.white;
            RectTransform txtRT = txtGO.GetComponent<RectTransform>();
            txtRT.anchorMin = Vector2.zero;
            txtRT.anchorMax = Vector2.one;
            txtRT.offsetMin = Vector2.zero;
            txtRT.offsetMax = Vector2.zero;

            if (i == 0) botonJugar    = btn;
            if (i == 1) botonOpciones = btn;
            if (i == 2) botonSalir    = btn;
        }

        // Version
        GameObject verGO = new GameObject("Version");
        verGO.transform.SetParent(panelMenuGO.transform, false);
        TextMeshProUGUI verTMP = verGO.AddComponent<TextMeshProUGUI>();
        verTMP.text = "v0.1 Alpha";
        verTMP.fontSize = 16;
        verTMP.alignment = TextAlignmentOptions.BottomRight;
        verTMP.color = new Color(0.6f, 0.6f, 0.6f, 0.7f);
        RectTransform verRT = verGO.GetComponent<RectTransform>();
        verRT.anchorMin = new Vector2(0.75f, 0.02f);
        verRT.anchorMax = new Vector2(0.98f, 0.08f);
        verRT.offsetMin = Vector2.zero;
        verRT.offsetMax = Vector2.zero;

        // ── Panel Opciones ────────────────────────────────────────────
        GameObject panelOpGO = new GameObject("PanelOpciones");
        panelOpGO.transform.SetParent(canvasGO.transform, false);
        Image panelOpImg = panelOpGO.AddComponent<Image>();
        panelOpImg.color = new Color(0.05f, 0.03f, 0.02f, 0.92f);
        RectTransform panelOpRT = panelOpGO.GetComponent<RectTransform>();
        panelOpRT.anchorMin = new Vector2(0.2f, 0.1f);
        panelOpRT.anchorMax = new Vector2(0.8f, 0.9f);
        panelOpRT.offsetMin = Vector2.zero;
        panelOpRT.offsetMax = Vector2.zero;

        // Titulo opciones
        GameObject opTitGO = new GameObject("TituloOpciones");
        opTitGO.transform.SetParent(panelOpGO.transform, false);
        TextMeshProUGUI opTitTMP = opTitGO.AddComponent<TextMeshProUGUI>();
        opTitTMP.text = "OPCIONES";
        opTitTMP.fontSize = 42;
        opTitTMP.fontStyle = FontStyles.Bold;
        opTitTMP.alignment = TextAlignmentOptions.Center;
        opTitTMP.color = new Color(1f, 0.85f, 0.3f, 1f);
        RectTransform opTitRT = opTitGO.GetComponent<RectTransform>();
        opTitRT.anchorMin = new Vector2(0.05f, 0.82f);
        opTitRT.anchorMax = new Vector2(0.95f, 0.97f);
        opTitRT.offsetMin = Vector2.zero;
        opTitRT.offsetMax = Vector2.zero;

        // Boton volver
        GameObject volverGO = new GameObject("BotonVolver");
        volverGO.transform.SetParent(panelOpGO.transform, false);
        Image volverImg = volverGO.AddComponent<Image>();
        volverImg.color = colorBtn;
        Button volverBtn = volverGO.AddComponent<Button>();
        RectTransform volverRT = volverGO.GetComponent<RectTransform>();
        volverRT.anchorMin = new Vector2(0.3f, 0.05f);
        volverRT.anchorMax = new Vector2(0.7f, 0.15f);
        volverRT.offsetMin = Vector2.zero;
        volverRT.offsetMax = Vector2.zero;
        GameObject volverTxtGO = new GameObject("Texto");
        volverTxtGO.transform.SetParent(volverGO.transform, false);
        TextMeshProUGUI volverTMP = volverTxtGO.AddComponent<TextMeshProUGUI>();
        volverTMP.text = "VOLVER";
        volverTMP.fontSize = 26;
        volverTMP.fontStyle = FontStyles.Bold;
        volverTMP.alignment = TextAlignmentOptions.Center;
        volverTMP.color = Color.white;
        RectTransform volverTxtRT = volverTxtGO.GetComponent<RectTransform>();
        volverTxtRT.anchorMin = Vector2.zero;
        volverTxtRT.anchorMax = Vector2.one;
        volverTxtRT.offsetMin = Vector2.zero;
        volverTxtRT.offsetMax = Vector2.zero;

        panelOpGO.SetActive(false);

        // ── Fade Overlay ──────────────────────────────────────────────
        GameObject fadeGO = new GameObject("FadeOverlay");
        fadeGO.transform.SetParent(canvasGO.transform, false);
        Image fadeImg = fadeGO.AddComponent<Image>();
        fadeImg.color = Color.black;
        CanvasGroup fadeCG = fadeGO.AddComponent<CanvasGroup>();
        fadeCG.alpha = 1f;
        fadeCG.blocksRaycasts = false;
        RectTransform fadeRT = fadeGO.GetComponent<RectTransform>();
        fadeRT.anchorMin = Vector2.zero;
        fadeRT.anchorMax = Vector2.one;
        fadeRT.offsetMin = Vector2.zero;
        fadeRT.offsetMax = Vector2.zero;

        // ── Asignar referencias al MainMenuManager ────────────────────
        MainMenuManager mgr = FindObjectOfType<MainMenuManager>();
        if (mgr == null)
        {
            GameObject mgrGO = new GameObject("MainMenuManager");
            mgr = mgrGO.AddComponent<MainMenuManager>();
        }

        mgr.panelMenu      = panelMenuGO;
        mgr.panelOpciones  = panelOpGO;
        mgr.fadeOverlay    = fadeCG;
        mgr.videoPlayer    = vp;
        mgr.videoRawImage  = rawImg;
        mgr.tituloCG       = tituloCG;
        mgr.botonJugar     = botonJugar;
        mgr.botonOpciones  = botonOpciones;
        mgr.botonSalir     = botonSalir;

        // Asignar listeners del boton volver
        volverBtn.onClick.AddListener(mgr.BotonVolverAlMenu);

        Debug.Log("[MainMenuSetup] Menu construido correctamente.");
    }
}
