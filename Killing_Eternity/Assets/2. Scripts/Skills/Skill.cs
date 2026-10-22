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
    public float baseMeleeRange = 2f;
    public float baseCooldown = 0f;
    public float baseDuration = 0f;
    public float projectileSpeed = 20f;

    [Header("Projectile Mechanics")]
    public int basePierceCount = 0;
    public int baseForkCount = 0;
    public int baseChainCount = 0;
    public int baseAdditionalProjectiles = 0;

    [Header("AoE Parameters")]
    public float baseAoeRadius = 0f;
    public float baseAoeDamageMult = 0.5f;

    [Header("Critical & Status")]
    public float baseCritChance = 0.05f; // 5% base crit
    public float baseCritDamage = 1.5f;  // 150% base multiplier
    public float baseStatusMod = 1.0f;

    [Header("Implementation Parts")]
    public string skillAnimation;
    public GameObject skillPrefab;

}
