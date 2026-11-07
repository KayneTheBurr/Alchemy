using UnityEngine;
using KE;

public class ThermalShockEffect : StatusEffect
{
    private float burstRadius;
    private float burstDamage;

    public ThermalShockEffect()
    {
        statusType = Status.ThermalShock;
    }

    public override void ApplyEffect(IDamageable target, float dotAmount, IDamageable source)
    {
        burstRadius = AlchemyEngine.instance.tuningData.thermalShockRadius;
        burstDamage = AlchemyEngine.instance.tuningData.thermalShockBaseDamage;

        // does anything scale up thermal shock damage?

        base.ApplyEffect(target, dotAmount, source);
    }

    protected override void StartEffect()
    {
        base.StartEffect();

        // only hit things taht can take damage
        LayerMask targetLayers = WorldUtilityManager.instance.GetDamageLayers();

        // trigger shattering burst in aoe radius
        Collider[] hitColliders = Physics.OverlapSphere(
            target.GetTransform().position, burstRadius, targetLayers);

        foreach (Collider hit in hitColliders)
        {
            IDamageable splashTarget = hit.GetComponent<IDamageable>();

            // Ensure we don't hit ourselves, dead targets, or allies (if friendly fire is off)
            if (splashTarget != null && !splashTarget.IsDead())
            {
                if (WorldUtilityManager.instance.CanIDamageThisTarget(source.MyFaction, splashTarget.MyFaction))
                {
                    splashTarget.TakeDamage(burstDamage, Element.Stone, source, isDoT: false);
                }

            }
        }

        // pkay shatter vfx / sfx here
        GameObject vfxPrefab = AlchemyEngine.instance.tuningData.thermalShockVFX;
        if (vfxPrefab != null)
        {
            GameObject spawnedVFX = Object.Instantiate(vfxPrefab, target.GetTransform().position, Quaternion.identity);

            // 2 meter radius = about 0.75 scale -> 0.375 multiplier
            float scaleFactor = 0.375f * burstRadius;
            spawnedVFX.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            Object.Destroy(spawnedVFX, 4f);
        }

        //finish right away, no going to the update loop 
        isFinished = true;
    }
}
