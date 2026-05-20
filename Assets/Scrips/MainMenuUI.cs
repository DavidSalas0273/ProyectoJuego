using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;

// Alias para evitar ambiguedad con UnityEngine.UI
using UIButton  = UnityEngine.UIElements.Button;
using UISlider  = UnityEngine.UIElements.Slider;
using UIToggle  = UnityEngine.UIElements.Toggle;

/// <summary>
/// Controlador del menu principal.
/// Requiere: UIDocument, VideoPlayer en VideoBackground, RawImage para el video.
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class MainMenuUI : MonoBehaviour
{
    [Header("Video de fondo")]
    public VideoPlayer videoPlayer;
    public UnityEngine.UI.RawImage videoRawImage;   // RawImage que cubre toda la pantalla

    // Elementos UI Toolkit
    private VisualElement root;
    private VisualElement optionsPanel;
    private VisualElement btnContainer;
    private UIButton btnPlay;
    private UIButton btnOptions;
    private UIButton btnQuit;
    private UIButton btnBack;
    private UISlider sliderMusic;
    private UISlider sliderSFX;
    private UIToggle toggleFullscreen;
    private VisualElement fadeOverlay;

    // PlayerPrefs keys
    const string KEY_MUSIC      = "vol_musica";
    const string KEY_SFX        = "vol_sfx";
    const string KEY_FULLSCREEN = "fullscreen";

    void OnEnable()
    {
        var doc = GetComponent<UIDocument>();
        root = doc.rootVisualElement;

        optionsPanel     = root.Q<VisualElement>("options-panel");
        btnContainer     = root.Q<VisualElement>("btn-container");
        btnPlay          = root.Q<UIButton>("btn-play");
        btnOptions       = root.Q<UIButton>("btn-options");
        btnQuit          = root.Q<UIButton>("btn-quit");
        btnBack          = root.Q<UIButton>("btn-back");
        sliderMusic      = root.Q<UISlider>("slider-music");
        sliderSFX        = root.Q<UISlider>("slider-sfx");
        toggleFullscreen = root.Q<UIToggle>("toggle-fullscreen");

        // Fade overlay dinamico (encima de todo)
        fadeOverlay = new VisualElement();
        fadeOverlay.style.position         = Position.Absolute;
        fadeOverlay.style.left             = 0;
        fadeOverlay.style.top              = 0;
        fadeOverlay.style.width            = Length.Percent(100);
        fadeOverlay.style.height           = Length.Percent(100);
        fadeOverlay.style.backgroundColor  = new StyleColor(new Color(0, 0, 0, 1));
        fadeOverlay.pickingMode            = PickingMode.Ignore;
        root.Add(fadeOverlay);

        // Estado inicial
        if (optionsPanel != null) optionsPanel.style.display = DisplayStyle.None;

        LoadOptions();

        btnPlay?.RegisterCallback<ClickEvent>(e => StartCoroutine(LoadScene("Intro")));
        btnOptions?.RegisterCallback<ClickEvent>(e => OpenOptions());
        btnQuit?.RegisterCallback<ClickEvent>(e => StartCoroutine(QuitGame()));
        btnBack?.RegisterCallback<ClickEvent>(e => CloseOptions());
        sliderMusic?.RegisterValueChangedCallback(e => OnMusicChanged(e.newValue));
        sliderSFX?.RegisterValueChangedCallback(e => OnSFXChanged(e.newValue));
        toggleFullscreen?.RegisterValueChangedCallback(e => OnFullscreenChanged(e.newValue));

        // Configurar video con RenderTexture
        SetupVideo();

        StartCoroutine(FadeIn());
    }

    void SetupVideo()
    {
        if (videoPlayer == null) return;

        // Crear RenderTexture en runtime
        RenderTexture rt = new RenderTexture(1920, 1080, 0);
        rt.Create();

        videoPlayer.renderMode    = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = rt;
        videoPlayer.isLooping     = true;
        videoPlayer.playOnAwake   = true;
        videoPlayer.Play();

        // Asignar al RawImage si existe
        if (videoRawImage != null)
        {
            videoRawImage.texture = rt;
        }
    }

    void LoadOptions()
    {
        float music      = PlayerPrefs.GetFloat(KEY_MUSIC,      0.8f);
        float sfx        = PlayerPrefs.GetFloat(KEY_SFX,        1f);
        bool  fullscreen = PlayerPrefs.GetInt(KEY_FULLSCREEN,   1) == 1;

        if (sliderMusic      != null) sliderMusic.value      = music;
        if (sliderSFX        != null) sliderSFX.value        = sfx;
        if (toggleFullscreen != null) toggleFullscreen.value = fullscreen;

        AudioListener.volume = music;
        Screen.fullScreen    = fullscreen;
    }

    void OnMusicChanged(float val)
    {
        AudioListener.volume = val;
        PlayerPrefs.SetFloat(KEY_MUSIC, val);
    }

    void OnSFXChanged(float val)
    {
        PlayerPrefs.SetFloat(KEY_SFX, val);
        var sources = FindObjectsOfType<AudioSource>();
        foreach (var s in sources)
            if (!s.loop) s.volume = val;
    }

    void OnFullscreenChanged(bool val)
    {
        Screen.fullScreen = val;
        PlayerPrefs.SetInt(KEY_FULLSCREEN, val ? 1 : 0);
    }

    void OpenOptions()
    {
        if (btnContainer != null) btnContainer.style.display = DisplayStyle.None;
        if (optionsPanel != null) optionsPanel.style.display = DisplayStyle.Flex;
    }

    void CloseOptions()
    {
        PlayerPrefs.Save();
        if (optionsPanel != null) optionsPanel.style.display = DisplayStyle.None;
        if (btnContainer != null) btnContainer.style.display = DisplayStyle.Flex;
    }

    IEnumerator FadeIn()
    {
        float t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime * 1.5f;
            if (fadeOverlay != null)
                fadeOverlay.style.backgroundColor = new StyleColor(new Color(0, 0, 0, Mathf.Clamp01(t)));
            yield return null;
        }
        if (fadeOverlay != null)
            fadeOverlay.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0));
    }

    IEnumerator LoadScene(string sceneName)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            if (fadeOverlay != null)
                fadeOverlay.style.backgroundColor = new StyleColor(new Color(0, 0, 0, Mathf.Clamp01(t)));
            yield return null;
        }
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator QuitGame()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            if (fadeOverlay != null)
                fadeOverlay.style.backgroundColor = new StyleColor(new Color(0, 0, 0, Mathf.Clamp01(t)));
            yield return null;
        }
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
