using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// Maneja el HUD de vida con corazones animados en circulo
public class HUDVida : MonoBehaviour
{
    [Header("Configuracion")]
    public int maxCorazones = 5;
    public float vidaPorCorazon = 20f; // 100 vida / 5 corazones = 20 por corazon

    [Header("Referencias")]
    public PlayerStats playerStats;

    // Internos
    private List<Image> corazones = new List<Image>();
    private List<Image> circulos  = new List<Image>();
    private int corazonesActuales;
    private int corazonesAnteriores;

    // Colores
    private static readonly Color colorLleno    = new Color(0.95f, 0.2f,  0.2f,  1f);
    private static readonly Color colorVacio     = new Color(0.25f, 0.05f, 0.05f, 0.7f);
    private static readonly Color colorCirculo   = new Color(0.15f, 0.15f, 0.15f, 0.85f);
    private static readonly Color colorBorde     = new Color(0.8f,  0.15f, 0.15f, 1f);

    void Start()
    {
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();

        ConstruirCorazones();
        corazonesActuales    = maxCorazones;
        corazonesAnteriores  = maxCorazones;
    }

    void Update()
    {
        if (playerStats == null) return;

        int nuevos = Mathf.CeilToInt(playerStats.currentHealth / vidaPorCorazon);
        nuevos = Mathf.Clamp(nuevos, 0, maxCorazones);

        if (nuevos != corazonesActuales)
        {
            int anterior = corazonesActuales;
            corazonesActuales = nuevos;
            ActualizarCorazones(anterior, nuevos);
        }
    }

    void ConstruirCorazones()
    {
        // Limpiar hijos existentes
        foreach (Transform hijo in transform)
            Destroy(hijo.gameObject);
        corazones.Clear();
        circulos.Clear();

        float tamano   = 48f;
        float espaciado = 56f;
        float totalAncho = (maxCorazones - 1) * espaciado;
        float startX = -totalAncho / 2f;

        for (int i = 0; i < maxCorazones; i++)
        {
            float x = startX + i * espaciado;

            // ── Circulo de fondo ──────────────────────────────────────
            var circuloGO = new GameObject("Circulo_" + i);
            circuloGO.transform.SetParent(transform, false);
            var circuloRT = circuloGO.AddComponent<RectTransform>();
            circuloRT.anchoredPosition = new Vector2(x, 0f);
            circuloRT.sizeDelta = new Vector2(tamano + 8f, tamano + 8f);

            var circuloImg = circuloGO.AddComponent<Image>();
            circuloImg.sprite = CrearSpritCirculo(64);
            circuloImg.color  = colorCirculo;
            circuloImg.raycastTarget = false;
            circulos.Add(circuloImg);

            // ── Borde del circulo ─────────────────────────────────────
            var bordeGO = new GameObject("Borde_" + i);
            bordeGO.transform.SetParent(circuloGO.transform, false);
            var bordeRT = bordeGO.AddComponent<RectTransform>();
            bordeRT.anchorMin = Vector2.zero; bordeRT.anchorMax = Vector2.one;
            bordeRT.offsetMin = new Vector2(-2f, -2f);
            bordeRT.offsetMax = new Vector2(2f, 2f);
            var bordeImg = bordeGO.AddComponent<Image>();
            bordeImg.sprite = CrearSpritAnillo(64, 4);
            bordeImg.color  = colorBorde;
            bordeImg.raycastTarget = false;

            // ── Corazon ───────────────────────────────────────────────
            var corazonGO = new GameObject("Corazon_" + i);
            corazonGO.transform.SetParent(circuloGO.transform, false);
            var corazonRT = corazonGO.AddComponent<RectTransform>();
            corazonRT.anchorMin = new Vector2(0.1f, 0.1f);
            corazonRT.anchorMax = new Vector2(0.9f, 0.9f);
            corazonRT.offsetMin = Vector2.zero;
            corazonRT.offsetMax = Vector2.zero;

            var corazonImg = corazonGO.AddComponent<Image>();
            corazonImg.sprite = CrearSpriteCorazon(64);
            corazonImg.color  = colorLleno;
            corazonImg.raycastTarget = false;
            corazones.Add(corazonImg);

            // Animacion de latido continuo
            StartCoroutine(AnimarLatido(circuloGO.transform, i));
        }
    }

