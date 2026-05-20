using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

/// <summary>
/// Intro estilo Star Wars: texto que sube desde abajo con perspectiva.
/// Adjuntar a un GameObject vacio en la escena Intro.
/// </summary>
public class IntroStarWars : MonoBehaviour
{
    [Header("Configuracion")]
    [TextArea(5, 15)]
    public string textoIntro =
        "En un tiempo medieval\nmuy muy lejano...\n\n" +
        "...existia un pequeño barbaro\nque era considerado un heroe\npara su pueblo.\n\n" +
        "Pero un dia, ese pequeño heroe\ntiene que vencer a las artes oscuras\npara salvar a su pueblo.\n\n" +
        "Es ahi donde empieza\nnuestra historia...";

    [Header("Velocidad y duracion")]
    public float velocidadScroll = 60f;   // pixeles por segundo
    public float duracionTotal   = 18f;   // segundos antes de ir al juego
    public float tiempoFadeIn    = 2f;
    public float tiempoFadeOut   = 2f;

    [Header("Escena destino")]
    public string escenaDestino = "Game";

    // Componentes internos
    private Canvas canvas;
    private RectTransform textRect;
    private TextMeshProUGUI tmp;
    private Image fadeImage;
    private float timerTotal = 0f;
    private bool terminado = false;

    void Awake()
    {
        BuildUI();
    }

    void BuildUI()
    {
        // Fondo negro
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camGO = new GameObject("Camera");
            cam = camGO.AddComponent<Camera>();
            cam.tag = "MainCamera";
        }
        cam.backgroundColor = Color.black;
        cam.clearFlags = CameraClearFlags.SolidColor;

        // Canvas
        GameObject canvasGO = new GameObject("Canvas_Intro");
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        // Mascara: recorta el texto para que no se vea fuera del area central
        GameObject maskGO = new GameObject("Mascara");
        maskGO.transform.SetParent(canvasGO.transform, false);
        Image maskImg = maskGO.AddComponent<Image>();
        maskImg.color = Color.black;
        Mask mask = maskGO.AddComponent<Mask>();
        mask.showMaskGraphic = false;
        RectTransform maskRT = maskGO.GetComponent<RectTransform>();
        maskRT.anchorMin = new Vector2(0.15f, 0.0f);
        maskRT.anchorMax = new Vector2(0.85f, 0.85f);
        maskRT.offsetMin = Vector2.zero;
        maskRT.offsetMax = Vector2.zero;

