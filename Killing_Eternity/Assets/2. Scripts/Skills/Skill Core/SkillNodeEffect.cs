using KE;
using UnityEngine;

[System.Serializable]
public class SkillNodeEffect
{
    public NodeScope scope;
    public NodeEffectType effectType = NodeEffectType.None;

    [Header("Stat Modifier Values")]
    public StatModType modType = StatModType.None;
    public StatType statType = StatType.None;
    public float modifierValue = 1.0f;

    [Header("Mechanic Altering Affects")]
    public string mechanicID;

    [Header("Overrides & Conversions")]
    public Element newElement; // if the node changes the skills element or adds an element
    public string newAnimationString; // if the modification needs a new aniamtion
    public GameObject newPrefab; // if the mod uses a completely different vfx
    public SkillTag tagModifier = SkillTag.None;
    public ResourceType newSkillResource = ResourceType.None;

}
