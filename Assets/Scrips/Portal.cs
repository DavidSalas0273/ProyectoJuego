using UnityEngine;

public class Portal : MonoBehaviour
{
    public string escenaDestino;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instancia.CargarEscena(escenaDestino);
        }
    }
}