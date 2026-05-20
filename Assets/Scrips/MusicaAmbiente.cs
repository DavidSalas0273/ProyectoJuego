using UnityEngine;

/// <summary>
/// Maneja la música de ambiente en la escena Game.
/// Se reproduce automáticamente al iniciar la escena.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class MusicaAmbiente : MonoBehaviour
{
    [Header("Configuración de Audio")]
    public AudioClip clipAmbiente;
    
    [Range(0f, 1f)]
    public float volumen = 0.3f;
    
    [Header("Configuración de Reproducción")]
    public bool reproducirAlIniciar = true;
    public bool loop = true;
    
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        ConfigurarAudioSource();
    }

    void Start()
    {
        if (reproducirAlIniciar && clipAmbiente != null)
        {
            ReproducirMusica();
        }
    }

    void ConfigurarAudioSource()
    {
        audioSource.loop = loop;
        audioSource.spatialBlend = 0f; // 2D audio
        audioSource.volume = volumen;
        audioSource.priority = 0; // Alta prioridad
        audioSource.playOnAwake = false;
    }

    public void ReproducirMusica()
    {
        if (clipAmbiente != null && !audioSource.isPlaying)
        {
            audioSource.clip = clipAmbiente;
            audioSource.Play();
            Debug.Log("🎵 Reproduciendo música de ambiente: " + clipAmbiente.name);
        }
    }

    public void DetenerMusica()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("🔇 Música de ambiente detenida");
        }
    }

    public void CambiarVolumen(float nuevoVolumen)
    {
        volumen = Mathf.Clamp01(nuevoVolumen);
        audioSource.volume = volumen;
    }
}