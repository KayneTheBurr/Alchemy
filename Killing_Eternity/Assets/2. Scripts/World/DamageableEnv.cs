using KE;
using UnityEngine;

public class DamageableEnv : MonoBehaviour, IDamageable
{
    
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    public Faction faction = Faction.Neutral;
    public Faction MyFaction => faction;
    public void TakeDamage(float damage, Element element, IDamageable damageSource = null)
    {
        Destroy(gameObject);
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public bool IsDead()
    {
        return currentHealth < 0;
    }
    public void ApplyBuildup(Element element, float buildupAmount, float dotAmount, IDamageable source = null)
    {

    }
}