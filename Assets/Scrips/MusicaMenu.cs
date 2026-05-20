using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicaMenu : MonoBehaviour
{
    [Header("Clip de música")]
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volumen = 0.5f;

    void Start()
    {
        AudioSource src = GetComponent<AudioSource>();
        src.loop         = true;
        src.spatialBlend = 0f;
        src.volume       = volumen;

        if (clip != null)
        {
            src.clip = clip;
            src.Play();
        }
        else
        {
            Debug.LogWarning("[MusicaMenu] No hay clip asignado en " + gameObject.name);
        }
    }
}
