using UnityEngine;
using KE;
using Unity.VisualScripting;

public class BurnEffect : DamageOverTimeEffect
{
    private StatModifier burnDamageModifier;
    GameObject activeVFX;
    public BurnEffect()
    {
        statusType = Status.Burn;
        duration = AlchemyEngine.instance.tuningData.burnDuration;
        dps = 0;
        damageElement = Element.Fire;
        burnDamageModifier = new StatModifier(0.1f, StatModType.PercentMult, this);
    }

    public override void ApplyEffect(IDamageable target, float dotAmount, IDamageable source)
    {
        this.dps = dotAmount * AlchemyEngine.instance.tuningData.burnDamageRatio;

        base.ApplyEffect(target, dotAmount, source);
    }

    protected override void StartEffect()
    {
        base.StartEffect();

        CharacterManager charTarget = target.GetTransform().GetComponent<CharacterManager>();
        if (charTarget != null && charTarget.characterStatsManager != null)
        {
            charTarget.characterStatsManager.allDamageModifier.AddModifier(burnDamageModifier);
        }

        // Instantiate Fire VFX on target.GetTransform()
        GameObject vfxPrefab = AlchemyEngine.instance.tuningData.burnVFXPrefab;
        if (vfxPrefab != null)
        {
            activeVFX = Object.Instantiate(vfxPrefab, target.GetTransform());
            if(charTarget != null)
            {
                activeVFX.GetComponent<OverlayFX>().targetRenderer = charTarget.mainMesh;
                activeVFX.GetComponent<OverlayFX>().InitializeWithMesh();
            }
            
        }
    }

    public override void RemoveEffect()
    {
        base.RemoveEffect();

        CharacterManager charTarget = target.GetTransform().GetComponent<CharacterManager>();
        if (charTarget != null && charTarget.characterStatsManager != null)
        {
            charTarget.characterStatsManager.allDamageModifier.RemoveAllModifiersFromSource(this);
        }

        // Destroy Fire VFX
        Object.Destroy(activeVFX);
    }
}