        // Contenedor del texto (dentro de la mascara, empieza abajo)
        GameObject textContainerGO = new GameObject("TextContainer");
        textContainerGO.transform.SetParent(maskGO.transform, false);
        textRect = textContainerGO.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0f, 0f);
        textRect.anchorMax = new Vector2(1f, 0f);
        textRect.pivot     = new Vector2(0.5f, 0f);
        textRect.anchoredPosition = new Vector2(0f, -200f); // empieza debajo

        // Texto TMP
        tmp = textContainerGO.AddComponent<TextMeshProUGUI>();
        tmp.text      = textoIntro;
        tmp.fontSize  = 38;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color     = new Color(1f, 0.92f, 0.4f, 1f); // amarillo dorado
        tmp.fontStyle = FontStyles.Bold;
        tmp.lineSpacing = 8f;

        // Forzar recalculo del tamaño
        tmp.ForceMeshUpdate();
        float alturaTexto = tmp.preferredHeight + 300f;
        textRect.sizeDelta = new Vector2(0f, alturaTexto);

        // Gradiente superior (fade del texto al negro)
        GameObject gradTopGO = new GameObject("GradienteTop");
        gradTopGO.transform.SetParent(canvasGO.transform, false);
        Image gradTop = gradTopGO.AddComponent<Image>();
        gradTop.color = Color.black;
        RectTransform gradTopRT = gradTopGO.GetComponent<RectTransform>();
        gradTopRT.anchorMin = new Vector2(0f, 0.7f);
        gradTopRT.anchorMax = new Vector2(1f, 1f);
        gradTopRT.offsetMin = Vector2.zero;
        gradTopRT.offsetMax = Vector2.zero;

        // Titulo "DUNGEONS & SWORD" arriba
        GameObject tituloGO = new GameObject("TituloIntro");
        tituloGO.transform.SetParent(canvasGO.transform, false);
        TextMeshProUGUI tituloTMP = tituloGO.AddComponent<TextMeshProUGUI>();
        tituloTMP.text      = "DUNGEONS & SWORD";
        tituloTMP.fontSize  = 28;
        tituloTMP.alignment = TextAlignmentOptions.Center;
        tituloTMP.color     = new Color(0.6f, 0.6f, 0.6f, 0.7f);
        tituloTMP.fontStyle = FontStyles.Bold;
        RectTransform tituloRT = tituloGO.GetComponent<RectTransform>();
        tituloRT.anchorMin = new Vector2(0.1f, 0.88f);
        tituloRT.anchorMax = new Vector2(0.9f, 0.97f);
        tituloRT.offsetMin = Vector2.zero;
        tituloRT.offsetMax = Vector2.zero;

        // Texto "Presiona cualquier tecla para saltar"
        GameObject skipGO = new GameObject("SkipText");
        skipGO.transform.SetParent(canvasGO.transform, false);
        TextMeshProUGUI skipTMP = skipGO.AddComponent<TextMeshProUGUI>();
        skipTMP.text      = "Presiona cualquier tecla para saltar";
        skipTMP.fontSize  = 18;
        skipTMP.alignment = TextAlignmentOptions.Center;
        skipTMP.color     = new Color(0.5f, 0.5f, 0.5f, 0.6f);
        RectTransform skipRT = skipGO.GetComponent<RectTransform>();
        skipRT.anchorMin = new Vector2(0.2f, 0.02f);
        skipRT.anchorMax = new Vector2(0.8f, 0.08f);
        skipRT.offsetMin = Vector2.zero;
        skipRT.offsetMax = Vector2.zero;

        // Fade overlay (negro encima de todo para transiciones)
        GameObject fadeGO = new GameObject("FadeOverlay");
        fadeGO.transform.SetParent(canvasGO.transform, false);
        fadeImage = fadeGO.AddComponent<Image>();
        fadeImage.color = Color.black;
        RectTransform fadeRT = fadeGO.GetComponent<RectTransform>();
        fadeRT.anchorMin = Vector2.zero;
        fadeRT.anchorMax = Vector2.one;
        fadeRT.offsetMin = Vector2.zero;
        fadeRT.offsetMax = Vector2.zero;
    }

    void Start()
    {
        StartCoroutine(RunIntro());
    }

    IEnumerator RunIntro()
    {
        // Fade in desde negro
        yield return StartCoroutine(Fade(1f, 0f, tiempoFadeIn));

        float tiempoScroll = duracionTotal - tiempoFadeIn - tiempoFadeOut;
        float alturaTotal  = textRect.sizeDelta.y + 1200f;
        float t = 0f;

        while (t < tiempoScroll && !terminado)
        {
            // Saltar con cualquier tecla (Input System)
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            {
                terminado = true;
                break;
            }
            // También con cualquier botón de gamepad
            if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                terminado = true;
                break;
            }

            t += Time.deltaTime;
            float progreso = t / tiempoScroll;
            float posY = Mathf.Lerp(-200f, alturaTotal, progreso);
            textRect.anchoredPosition = new Vector2(0f, posY);
            yield return null;
        }

        // Fade out y cargar escena
        yield return StartCoroutine(Fade(0f, 1f, tiempoFadeOut));
        SceneManager.LoadScene(escenaDestino);
    }

    IEnumerator Fade(float desde, float hasta, float duracion)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duracion;
            float alpha = Mathf.Lerp(desde, hasta, t);
            if (fadeImage != null)
                fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
    }
}