    void ActualizarCorazones(int anterior, int nuevo)
    {
        for (int i = 0; i < maxCorazones; i++)
        {
            bool lleno = i < nuevo;
            corazones[i].color = lleno ? colorLleno : colorVacio;

            // Animar perdida de corazon
            if (i >= nuevo && i < anterior)
                StartCoroutine(AnimarPerdida(corazones[i].transform.parent));
        }
    }

    // ── Animaciones ───────────────────────────────────────────────────

    IEnumerator AnimarLatido(Transform t, int indice)
    {
        // Desfase para que no latan todos al mismo tiempo
        yield return new WaitForSeconds(indice * 0.15f);

        while (true)
        {
            // Latido: escala rapida hacia arriba y vuelve
            yield return StartCoroutine(Escalar(t, 1f, 1.18f, 0.12f));
            yield return StartCoroutine(Escalar(t, 1.18f, 0.95f, 0.08f));
            yield return StartCoroutine(Escalar(t, 0.95f, 1f, 0.06f));
            // Pausa entre latidos
            yield return new WaitForSeconds(0.75f);
        }
    }

    IEnumerator AnimarPerdida(Transform t)
    {
        // Sacudida + encogimiento al perder un corazon
        yield return StartCoroutine(Escalar(t, 1f, 1.3f, 0.08f));
        yield return StartCoroutine(Escalar(t, 1.3f, 0.7f, 0.12f));
        yield return StartCoroutine(Escalar(t, 0.7f, 1f, 0.1f));
    }

    IEnumerator Escalar(Transform t, float desde, float hasta, float duracion)
    {
        float tiempo = 0f;
        while (tiempo < duracion)
        {
            tiempo += Time.unscaledDeltaTime;
            float s = Mathf.Lerp(desde, hasta, tiempo / duracion);
            t.localScale = Vector3.one * s;
            yield return null;
        }
        t.localScale = Vector3.one * hasta;
    }

    // ── Generacion de sprites por codigo ─────────────────────────────

    Sprite CrearSpritCirculo(int size)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        float centro = size / 2f;
        float radio  = size / 2f - 1f;
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float dist = Vector2.Distance(new Vector2(x, y), new Vector2(centro, centro));
            float alpha = Mathf.Clamp01(1f - (dist - radio + 1.5f));
            tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    Sprite CrearSpritAnillo(int size, int grosor)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        float centro = size / 2f;
        float radioExt = size / 2f - 1f;
        float radioInt = radioExt - grosor;
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float dist = Vector2.Distance(new Vector2(x, y), new Vector2(centro, centro));
            float alpha = 0f;
            if (dist <= radioExt && dist >= radioInt)
                alpha = Mathf.Clamp01(Mathf.Min(radioExt - dist, dist - radioInt) + 0.5f);
            tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    Sprite CrearSpriteCorazon(int size)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        // Limpiar
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
            tex.SetPixel(x, y, Color.clear);

        float s = size;
        // Dibujar corazon con formula matematica
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            // Normalizar a [-1.5, 1.5]
            float nx = (x / s - 0.5f) * 3f;
            float ny = (y / s - 0.35f) * 3f;

            // Formula del corazon: (x^2 + y^2 - 1)^3 - x^2*y^3 <= 0
            float val = Mathf.Pow(nx*nx + ny*ny - 1f, 3f) - nx*nx * ny*ny*ny;
            if (val <= 0f)
            {
                // Suavizar bordes
                float borde = Mathf.Clamp01(-val * 2f + 1f);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Clamp01(borde + 0.5f)));
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}
