using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

public class Portal : MonoBehaviour
{
    [Header("Escena destino")]
    public string escenaDestino;

    [Header("Fallback sin Rigidbody")]
    public bool usarDeteccionPorDistancia = false;
    public float radioDeteccion = 1f;
    public string tagJugador = "Player";

    #if UNITY_EDITOR
    public SceneAsset escenaDestinoAsset;

    private void OnValidate()
    {
        if (escenaDestinoAsset != null)
        {
            string path = AssetDatabase.GetAssetPath(escenaDestinoAsset);
            if (!string.IsNullOrEmpty(path) && path.EndsWith(".unity"))
            {
                escenaDestino = Path.GetFileNameWithoutExtension(path);
            }
        }
    }
    #endif

    private void Update()
    {
        if (usarDeteccionPorDistancia)
        {
            DetectarJugadorPorDistancia();
        }
    }

    private void DetectarJugadorPorDistancia()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radioDeteccion);
        foreach (var hit in hits)
        {
            if (hit.CompareTag(tagJugador) || hit.transform.root.CompareTag(tagJugador))
            {
                Teletransportar();
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagJugador) || other.transform.root.CompareTag(tagJugador))
        {
            Teletransportar();
        }
    }

    private void Teletransportar()
    {
        if (string.IsNullOrEmpty(escenaDestino))
        {
            Debug.LogWarning("Portal: escenaDestino no está configurada.");
            return;
        }

        if (GameManager.instancia != null)
        {
            GameManager.instancia.CargarEscena(escenaDestino);
        }
        else
        {
            SceneManager.LoadScene(escenaDestino);
        }
    }
}