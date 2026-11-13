using KE;
using UnityEngine;

public class SlamExecution : SkillExecution
{
    public GameObject VFXObject;
    public float selfDestructTime;
    public override void Initialize(ActiveSkillInstance activeSkill, CharacterManager characterManager, IDamageable target = null)
    {
        base.Initialize(activeSkill, characterManager);

        PerformMelee();

        Destroy(gameObject, selfDestructTime);
    }

    private void PerformMelee()
    {
        Vector3 hitCenter = transform.position + (transform.forward * (skillData.activeMeleeRange.Value * 0.5f));
        Instantiate(VFXObject, hitCenter, Quaternion.identity);
        Collider[] hitColliders = Physics.OverlapSphere(
            hitCenter, skillData.activeAoeRadius.Value, WorldUtilityManager.instance.GetDamageLayers());

        foreach (Collider hit in hitColliders)
        {
            if (hit.gameObject == character.gameObject || hit.isTrigger) continue;

            IDamageable target = hit.GetComponent<IDamageable>();
            if (target != null)
            {
                if (WorldUtilityManager.instance.CanIDamageThisTarget(character.MyFaction, target.MyFaction))
                {
                    ApplySkillDamageAndStatus(target, false);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (skillData != null)
        {
            Gizmos.color = Color.red;
            Vector3 hitCenter = transform.position + (transform.forward * (skillData.activeMeleeRange.Value * 0.5f));
            Gizmos.DrawWireSphere(hitCenter, skillData.activeAoeRadius.Value);
        }
    }
}
