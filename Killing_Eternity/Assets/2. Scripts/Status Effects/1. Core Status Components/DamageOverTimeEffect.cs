using UnityEngine;
using KE;

public class DamageOverTimeEffect : StatusEffect
{
    public float tickInterval = 1f;
    public float tickDamage = 10f;
    public Element damageElement;

    private float tickTimer = 0f;

    public override void ApplyEffect(IDamageable target, float dotAmount, IDamageable source)
    {
        this.tickDamage = dotAmount;

        base.ApplyEffect(target, dotAmount, source);
    }
    public override void UpdateEffect(float timeDelta)
    {
        base.UpdateEffect(timeDelta);
        if (isFinished) return;

        tickTimer += timeDelta;
        if (tickTimer >= tickInterval)
        {
            tickTimer = 0f;
            target.TakeDamage(tickDamage, damageElement, source);
        }
    }
}
