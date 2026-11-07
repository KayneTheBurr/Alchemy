using UnityEngine;
using KE;
public class WetEffect : StatusEffect
{
    private GameObject activeVFX;

    public WetEffect()
    {
        statusType = Status.Wet;
        duration = AlchemyEngine.instance.tuningData.wetDuration;
    }

    protected override void StartEffect()
    {
        base.StartEffect();
        CharacterManager charManager = target.GetTransform().GetComponent<CharacterManager>();
        // Spawn VFX
        GameObject vfxPrefab = AlchemyEngine.instance.tuningData.wetVFXPrefab;
        if (vfxPrefab != null)
        {
            activeVFX = Object.Instantiate(vfxPrefab, target.GetTransform());
            if (charManager != null)
            {
                activeVFX.GetComponent<OverlayFX>().targetRenderer = charManager.mainMesh;
                activeVFX.GetComponent<OverlayFX>().InitializeWithMesh();
            }
            // connect to overlay FX script automatically?
        }

        float sparkVulnerability = AlchemyEngine.instance.tuningData.wetSparkVulnerability;
        float iceVulnerability = AlchemyEngine.instance.tuningData.wetIceVulnerability;
        float fireMitigation = AlchemyEngine.instance.tuningData.wetFireMitigation;

        // appky stat modifiers to character class
        
        if (charManager != null)
        {
            charManager.characterStatsManager.sparkDamageTakenMult.AddModifier(new StatModifier(sparkVulnerability, StatModType.PercentAdd, this));
            charManager.characterStatsManager.iceDamageTakenMult.AddModifier(new StatModifier(iceVulnerability, StatModType.PercentAdd, this));
            charManager.characterStatsManager.fireDamageTakenMult.AddModifier(new StatModifier(fireMitigation, StatModType.PercentAdd, this));

            charManager.characterStatsManager.sparkThreshold.AddModifier(new StatModifier(-sparkVulnerability, StatModType.PercentAdd, this));
            charManager.characterStatsManager.iceThreshold.AddModifier(new StatModifier(-iceVulnerability, StatModType.PercentAdd, this));
            charManager.characterStatsManager.fireThreshold.AddModifier(new StatModifier(-fireMitigation, StatModType.PercentAdd, this));

            charManager.characterStatsManager.SyncStatusThresholds();
        }
        else
        {
            // do things to the NON character class here
            StatusManager statusManager = target.GetTransform().GetComponent<StatusManager>();
            if (statusManager != null)
            {
                statusManager.shockThreshold.AddModifier(new StatModifier(-sparkVulnerability, StatModType.PercentAdd, this));
                statusManager.chillThreshold.AddModifier(new StatModifier(-iceVulnerability, StatModType.PercentAdd, this));
                statusManager.burnThreshold.AddModifier(new StatModifier(-fireMitigation, StatModType.PercentAdd, this));
            }
        }
    }

    public override void RemoveEffect()
    {
        base.RemoveEffect();

        if (activeVFX != null) Object.Destroy(activeVFX);

        CharacterManager charManager = target.GetTransform().GetComponent<CharacterManager>();
        if (charManager != null)
        {
            // Strip all modifiers applied by this specific effect instance
            charManager.characterStatsManager.sparkDamageTakenMult.RemoveAllModifiersFromSource(this);
            charManager.characterStatsManager.iceDamageTakenMult.RemoveAllModifiersFromSource(this);
            charManager.characterStatsManager.fireDamageTakenMult.RemoveAllModifiersFromSource(this);

            charManager.characterStatsManager.sparkThreshold.RemoveAllModifiersFromSource(this);
            charManager.characterStatsManager.iceThreshold.RemoveAllModifiersFromSource(this);
            charManager.characterStatsManager.fireThreshold.RemoveAllModifiersFromSource(this);

            charManager.characterStatsManager.SyncStatusThresholds();
        }
        else
        {
            StatusManager statusManager = target.GetTransform().GetComponent<StatusManager>();
            if (statusManager != null)
            {
                statusManager.shockThreshold.RemoveAllModifiersFromSource(this);
                statusManager.chillThreshold.RemoveAllModifiersFromSource(this);
                statusManager.burnThreshold.RemoveAllModifiersFromSource(this);
            }
        }
    }
}
