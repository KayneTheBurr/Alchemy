using KE;
using UnityEngine;

public enum NormalAIState { Idle, Chase, Attack }
public class NormalAIManager : BaseAIManager
{
    [Header("AI State")]
    public NormalAIState currentState = NormalAIState.Idle;

    [Header("Targeting Info")]
    public float detectionRadius = 15f;
    public float allyNotifyRadius = 10f;

    //public for debugging 
    public float currentEngagementRange = 2f;
    public float targetDestinationDistance = 0f;

    private int queuedSkillIndex = 0;

    protected override void ProcessStateMachine()
    {
        switch (currentState)
        {
            case NormalAIState.Idle:
                SearchForTarget();
                break;
            case NormalAIState.Chase:
                ChaseTarget();
                break;
            case NormalAIState.Attack:
                AttemptAttack();
                break;
        }
    }
    private NormalAIState SwitchState(NormalAIState newState)
    {
        // ResetFlags();
        return newState;
    }

    private void SearchForTarget()
    {
        aiCharacter.navMeshAgent.isStopped = true;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, WorldUtilityManager.instance.GetDamageLayers());

        float closestDistance = Mathf.Infinity;
        IDamageable bestTarget = null;

        foreach (Collider hit in hitColliders)
        {
            // Skip ourselves
            if (hit.gameObject == aiCharacter.gameObject) continue;

            IDamageable detectedEntity = hit.GetComponent<IDamageable>();

            //lets sort by looking in a cone in front of us, rather tahn eyes in a full 360, but make the detection radius a variable.
            //maybe some enemies DO have eyes in the back of their head

            if (detectedEntity != null && !detectedEntity.IsDead())
            {
                // Ask if we should attack this faction
                if (WorldUtilityManager.instance.CanIDamageThisTarget(aiCharacter.MyFaction, detectedEntity.MyFaction))
                {
                    float distance = Vector3.Distance(transform.position, hit.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        bestTarget = detectedEntity;
                    }
                }
            }
        }

        if (bestTarget != null)
        {
            aiCharacter.aiCombatManager.currentTarget = bestTarget;
            EvaluateCombatOptions();

            ShoutToAllies(bestTarget);

            currentState = SwitchState(NormalAIState.Chase);
        }
    }
    public override void ShoutToAllies(IDamageable detectedTarget)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, allyNotifyRadius, WorldUtilityManager.instance.GetCharacterLayers());

        foreach (Collider hit in hitColliders)
        {
            if (hit.gameObject == aiCharacter.gameObject) continue;

            BaseAIManager allyBrain = hit.GetComponent<BaseAIManager>();

            //give someone on my allied team a target and get them in combat
            if (allyBrain != null && allyBrain.aiCharacter.MyFaction == aiCharacter.MyFaction)
            {
                allyBrain.AlertToTarget(detectedTarget);
            }
        }
    }
    public override void AlertToTarget(IDamageable targetToChase)
    {
        if (currentState == NormalAIState.Idle || aiCharacter.aiCombatManager.currentTarget == null)
        {
            aiCharacter.aiCombatManager.currentTarget = targetToChase;
            EvaluateCombatOptions();
            currentState = NormalAIState.Chase;
        }
    }

    private void ChaseTarget()
    {
        if (aiCharacter.aiCombatManager.currentTarget == null || aiCharacter.aiCombatManager.currentTarget.IsDead())
        {
            currentState = SwitchState(NormalAIState.Idle);
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, aiCharacter.aiCombatManager.currentTarget.GetTransform().position);
        targetDestinationDistance = distanceToTarget;

        if (distanceToTarget <= currentEngagementRange)
        {
            currentState = SwitchState(NormalAIState.Attack);
        }
        else
        {
            aiCharacter.navMeshAgent.isStopped = false;
            aiCharacter.navMeshAgent.SetDestination(aiCharacter.aiCombatManager.currentTarget.GetTransform().position);

            SnapRotationTowards(aiCharacter.navMeshAgent.steeringTarget, aiCharacter.aiMovementManager.rotationSpeed);
        }
    }

    private void AttemptAttack()
    {

        if (aiCharacter.aiCombatManager.currentTarget == null || aiCharacter.aiCombatManager.currentTarget.IsDead())
        {
            currentState = SwitchState(NormalAIState.Idle);
            return;
        }


        float distanceToTarget = Vector3.Distance(transform.position, aiCharacter.aiCombatManager.currentTarget.GetTransform().position);

        // If the player rolled away or is now too far, reevaluate distance and get closer
        if (distanceToTarget > currentEngagementRange)
        {
            EvaluateCombatOptions();
            currentState = SwitchState(NormalAIState.Chase);
            return;
        }

        aiCharacter.navMeshAgent.isStopped = true;
        SnapRotationTowards(aiCharacter.aiCombatManager.currentTarget.GetTransform().position, aiCharacter.aiMovementManager.rotationSpeed);

        if (aiCharacter.characterCombatManager.IsSkillReady(queuedSkillIndex))
        {
            aiCharacter.characterCombatManager.ExecuteSkill(queuedSkillIndex);
        }
    }

    private void EvaluateCombatOptions()
    {
        //TODO  add in a randomizer to or some way to choose which attack to use, just using one base attack for now 

        queuedSkillIndex = 0;
        ActiveSkillInstance activeSkill = aiCharacter.characterCombatManager.activeSkills[queuedSkillIndex];

        if (activeSkill != null)
        {
            if (activeSkill.baseSkill.skillTags.Contains(SkillTag.Melee))
            {
                currentEngagementRange = activeSkill.baseSkill.baseMeleeRange;
            }
            else
            {
                currentEngagementRange = activeSkill.baseSkill.projectileSpeed * activeSkill.baseSkill.baseDuration * 0.8f;
            }

            //set stopping distance to just a bit less than the current attack distance
            aiCharacter.navMeshAgent.stoppingDistance = currentEngagementRange * 0.8f;
        }
    }

    private void SnapRotationTowards(Vector3 targetPosition, float rotationSpeed)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, allyNotifyRadius);
    }
}
