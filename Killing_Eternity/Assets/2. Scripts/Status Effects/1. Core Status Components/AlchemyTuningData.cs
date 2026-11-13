using UnityEngine;

[CreateAssetMenu(menuName = "Alchemy Tuning Data")]
public class AlchemyTuningData : ScriptableObject
{
    [Header("Core Elemental Values")]
    public float bileShieldDamageMultiplier = 2.0f;


    [Header("Base Status Effects")]
    [Header("Bleed")]
    public float bleedDuration = 5f;
    public float bleedDamageRatio = 0.1f;
    public float bleedDamageTakenDebuffMultiplier = 0.15f;
    public GameObject bleedVFXPrefab;

    [Header("Toxin")]
    public float toxinDuration = 5f;
    public float toxinDamageRatio = 0.1f;
    public GameObject toxinVFXPrefab;

    [Header("Burn")]
    public float burnDuration = 3f;
    public float burnDamageRatio = 0.30f;
    public float burnDamageBonus = 0.1f;
    public GameObject burnVFXPrefab;

    [Header("Chill")]
    public float chillDuration = 6f;
    public float chillAmount = 0.3f;
    public GameObject chillVFXPrefab;

    [Header("Shock")]
    public float shockDuration = 5f;
    public GameObject shockVFXPrefab;

    [Header("Sunder")]
    public float sunderDuration = 12f;
    public float sunderArmourBreakMult = 0.2f;
    public GameObject sunderVFXPrefab;

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
    public Material frozenMaterial;
}
