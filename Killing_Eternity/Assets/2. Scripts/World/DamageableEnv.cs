using KE;
using UnityEngine;

public class DamageableEnv : MonoBehaviour, IDamageable
{
    public void TakeDamage(float damage, Element element)
    {
        Destroy(gameObject);
    }

    public Transform GetTransform()
    {
        return transform;
    }
}