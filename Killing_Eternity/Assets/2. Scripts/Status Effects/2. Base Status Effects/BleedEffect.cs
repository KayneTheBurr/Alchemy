using System.Net;
using UnityEngine;

namespace KE
{
    public class BleedEffect : DamageOverTimeEffect
    {
        private StatModifier stoneVulnerability;
        private StatModifier bloodVulnerability;
        private StatModifier bileVulnerability;
        private GameObject activeVFX;

        public BleedEffect()
        {
            statusType = Status.Bleed;
            duration = AlchemyEngine.instance.tuningData.bleedDuration;
            dps = 0;
            damageElement = Element.Blood;

            stoneVulnerability = new StatModifier(AlchemyEngine.instance.tuningData.bleedDamageTakenDebuffMultiplier, StatModType.PercentAdd, this);
            bloodVulnerability = new StatModifier(AlchemyEngine.instance.tuningData.bleedDamageTakenDebuffMultiplier, StatModType.PercentAdd, this);
            bileVulnerability = new StatModifier(AlchemyEngine.instance.tuningData.bleedDamageTakenDebuffMultiplier, StatModType.PercentAdd, this);
        }

        public override void ApplyEffect(IDamageable target, float dotAmount, IDamageable source)
        {
            this.dps = dotAmount * AlchemyEngine.instance.tuningData.bleedDamageRatio;
            base.ApplyEffect(target, dotAmount, source);
        }

        protected override void StartEffect()
        {
            base.StartEffect();

            CharacterManager charTarget = target.GetTransform().GetComponent<CharacterManager>();
            if (charTarget != null && charTarget.characterStatsManager != null)
            {
                charTarget.characterStatsManager.stoneDamageTakenMult.AddModifier(stoneVulnerability);
                charTarget.characterStatsManager.bloodDamageTakenMult.AddModifier(bloodVulnerability);
                charTarget.characterStatsManager.bileDamageTakenMult.AddModifier(bileVulnerability);
            }
            DamageableEnv envObj = target.GetTransform().GetComponent<DamageableEnv>();
            GameObject vfxPrefab = AlchemyEngine.instance.tuningData.bleedVFXPrefab;
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

            CharacterManager charTarget = target.GetTransform().GetComponent<CharacterManager>();
            if (charTarget != null && charTarget.characterStatsManager != null)
            {
                charTarget.characterStatsManager.stoneDamageTakenMult.RemoveAllModifiersFromSource(this);
                charTarget.characterStatsManager.bloodDamageTakenMult.RemoveAllModifiersFromSource(this);
                charTarget.characterStatsManager.bileDamageTakenMult.RemoveAllModifiersFromSource(this);
            }

            if (activeVFX != null) Object.Destroy(activeVFX);
        }
    }
}