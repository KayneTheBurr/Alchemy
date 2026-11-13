using KE;
using UnityEngine;

public class ChillEffect : StatusEffect
{
    private GameObject activeVFX;

    public ChillEffect()
    {
        statusType = Status.Chill;
        duration = AlchemyEngine.instance.tuningData.chillDuration;
    }

    protected override void StartEffect()
    {
        CharacterManager charTarget = target.GetTransform().GetComponent<CharacterManager>();

        float slowPercentage = -1 * AlchemyEngine.instance.tuningData.chillAmount;

        if (charTarget != null && charTarget.characterStatsManager != null)
        {
            // Apply the debuff
            StatModifier actionSlow = new StatModifier(slowPercentage, StatModType.PercentAdd, this);
            StatModifier moveSlow = new StatModifier(slowPercentage, StatModType.PercentAdd, this);

            charTarget.characterStatsManager.actionSpeed.AddModifier(actionSlow);
            charTarget.characterStatsManager.moveSpeedMultiplier.AddModifier(moveSlow);
        }

        DamageableEnv envObj = target.GetTransform().GetComponent<DamageableEnv>();
        GameObject vfxPrefab = AlchemyEngine.instance.tuningData.chillVFXPrefab;
        if (vfxPrefab != null)
        {
            activeVFX = Object.Instantiate(vfxPrefab, target.GetTransform());
            if (charTarget != null)
            {
                activeVFX.GetComponent<OverlayFX>().targetRenderer = charTarget.mainMesh;
                activeVFX.GetComponent<OverlayFX>().InitializeWithMesh();
            }
            else if (envObj != null)
            {

                activeVFX.GetComponent<OverlayFX>().targetRenderer = envObj.mainMesh;
                activeVFX.GetComponent<OverlayFX>().InitializeWithMesh();
            }
        }
    }

    public override void RemoveEffect()
    {
        base.RemoveEffect();

        if (activeVFX != null) Object.Destroy(activeVFX);

        CharacterManager charTarget = target.GetTransform().GetComponent<CharacterManager>();
        if (charTarget != null && charTarget.characterStatsManager != null)
        {
            charTarget.characterStatsManager.actionSpeed.RemoveAllModifiersFromSource(this);
            charTarget.characterStatsManager.moveSpeedMultiplier.RemoveAllModifiersFromSource(this);
        }
    }
}
