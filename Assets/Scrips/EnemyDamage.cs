using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public GameObject hitbox;

    public void ActivarHitbox()
    {
        Debug.Log("🟥 ENEMIGO ATACA");
        hitbox.SetActive(true);
    }

    public void DesactivarHitbox()
    {
        hitbox.SetActive(false);
    }
}