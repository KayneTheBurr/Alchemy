using KE;
using UnityEngine;
using UnityEngine.AI;

public class DamageableEnv : MonoBehaviour, IDamageable
{
    [HideInInspector] public StatusManager statusManager;



    [Header("Health")]
    public float maxHealth = 100f;
    public ObservableVariable<float> currentHealth = new ObservableVariable<float> (100f);


    [Header("Core Data")]
    public string objectName = "Damageable Object"; 
    public Faction faction = Faction.Neutral;
    public Faction MyFaction => faction;

    [Header("Elemental Resistances (% mitigation of damage of that element")]
    public float bloodDefense = 0f;
    public float bileDefense = 0f;
    public float fireDefense = 0f;
    public float iceDefense = 0f;
    public float sparkDefense = 0f;
    public float stoneDefense = 0f;

    private void Awake()
    {
        statusManager = GetComponent<StatusManager>();

        currentHealth.Value = maxHealth;
    }
    public void TakeDamage(float damageValue, Element element, IDamageable damageSource = null, bool isDoT = false)
    {
        float rawResistance = GetResistance(element);
        float effectiveResistance = Mathf.Min(rawResistance, 100f);

        float damageMultiplier = 1f - (effectiveResistance / 100f);
        float finalDamage = Mathf.Max(damageValue * damageMultiplier, 0f);

        currentHealth.Value -= finalDamage;

        if (currentHealth.Value <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public bool IsDead()
    {
        return currentHealth.Value <= 0;
    }
    public void ApplyBuildup(Element element, float buildupAmount, float dotAmount, IDamageable source = null)
    {
        statusManager.AddBuildup(element, buildupAmount, dotAmount, source);
    }

    public float GetResistance(Element element)
    {
        switch (element)
        {
            case Element.Blood: return bloodDefense;
            case Element.Bile: return bileDefense;
            case Element.Fire: return fireDefense;
            case Element.Ice: return iceDefense;
            case Element.Spark: return sparkDefense;
            case Element.Stone: return stoneDefense;
            default: return 0f;
        }
    }
}