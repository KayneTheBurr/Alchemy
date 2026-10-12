using KE;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill / Core Skill")]
public class Skill : ScriptableObject
{
    [Header("Skill Info")]
    public string skillName;
    [TextArea] public string skillDescription;
    public int skillID;
    public Sprite skillIcon;

    [Header("Element and Tags")]
    public Element skillElement = Element.Blood;
    public List<SkillTag> skillTags;
    public SkillSpawnLocation spawnLocation;

    [Header("Skill Costs")]
    public int baseSkillCost = 10;
    public ResourceType baseResourceType = ResourceType.Mana;

    [Header("Combat Data ( 0 if unused")]
    public float baseDamage = 10f;
    public float baseCooldown = 0f;
    public float baseAoeRadius = 0f;
    public float baseAoeDamageMult = 0.5f;
    public float baseDuration = 0f;
    public float projectileSpeed = 20f;

    [Header("Implementation Parts")]
    public string skillAnimation;
    public GameObject skillPrefab;

}
