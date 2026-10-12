using UnityEngine;

namespace KE
{
    public class FireballExecution : SkillExecution
    {
        [Header("Elemental VFX Variations")]
        public GameObject defaultFireVFX;
        public GameObject iceConversionVFX;
        public GameObject bloodConversionVFX;
        public GameObject explosionVFX;

        public override void Initialize(ActiveSkillInstance activeSkill, CharacterManager character)
        {
            base.Initialize(activeSkill, character);

            // Turn off everything first
            defaultFireVFX.SetActive(false);
            iceConversionVFX.SetActive(false);
            bloodConversionVFX.SetActive(false);

            switch (skillData.activeElement)
            {
                case Element.Fire:
                    defaultFireVFX.SetActive(true);
                    //anything else that ONLY fire does 
                    break;
                case Element.Ice:
                    iceConversionVFX.SetActive(true);
                    // things only ICE does
                    break;
                case Element.Blood:
                    bloodConversionVFX.SetActive(true);
                    //things only blood does
                    break;
            }
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = character.transform.forward * activeSkill.activeProjectileSpeed.Value;
            }
            Destroy(gameObject, 5f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == character.gameObject || other.isTrigger) return;
            // add any other layers the fireball shouldnt hit here

            IDamageable target = other.GetComponent<IDamageable>();

            if (target != null)
            {
                float finalDamage = skillData.baseSkill.baseDamage * skillData.activeDamageMultiplier.Value;

                target.TakeDamage(finalDamage, skillData.activeElement);

                //check for pierce, fork, chain, etc here
                //else
                //trigger the fireball detonation once it hits a target here 
                DetonateFireball(target);
                Destroy(gameObject);
            }
            //fireball should actually detonate if it hits a wall too
            Destroy(gameObject);
        }

        public void DetonateFireball(IDamageable excludeTarget)
        {
            float explosionRadius = skillData.activeAoeRadius.Value;

            if (explosionVFX != null)
            {
                Instantiate(explosionVFX, transform.position, Quaternion.identity);
            }

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

            foreach (Collider hit in hitColliders)
            {
                if (hit.gameObject == character.gameObject) continue;

                IDamageable splashTarget = hit.GetComponent<IDamageable>();

                //must be a damageable target and not the one directly hit by the skill 
                if (splashTarget != null && splashTarget != excludeTarget)
                {
                    // calc base damage with multipliers, then apply the aoe multiplier to the base damage
                    float baseCalc = skillData.baseSkill.baseDamage * skillData.activeDamageMultiplier.Value;
                    float splashDamage = baseCalc * skillData.activeAoeDamageMultiplier.Value;

                    splashTarget.TakeDamage(splashDamage, skillData.activeElement);
                }
            }
        }
    }
}