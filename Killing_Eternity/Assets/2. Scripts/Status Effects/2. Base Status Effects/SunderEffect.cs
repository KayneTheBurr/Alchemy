using UnityEngine;

namespace KE
{
    public class SunderEffect : StatusEffect
    {
        private GameObject activeVFX;
        private StatModifier armorBreakModifier;

        // TODO: Add Stagger/Stun buildup modifier when the

        public SunderEffect()
        {
            statusType = Status.Sunder;
            duration = AlchemyEngine.instance.tuningData.sunderDuration;

            armorBreakModifier = new StatModifier(-AlchemyEngine.instance.tuningData.sunderArmourBreakMult, StatModType.PercentAdd, this);
        }

        protected override void StartEffect()
        {
            base.StartEffect();

            Debug.Log(" Triggered Sunder Effect!");

            CharacterManager charManager = target.GetTransform().GetComponent<CharacterManager>();
            if (charManager != null && charManager.characterStatsManager != null)
            {
                charManager.characterStatsManager.armourAmount.AddModifier(armorBreakModifier);
            }
            DamageableEnv envObj = target.GetTransform().GetComponent<DamageableEnv>();
            GameObject vfxPrefab = AlchemyEngine.instance.tuningData.sunderVFXPrefab;
            if (vfxPrefab != null)
            {
                activeVFX = Object.Instantiate(vfxPrefab);
                if (charManager != null)
                {
                    activeVFX.GetComponent<OverlayFX>().targetRenderer = charManager.mainMesh;
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

            CharacterManager charManager = target.GetTransform().GetComponent<CharacterManager>();
            if (charManager != null && charManager.characterStatsManager != null)
            {
                charManager.characterStatsManager.armourAmount.RemoveAllModifiersFromSource(this);
            }

            if (activeVFX != null) Object.Destroy(activeVFX);
        }
    }
}