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

        public override void Initialize(ActiveSkillInstance activeSkill, CharacterManager character, IDamageable target = null)
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
                RaycastHit[] hits = Physics.SphereCastAll(previousPosition, projectileRadius, direction.normalized, distanceThisFrame);

                //sort the things we hit by distance
                System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

                foreach (RaycastHit hit in hits)
                {
                    if (character.IsDead() || character == null) continue;

                    // ignore triggers and ourselves
                    if (hit.collider.gameObject == character.gameObject || hit.collider.isTrigger) continue;

                    IDamageable hitTarget = hit.collider.GetComponent<IDamageable>();
                    if (hitTarget != null)
                    {
                        // projectile will phase through allies
                        if (!WorldUtilityManager.instance.CanIDamageThisTarget(character.MyFaction, hitTarget.MyFaction))
                        {
                            continue;
                        }
                    }

                    HandleImpact(hit.collider, hit.point);

                    previousPosition = transform.position;
                    return;
                }
            }
            
            // update for next frame
            previousPosition = transform.position;
        }
        private void OnTriggerEnter(Collider other)
        {
            //OnTriggerEnter can catch and things that the sphere cast might miss, so the skill *should* never phase through things
            if (hasDetonated || other.gameObject == character.gameObject || other.isTrigger) return;

            IDamageable hitTarget = other.GetComponent<IDamageable>();
            if (hitTarget != null)
            {
                // projectile will phase through allies
                if (!WorldUtilityManager.instance.CanIDamageThisTarget(character.MyFaction, hitTarget.MyFaction))
                {
                    return;
                }
            }

            Vector3 impactPoint = other.ClosestPoint(transform.position);
            HandleImpact(other, impactPoint);
        }

        private void HandleImpact(Collider other, Vector3 impactPoint)
        {
            if (hasDetonated) return;
            hasDetonated = true;

            IDamageable target = other.GetComponent<IDamageable>();

            if (target != null)
            {
                ApplySkillDamageAndStatus(target, false);
            }

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
                    // if this can be converted to other elements, do it here
            }

            if (selectedExplosionVFX != null)
            {
                Vector3 vfxSpawnPos = detonationPoint;

                int groundLayerMask = WorldUtilityManager.instance.GetEnviroLayers();

                if (Physics.Raycast(detonationPoint, Vector3.down, out RaycastHit hit, 10f, groundLayerMask))
                {
                    // Snap to the floor, add a little vertical offset for vfx to stay not inside the ground
                    vfxSpawnPos = hit.point + new Vector3(0, 0.05f, 0);
                }

                GameObject spawnedVFX = Instantiate(selectedExplosionVFX, vfxSpawnPos, Quaternion.identity);
                Destroy(spawnedVFX, 2f);
            }

            Collider[] hitColliders = Physics.OverlapSphere(detonationPoint, explosionRadius, WorldUtilityManager.instance.GetDamageLayers());
            DrawDebugSphere(detonationPoint, explosionRadius, Color.red);

            foreach (Collider hit in hitColliders)
            {
                //ignore casting character 
                if (hit.gameObject == character.gameObject) continue;

                IDamageable splashTarget = hit.GetComponent<IDamageable>();

                //must be a damageable target and not the one directly hit by the skill 
                if (splashTarget != null && splashTarget != excludeTarget && !splashTarget.IsDead())
                {
                    if (WorldUtilityManager.instance.CanIDamageThisTarget(character.MyFaction, splashTarget.MyFaction))
                    {
                        ApplySkillDamageAndStatus(splashTarget, isAOESplash: true);
                        Debug.Log($"Dealt Splash damage to {splashTarget}");
                    }
                }
            }
        }
        public static void DrawDebugSphere(Vector3 center, float radius, Color color, int segments = 16)
        {
            float angleStep = 360f / segments;

            Vector3 prevX = Vector3.zero;
            Vector3 prevY = Vector3.zero;
            Vector3 prevZ = Vector3.zero;

            for (int i = 0; i <= segments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                float s = Mathf.Sin(angle) * radius;
                float c = Mathf.Cos(angle) * radius;

                Vector3 nextX = center + new Vector3(0, s, c);  // YZ plane ring
                Vector3 nextY = center + new Vector3(s, 0, c);  // XZ plane ring
                Vector3 nextZ = center + new Vector3(s, c, 0);  // XY plane ring

                if (i > 0)
                {
                    Debug.DrawLine(prevX, nextX, color);
                    Debug.DrawLine(prevY, nextY, color);
                    Debug.DrawLine(prevZ, nextZ, color);
                }

                prevX = nextX;
                prevY = nextY;
                prevZ = nextZ;
            }
        }
    }
}