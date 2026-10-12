using KE;
using UnityEngine;

[System.Serializable]
public class SkillNodeEffect
{
    public NodeScope scope;
    public NodeEffectType effectType;
    public GameObject overridePrefab;

    [Header("Stat Modifier Values")]
    public StatModType modType = StatModType.None;
    public StatType statType = StatType.None; //Enum needs to contain every possible stat type in the game, very very long enum
    public float modifierValue = 1.0f;

    [Header("Mechanic Altering Affects")]
    public string mechanicID;

}
