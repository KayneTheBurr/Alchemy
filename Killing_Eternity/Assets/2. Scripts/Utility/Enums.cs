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
        Health,
        Mana,
        Stamina,
        Shield
    }

    public enum NodeEffectType
    {
        StatModifier,
        UnlockSecondarySlot,
        CapstoneMechanic
    }

    public enum NodeScope
    {
        Global, SkillSocket
    }

    public enum StatType
    {
        None,
        Damage
    }
    public enum Element
    {
        Blood, Bile, Fire, Ice, Spark, Stone
    }
    public enum SkillTag
    {
        Spell, Attack, Melee, Projectile, AoE, Nova,
        Duration, Cooldown, Buff, Debuff, DoT, Channeled, ConditionalTrigger
    }
    public enum SkillSpawnLocation { CasterWeapon, CasterFeet, TargetEnemy, CursorLocation }


}