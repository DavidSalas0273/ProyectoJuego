using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelMenu;
    public GameObject panelOpciones;
    public CanvasGroup fadeOverlay;          // Image negra que cubre toda la pantalla

    [Header("Video de fondo")]
    public VideoPlayer videoPlayer;
    public RawImage videoRawImage;           // RawImage que muestra el video

    [Header("Titulo")]
    public CanvasGroup tituloCG;             // CanvasGroup del texto titulo

    [Header("Botones del menu")]
    public Button botonJugar;
    public Button botonOpciones;
    public Button botonSalir;

    [Header("Opciones - Audio")]
    public Slider sliderMusica;
    public Slider sliderSFX;
    public Toggle togglePantallaCompleta;

    [Header("Opciones - Graficos")]
    public Dropdown dropdownCalidad;

    // Claves PlayerPrefs
    const string KEY_MUSICA     = "vol_musica";
    const string KEY_SFX        = "vol_sfx";
    const string KEY_FULLSCREEN = "fullscreen";
    const string KEY_CALIDAD    = "calidad";

    void Start()
    {
        // Asegurar estado inicial
        panelMenu?.SetActive(true);
        panelOpciones?.SetActive(false);

        // Fade in al abrir el menu
        if (fadeOverlay != null)
            StartCoroutine(FadeIn());

        // Animar titulo
        if (tituloCG != null)
            StartCoroutine(AnimarTitulo());

        // Iniciar video de fondo en loop
        if (videoPlayer != null)
        {
            videoPlayer.isLooping = true;
            videoPlayer.Play();
        }

        // Cargar configuracion guardada
        CargarOpciones();

        // Asignar listeners de botones
        botonJugar?.onClick.AddListener(BotonJugar);
        botonOpciones?.onClick.AddListener(BotonOpciones);
        botonSalir?.onClick.AddListener(BotonSalir);
    }

    // ── Navegacion ────────────────────────────────────────────────────

    public void BotonJugar()
    {
        StartCoroutine(TransicionEscena("Game"));
    }

    public void BotonOpciones()
    {
        panelMenu?.SetActive(false);
        panelOpciones?.SetActive(true);
    }

    public void BotonSalir()
    {
        StartCoroutine(SalirConFade());
    }

    public void BotonVolverAlMenu()
    {
        GuardarOpciones();
        panelOpciones?.SetActive(false);
        panelMenu?.SetActive(true);
    }

    // ── Opciones ──────────────────────────────────────────────────────

    public void CambiarVolumenMusica(float valor)
    {
        AudioListener.volume = valor;
        PlayerPrefs.SetFloat(KEY_MUSICA, valor);
    }

    public void CambiarVolumenSFX(float valor)
    {
        PlayerPrefs.SetFloat(KEY_SFX, valor);
        var sources = FindObjectsOfType<AudioSource>();
        foreach (var s in sources)
            if (!s.loop) s.volume = valor;
    }

    public void CambiarPantallaCompleta(bool valor)
    {
        Screen.fullScreen = valor;
        PlayerPrefs.SetInt(KEY_FULLSCREEN, valor ? 1 : 0);
    }

    public void CambiarCalidad(int indice)
    {
        QualitySettings.SetQualityLevel(indice);
        PlayerPrefs.SetInt(KEY_CALIDAD, indice);
    }

    void CargarOpciones()
    {
        float musica     = PlayerPrefs.GetFloat(KEY_MUSICA,     0.8f);
        float sfx        = PlayerPrefs.GetFloat(KEY_SFX,        1f);
        bool  fullscreen = PlayerPrefs.GetInt(KEY_FULLSCREEN,   1) == 1;
        int   calidad    = PlayerPrefs.GetInt(KEY_CALIDAD,      2);

        if (sliderMusica           != null) sliderMusica.value           = musica;
        if (sliderSFX              != null) sliderSFX.value              = sfx;
        if (togglePantallaCompleta != null) togglePantallaCompleta.isOn  = fullscreen;
        if (dropdownCalidad        != null) dropdownCalidad.value        = calidad;

        AudioListener.volume = musica;
        Screen.fullScreen    = fullscreen;
        QualitySettings.SetQualityLevel(calidad);
    }

    void GuardarOpciones()
    {
        PlayerPrefs.Save();
    }

    // ── Animaciones / Transiciones ────────────────────────────────────

    /// Fade de negro a transparente al entrar al menu
    IEnumerator FadeIn()
    {
        if (fadeOverlay == null) yield break;
        fadeOverlay.alpha = 1f;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 1.5f;
            fadeOverlay.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
        fadeOverlay.alpha = 0f;
    }

    /// Aparicion suave del titulo
    IEnumerator AnimarTitulo()
    {
        if (tituloCG == null) yield break;
        tituloCG.alpha = 0f;
        yield return new WaitForSeconds(0.3f);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 1.2f;
            tituloCG.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
        tituloCG.alpha = 1f;
    }

    /// Fade a negro y carga de escena
    IEnumerator TransicionEscena(string escena)
    {
        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 0f;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * 2f;
                fadeOverlay.alpha = Mathf.Lerp(0f, 1f, t);
                yield return null;
            }
            fadeOverlay.alpha = 1f;
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
        }
        SceneManager.LoadScene(escena);
    }

    /// Fade a negro y salir
    IEnumerator SalirConFade()
    {
        if (fadeOverlay != null)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * 2f;
                fadeOverlay.alpha = Mathf.Lerp(0f, 1f, t);
                yield return null;
            }
        }
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
