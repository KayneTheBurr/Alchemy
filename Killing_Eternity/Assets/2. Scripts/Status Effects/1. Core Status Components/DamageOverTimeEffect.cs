using UnityEngine;
using KE;

public class DamageOverTimeEffect : StatusEffect
{
    public float dps = 0f;
    public Element damageElement;

    public override void UpdateEffect(float timeDelta)
    {
        base.UpdateEffect(timeDelta);
        if (isFinished) return;

        float frameDamage = dps * timeDelta;

        target.TakeDamage(frameDamage, damageElement, source, isDoT: true);
    }
}
