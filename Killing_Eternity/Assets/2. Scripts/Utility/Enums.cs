using UnityEngine;

namespace KE
{
    public class Enums { }

    public enum StatModType
    {
        FlatAdd,      // flat amount change
        PercentAdd,   // Additive values
        PercentMult,   // multiplicitive values
        None
    }

    public enum ResourceType
    {
        None,
        Health,
        Mana,
        Stamina,
        Shield
    }

    public enum NodeEffectType
    {
        None,
        StatModifier,
        UnlockSecondarySlot,
        Capstone,
        ElementalConversion,
        AddTag, RemoveTag,
        PrefabOverride, AnimationOverride,
        ResourceConversion
    }

    public enum NodeScope
    {
        Global, SkillSocket
    }

    public enum StatType
    {
        None,
        Damage, BloodDamage, BileDamage, FireDamage, IceDamage, SparkDamage, StoneDamage,
        ProjectileSpeed, ProjectilePierce, ProjectileFork, ProjectileChain, ExtraProjectile

    }
    public enum Element
    {
        Blood, Bile, Fire, Ice, Spark, Stone
    }
    public enum SkillTag
    {
        None, Spell, Attack, Melee, Projectile, AOE, Nova,
        Duration, Cooldown, Buff, Debuff, DOT, Channeled, ConditionalTrigger
    }
    public enum SkillSpawnLocation { CasterRightWeapon, CasterLeftWeapon, CasterFeet, TargetEnemy, CursorLocation }

    public enum Faction 
    { 
        Player, 
        Enemy,
        Ally, 
        Neutral 
    }
}