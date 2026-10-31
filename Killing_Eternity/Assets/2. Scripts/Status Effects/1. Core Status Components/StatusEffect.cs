using KE;
using UnityEngine;

public abstract class StatusEffect
{
    [Header ("Core Stats")]
    public Status statusType;
    public float duration;
    public float elapsedTime;
    public bool isFinished = false;

    protected float payload;
    protected IDamageable target; // Who is suffering the effect
    protected IDamageable source; // Who caused the effect

    public virtual void ApplyEffect(IDamageable target, float dotAmount, IDamageable source)
    {
        this.target = target;
        this.source = source;
        this.elapsedTime = 0f;
        this.isFinished = false;
        this.payload = dotAmount;

        StartEffect();
    }
    public virtual void UpdateEffect(float timeDelta)
    {
        if (isFinished) return;

        elapsedTime += timeDelta;
        if (elapsedTime >= duration)
        {
            isFinished = true;
            RemoveEffect();
        }
    }
    protected virtual void StartEffect()
    {
        // play sfx / vfx
        // apply stat modifiers
        // deal immediate damage
    }
    public virtual void RemoveEffect()
    {
        // stop vfx
        // remove stat modifiers
    }
}
