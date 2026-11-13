using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KE
{
    public class CurseExecution : SkillExecution
    {
        [Header("Elemental VFX Variations")]
        public GameObject bileDebuffVFX;
        [Header("Misc. Settings")]
        public float curseDamageDelay = 0.25f;
        Coroutine curseDamageDelayRoutine;

        public override void Initialize(ActiveSkillInstance activeSkill, CharacterManager character, IDamageable target = null)
        {
            base.Initialize(activeSkill, character);

            CurseTarget();

            Destroy(gameObject, 5f);
        }

        void CurseTarget()
        {
            if(character.characterCombatManager.currentTarget == null) return;

            GameObject selectedImpactVFX = bileDebuffVFX; // Default
            if(selectedImpactVFX != null)
            {
                // prep vfx spawn position & duration
                IDamageable targetEnemy = character.characterCombatManager.currentTarget;
                Vector3 vfxSpawnPos = targetEnemy.GetTransform().position;

                GameObject spawnedVFX = Instantiate(selectedImpactVFX, vfxSpawnPos, Quaternion.identity, targetEnemy.GetTransform());

                if(curseDamageDelayRoutine == null)
                    curseDamageDelayRoutine = StartCoroutine(CurseDamageDelayRoutine(targetEnemy));

                Destroy(spawnedVFX, 5f);
            }
        }

        IEnumerator CurseDamageDelayRoutine(IDamageable target)
        {
            yield return new WaitForSeconds(curseDamageDelay);
            ApplySkillDamageAndStatus(target);
        }
    }
}
