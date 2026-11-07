using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace KE
{
    public class IceLanceExecution : SkillExecution
    {
        [Header("Elemental VFX Variations")]
        public GameObject iceStabVFX;
        [SerializeField] private float fwdOffset = 5f;
        [SerializeField] private float vfxScale = 1.25f;

        Coroutine executeCoroutine;

        public override void Initialize(ActiveSkillInstance activeSkill, CharacterManager character, IDamageable target = null)
        {
            base.Initialize(activeSkill, character);

            executeCoroutine = StartCoroutine(ExecuteSkill());

            Destroy(gameObject, 3f);
        }

        // steps for execution:
        // 1. spawn (w/ forward offset if its that one vfx) 
        // 2. hit anything in spherecast, do damage (pierces through enemies to hit anything in set range)

        IEnumerator ExecuteSkill()
        {
            GameObject selectedImpactVFX = iceStabVFX; // Default
            if (selectedImpactVFX != null)
            {
                Vector3 vfxSpawnPos = transform.position + (transform.forward * fwdOffset); // specific vfx being used has a weird transform origin

                // this should likely be included in the world utility manager now instead of hard coded
                int groundLayerMask = WorldUtilityManager.instance.GetEnviroLayers();

                if (Physics.Raycast(vfxSpawnPos, Vector3.down, out RaycastHit hit, 10f, groundLayerMask))
                {
                    // Snap to the floor, add a little vertical offset for vfx to stay not inside the ground
                    vfxSpawnPos = hit.point + new Vector3(0, 0.55f, 0);
                }

                GameObject spawnedVFX = Instantiate(selectedImpactVFX, vfxSpawnPos, Quaternion.identity);
                spawnedVFX.transform.localScale = new Vector3(vfxScale, vfxScale, vfxScale);
                spawnedVFX.transform.forward = character.transform.forward; // make sure the vfx is facing the same direction as the character
                Destroy(spawnedVFX, 5f);
            }

            yield return new WaitForSeconds(0.05f);

            ApplyDamage();

        }

        void ApplyDamage()
        {
            Vector3 point1 = transform.position + Vector3.up;
            Vector3 point2 = transform.position + (transform.forward * skillData.activeMeleeRange.Value) + Vector3.up;

            Collider[] hitColliders = Physics.OverlapCapsule(point1, point2, skillData.activeAoeRadius.Value, WorldUtilityManager.instance.GetDamageLayers());
            Debug.DrawLine(point1, point2, Color.red, 5);
            List<IDamageable> validTargets = new List<IDamageable>();

            foreach (Collider hit in hitColliders)
            {
                //ignore casting character 
                if (hit.gameObject == character.gameObject) continue;

                IDamageable hitTarget = hit.GetComponent<IDamageable>();
                if (hitTarget != null && !hitTarget.IsDead())
                {
                    validTargets.Add(hitTarget);
                }
            }

            foreach (IDamageable target in validTargets)
            {
                ApplySkillDamageAndStatus(target);
            }
        }
    }
}
