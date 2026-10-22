using UnityEngine;

namespace KE
{
    public class FireballExecution : SkillExecution
    {
        [Header("Elemental VFX Variations")]
        public GameObject fireExplosionVFX;
        public GameObject iceExplosionVFX;

        private Vector3 previousPosition;
        private bool hasDetonated = false;
        private float projectileRadius;

        public override void Initialize(ActiveSkillInstance activeSkill, CharacterManager character)
        {
            base.Initialize(activeSkill, character);

            previousPosition = transform.position;

            //sphere collider radius determines size of sphere cast 
            SphereCollider sc = GetComponent<SphereCollider>();
            if(sc != null)
            {
                projectileRadius = sc.radius * Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = character.transform.forward * activeSkill.activeProjectileSpeed.Value;
            }
            Destroy(gameObject, 5f);
        }

        private void FixedUpdate()
        {
            if (hasDetonated) return;

            //distance from where we are to where we were
            Vector3 direction = transform.position - previousPosition;
            float distanceThisFrame = direction.magnitude;

            if(distanceThisFrame > 0)
            {
                //raycast between the two positions
                if (Physics.SphereCast(previousPosition, projectileRadius, direction.normalized, out RaycastHit hit, distanceThisFrame))
                {
                    // Ignore the caster and trigger volumes
                    if (hit.collider.gameObject != character.gameObject && !hit.collider.isTrigger)
                    {
                        // we hit something, then trigger the effect
                        HandleImpact(hit.collider, hit.point);
                        return;
                    }
                }
            }
            
            // update for next frame
            previousPosition = transform.position;
        }
        private void OnTriggerEnter(Collider other)
        {
            //OnTriggerEnter can catch and things that the sphere cast might miss, so the skill *should* never phase through things
            if (other.gameObject == character.gameObject || other.isTrigger) return;

            Vector3 impactPoint = other.ClosestPoint(transform.position);
            HandleImpact(other, impactPoint);
        }

        private void HandleImpact(Collider other, Vector3 impactPoint)
        {
            if (hasDetonated) return;
            hasDetonated = true;

            IDamageable target = other.GetComponent<IDamageable>();
            float damageDone = 0;
            if (target != null)
            {
                damageDone = skillData.GetCalculatedDamage(false);
                target.TakeDamage(damageDone, skillData.activeElement);
            }

            damageDone = skillData.GetCalculatedDamage(false);
            Debug.Log($"Dealt {damageDone} {skillData.activeElement} damage!");

            DetonateFireball(target, impactPoint);
            Destroy(gameObject);
        }
        


        public void DetonateFireball(IDamageable excludeTarget, Vector3 detonationPoint)
        {
            float explosionRadius = skillData.activeAoeRadius.Value;

            GameObject selectedExplosionVFX = fireExplosionVFX; // Default
            switch (skillData.activeElement)
            {
                case Element.Ice: selectedExplosionVFX = iceExplosionVFX; break;
            }

            if (selectedExplosionVFX != null)
            {
                Vector3 vfxSpawnPos = detonationPoint;
                int groundLayerMask = LayerMask.GetMask("Default");

                if (Physics.Raycast(detonationPoint, Vector3.down, out RaycastHit hit, 10f, groundLayerMask))
                {
                    // Snap to the floor! Add a tiny vertical offset (0.05f) so decals don't Z-fight.
                    vfxSpawnPos = hit.point + new Vector3(0, 0.05f, 0);
                }

                GameObject spawnedVFX = Instantiate(selectedExplosionVFX, vfxSpawnPos, Quaternion.identity);
                Destroy(spawnedVFX, 2f);
            }

            Collider[] hitColliders = Physics.OverlapSphere(detonationPoint, explosionRadius);

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