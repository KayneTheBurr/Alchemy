using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KE
{
    public class EarthquakeExecution : SkillExecution
    {
        [Header("Elemental VFX Variations")]
        public GameObject earthQuakeVFX;

        [Header("Misc. Settings")]
        public float VFXScale = 1;
        public float secondHitDelay = 1.3f;
        [Tooltip("Affects size of damage radius for the second hit. Base radius (as determined by skill data's activeAoeRadius) is multiplied by this value.")]
        public float secondHitRadiusMulti = 1.5f;

        Coroutine executeSkillRoutine;

        public override void Initialize(ActiveSkillInstance activeSkill, CharacterManager character, IDamageable target = null)
        {
            base.Initialize(activeSkill, character);

            executeSkillRoutine = StartCoroutine(ExecuteEarthquake());

            Destroy(gameObject, 5f);
        }

        IEnumerator ExecuteEarthquake()
        {
            GameObject selectedVFX = earthQuakeVFX; // Default
            if(selectedVFX != null)
            {
                // prep vfx spawn position & duration
                Vector3 vfxSpawnPos = character.transform.position;
                GameObject spawnedVFX = Instantiate(selectedVFX, vfxSpawnPos, Quaternion.identity);
                spawnedVFX.transform.localScale = new Vector3(VFXScale, VFXScale, VFXScale);
                Destroy(spawnedVFX, 5f);
            }

            HitInRadius(skillData.activeAoeRadius.Value, false); // first hit (full damage)

            yield return new WaitForSeconds(secondHitDelay);

            HitInRadius(secondHitRadiusMulti * skillData.activeAoeRadius.Value, true); // AOE multiplier is applied to second hit
        }

        void HitInRadius(float radius, bool isSecondHit = false)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, WorldUtilityManager.instance.GetDamageLayers());
            List<IDamageable> validTargets = new List<IDamageable>();

            foreach (Collider hit in hitColliders) // add valid hit targets to list
            {
                if (hit.gameObject == character.gameObject) continue;
                IDamageable damageable = hit.GetComponent<IDamageable>();

                bool isAlly = damageable != null && damageable.MyFaction == Faction.Ally;

                if (damageable != null && !validTargets.Contains(damageable) && !damageable.IsDead() && !isAlly)
                {
                    validTargets.Add(damageable);
                }
            }
            foreach (IDamageable target in validTargets) // deal damage to valid targets
            {
                ApplySkillDamageAndStatus(target, isSecondHit);
            }
        }
    }
}
