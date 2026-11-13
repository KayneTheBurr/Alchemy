using UnityEngine;

namespace KE
{
    public class ToxinEffect : DamageOverTimeEffect
    {
        private GameObject activeVFX;
        public ToxinEffect()
        {
            statusType = Status.Toxin;
            duration = AlchemyEngine.instance.tuningData.toxinDuration;
            dps = 0;
            damageElement = Element.Bile;
        }
        public override void ApplyEffect(IDamageable target, float dotAmount, IDamageable source)
        {
            this.dps = dotAmount * AlchemyEngine.instance.tuningData.toxinDamageRatio; ;
            base.ApplyEffect(target, dotAmount, source);
        }

        protected override void StartEffect()
        {
            base.StartEffect();

            CharacterManager charTarget = target.GetTransform().GetComponent<CharacterManager>();
            DamageableEnv envObj = target.GetTransform().GetComponent<DamageableEnv>();

            GameObject vfxPrefab = AlchemyEngine.instance.tuningData.toxinVFXPrefab;
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
        }
    }
}