using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Controla la animacion y logica visual de la pantalla de muerte
public class PantallamuertAnimacion : MonoBehaviour
{
    [Header("Referencias UI")]
    public Image fondoNegro;        // imagen de fondo que hace fade
    public Text textoMuerte;        // "HAS MUERTO"
    public Text textoSubtitulo;     // subtitulo debajo
    public GameObject botonesPanel; // panel con los botones

    [Header("Configuracion")]
    public float duracionFade = 1.2f;
    public float delayTexto   = 0.6f;
    public float delayBotones = 1.8f;

    void OnEnable()
    {
        // Cada vez que se activa el panel, lanza la animacion
        StartCoroutine(AnimarEntrada());
    }

    IEnumerator AnimarEntrada()
    {
        // Ocultar todo al inicio
        if (fondoNegro   != null) SetAlpha(fondoNegro,   0f);
        if (textoMuerte  != null) { SetAlpha(textoMuerte, 0f);    textoMuerte.transform.localScale  = Vector3.one * 0.5f; }
        if (textoSubtitulo != null) SetAlpha(textoSubtitulo, 0f);
        if (botonesPanel != null) botonesPanel.SetActive(false);

        // 1. Fade in del fondo negro (mundo todavia corriendo)
        yield return StartCoroutine(FadeImagen(fondoNegro, 0f, 0.85f, duracionFade));

        // 2. Pausar el mundo cuando el fondo ya cubre la pantalla
        Time.timeScale = 0f;

        // 3. Texto principal aparece con escala (efecto "impacto")
        yield return new WaitForSecondsRealtime(delayTexto - duracionFade > 0 ? delayTexto - duracionFade : 0f);

        if (textoMuerte != null)
            yield return StartCoroutine(AnimarTextoImpacto(textoMuerte));

        // 4. Subtitulo fade in
        yield return new WaitForSecondsRealtime(0.3f);
        if (textoSubtitulo != null)
            yield return StartCoroutine(FadeTexto(textoSubtitulo, 0f, 1f, 0.5f));

        // 5. Botones aparecen
        yield return new WaitForSecondsRealtime(0.4f);
        if (botonesPanel != null)
        {
            botonesPanel.SetActive(true);
            yield return StartCoroutine(FadeGrupo(botonesPanel, 0f, 1f, 0.4f));
        }
    }

    IEnumerator FadeImagen(Image img, float desde, float hasta, float duracion)
    {
        if (img == null) yield break;
        float t = 0f;
        Color c = img.color;
        while (t < duracion)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(desde, hasta, t / duracion);
            img.color = c;
            yield return null;
        }
        c.a = hasta;
        img.color = c;
    }

    IEnumerator FadeTexto(Text txt, float desde, float hasta, float duracion)
    {
        if (txt == null) yield break;
        float t = 0f;
        Color c = txt.color;
        while (t < duracion)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(desde, hasta, t / duracion);
            txt.color = c;
            yield return null;
        }
        c.a = hasta;
        txt.color = c;
    }

    IEnumerator AnimarTextoImpacto(Text txt)
    {
        float duracion = 0.4f;
        float t = 0f;
        Color c = txt.color;
        while (t < duracion)
        {
            t += Time.unscaledDeltaTime;
            float progreso = t / duracion;
            // Escala de 0.5 a 1.05 y luego a 1.0 (rebote)
            float escala = progreso < 0.7f
                ? Mathf.Lerp(0.5f, 1.08f, progreso / 0.7f)
                : Mathf.Lerp(1.08f, 1.0f, (progreso - 0.7f) / 0.3f);
            txt.transform.localScale = Vector3.one * escala;
            c.a = Mathf.Lerp(0f, 1f, progreso * 2f);
            txt.color = c;
            yield return null;
        }
        txt.transform.localScale = Vector3.one;
        c.a = 1f;
        txt.color = c;
    }

    IEnumerator FadeGrupo(GameObject panel, float desde, float hasta, float duracion)
    {
        var textos   = panel.GetComponentsInChildren<Text>();
        var imagenes = panel.GetComponentsInChildren<Image>();
        float t = 0f;
        while (t < duracion)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(desde, hasta, t / duracion);
            foreach (var txt in textos)   { Color c = txt.color;   c.a = a; txt.color   = c; }
            foreach (var img in imagenes) { Color c = img.color;   c.a = a; img.color   = c; }
            yield return null;
        }
    }

    void SetAlpha(Graphic g, float a)
    {
        if (g == null) return;
        Color c = g.color; c.a = a; g.color = c;
    }
}
