using UnityEngine;

[CreateAssetMenu(menuName = "Alchemy Tuning Data")]
public class AlchemyTuningData : ScriptableObject
{
    [Header("Base Status Effects")]
    [Header("Bleed")]

    [Header("Toxin")]

    [Header("Burn")]
    public float burnDuration = 3f;
    public float burnDamageRatio = 0.30f;
    public GameObject burnVFXPrefab;

    [Header("Chill")]
    public float chillDuration = 6f;
    public float chillAmount = 0.3f;
    public GameObject chillVFXPrefab;
    
    [Header("Shock")]

    [Header("Sunder")]

    [Header(" Tier 2 Effects")]

    [Header("Thermal Shock (Burn + Chill")]
    [Tooltip("Flat base Stone damage dealt by the Thermal Shock shatter")]
    public float thermalShockBaseDamage = 100f;
    [Tooltip("Radius of the shattering burst")]
    public float thermalShockRadius = 2f;
    public GameObject thermalShockVFX;

    [Header("Wet (Chill + Burn)")]
    public float wetDuration = 8f;
    public float wetSparkVulnerability = 0.3f;
    public float wetIceVulnerability = 0.3f;
    public float wetFireMitigation = -0.3f;
    public GameObject wetVFXPrefab;



    [Header("Tier 3 Effects")]

    [Header("Frozen (Wet + Chill")]
    public float frozenDuration = 4f;
    public GameObject frozenVFXPrefab;
}
