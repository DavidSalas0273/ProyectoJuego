using UnityEngine;

public class Camara : MonoBehaviour
{
    public Transform objetivo;

    public float distancia = 4f;
    public float altura = 2f;
    public float suavizado = 5f;

    public LayerMask capaPiso;
    public float alturaMinimaSobrePiso = 0.3f;

    void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 posicionDeseada = objetivo.position
                                - objetivo.forward * distancia
                                + Vector3.up * altura;

        // Detectar el piso debajo de la posición deseada
        RaycastHit hit;
        if (Physics.Raycast(posicionDeseada + Vector3.up * 5f, Vector3.down, out hit, 20f, capaPiso))
        {
            float alturaDelPiso = hit.point.y + alturaMinimaSobrePiso;

            if (posicionDeseada.y < alturaDelPiso)
            {
                posicionDeseada.y = alturaDelPiso;
            }
        }

        transform.position = Vector3.Lerp(
            transform.position,
            posicionDeseada,
            suavizado * Time.deltaTime
        );

        transform.LookAt(objetivo.position + Vector3.up * 1.5f);
    }
}