using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KE { 
    public class SmiteExecution : SkillExecution
    {
        [Header("Elemental VFX Variations")]
        public GameObject fireImpactVFX;
        [SerializeField] private float detonateDelay = 0.25f;
        [SerializeField] private float vfxScale = 0.6f;

        Coroutine detonateCoroutine;

        // steps for execution:
        // 1. Spawn at target location
        // 2. Play telegraph VFX for short duration
        // 3. Detonate skill & split total damage among hit enemies 

        // change the AOE damage multiplier in skillData to the percentage of damage dealt to each target (1 / totalTargets) before calling ApplySkillDamageAndStatus

        public override void Initialize(ActiveSkillInstance activeSkill, CharacterManager character, IDamageable target = null)
        {
            base.Initialize(activeSkill, character);

            detonateCoroutine = StartCoroutine(DetonateAfterDelay());

            Destroy(gameObject, 5f);
        }

        IEnumerator DetonateAfterDelay()
        {
            // spawn telegraph VFX
            GameObject selectedImpactVFX = fireImpactVFX; // Default
            if (selectedImpactVFX != null)
            {
                Vector3 vfxSpawnPos = transform.position;

                // this should likely be included in the world utility manager now instead of hard coded
                int groundLayerMask = WorldUtilityManager.instance.GetEnviroLayers();

                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f, groundLayerMask))
                {
                    // Snap to the floor, add a little vertical offset for vfx to stay not inside the ground
                    vfxSpawnPos = hit.point + new Vector3(0, 0.05f, 0);
                }

                GameObject spawnedVFX = Instantiate(selectedImpactVFX, vfxSpawnPos, Quaternion.identity);
                spawnedVFX.transform.localScale = new Vector3(vfxScale, vfxScale, vfxScale);
                Destroy(spawnedVFX, 5f);
            }

            yield return new WaitForSeconds(detonateDelay);
            
            Detonate();
        }

        void Detonate()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, skillData.activeAoeRadius.Value, WorldUtilityManager.instance.GetDamageLayers());
            List<IDamageable> validTargets = new List<IDamageable>();

            float aoeMulti = 1f;

            foreach (Collider hit in hitColliders)
            {
                //ignore casting character 
                if (hit.gameObject == character.gameObject) continue;

                IDamageable splashTarget = hit.GetComponent<IDamageable>();

                bool isAlly = splashTarget != null && splashTarget.MyFaction == Faction.Ally;

                if (splashTarget != null && !splashTarget.IsDead() && !isAlly)
                {
                    validTargets.Add(splashTarget);
                }
            }

            if (validTargets.Count > 0)
            {
                aoeMulti = 1f / validTargets.Count;
            }

            foreach (IDamageable target in validTargets)
            {
                // divide damage based on number of targets hit

                ApplySkillDamageAndStatus(target, true, aoeMulti);
            }
        }
    }
}
