using UnityEngine;
using KE;

public class BurnEffect : DamageOverTimeEffect
{
    public BurnEffect()
    {
        statusType = Status.Burn;
        duration = 5f;
        tickInterval = 1f;
        tickDamage = 15f;
        damageElement = Element.Fire;
    }

    protected override void StartEffect()
    {
        base.StartEffect();
        // Instantiate Fire VFX on target.GetTransform()
    }

    public override void RemoveEffect()
    {
        base.RemoveEffect();
        // Destroy Fire VFX
    }
}
