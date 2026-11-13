using KE;
using System.Collections.Generic;
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
            return;
        }

        //check for another skill that si ready and IS in range
        int opportunisticSkillIndex = GetReadySkillInRange(distanceToTarget);
        if (opportunisticSkillIndex != -1)
        {
            // Swap to the better skill
            queuedSkillIndex = opportunisticSkillIndex;
            SetEngagementRangeForSkill(queuedSkillIndex);

            currentState = SwitchState(NormalAIState.Attack);
            return;
        }

        // no skills ready and in range, chase more 
        aiCharacter.navMeshAgent.isStopped = false;
        aiCharacter.navMeshAgent.SetDestination(aiCharacter.aiCombatManager.currentTarget.GetTransform().position);

        SnapRotationTowards(aiCharacter.navMeshAgent.steeringTarget, aiCharacter.aiMovementManager.rotationSpeed);

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
            EvaluateCombatOptions();
        }
        else
        {
            EvaluateCombatOptions();
        }
    }

    private void EvaluateCombatOptions()
    {
        // check all 6 skill slots
        List<int> readySkillIndices = new List<int>();

        for (int i = 0; i < aiCharacter.characterCombatManager.activeSkills.Length; i++)
        {
            // check if skill is set and if its off cooldown
            if (aiCharacter.characterCombatManager.activeSkills[i] != null &&
                aiCharacter.characterCombatManager.IsSkillReady(i))
            {
                readySkillIndices.Add(i);
            }
        }

        // pick a random skill thats not on cooldown 
        if (readySkillIndices.Count > 0)
        {
            queuedSkillIndex = readySkillIndices[Random.Range(0, readySkillIndices.Count)];
            SetEngagementRangeForSkill(queuedSkillIndex);
        }

        //ActiveSkillInstance activeSkill = aiCharacter.characterCombatManager.activeSkills[queuedSkillIndex];

        //if (activeSkill != null)
        //{
        //    if (activeSkill.baseSkill.skillTags.Contains(SkillTag.Melee))
        //    {
        //        currentEngagementRange = activeSkill.baseSkill.baseMeleeRange;
        //    }
        //    else
        //    {
        //        currentEngagementRange = activeSkill.activeProjectileRange.Value;

        //        //set stopping distance to just a bit less than the current attack distance
        //        aiCharacter.navMeshAgent.stoppingDistance = currentEngagementRange * 0.8f;
        //    }
        //}
    }
    private int GetReadySkillInRange(float currentDistanceToTarget)
    {
        List<int> validSkillIndices = new List<int>();

        for (int i = 0; i < aiCharacter.characterCombatManager.activeSkills.Length; i++)
        {
            ActiveSkillInstance skill = aiCharacter.characterCombatManager.activeSkills[i];

            if (skill != null && aiCharacter.characterCombatManager.IsSkillReady(i))
            {
                float range = GetSkillRange(skill);

                // If the player is within this skill's range right now, it's valid to use instead
                if (currentDistanceToTarget <= range)
                {
                    validSkillIndices.Add(i);
                }
            }
        }

        if (validSkillIndices.Count > 0)
        {
            return validSkillIndices[Random.Range(0, validSkillIndices.Count)];
        }

        return -1; // No skills are currently in range
    }
    private void SetEngagementRangeForSkill(int index)
    {
        ActiveSkillInstance activeSkill = aiCharacter.characterCombatManager.activeSkills[index];
        if (activeSkill != null)
        {
            currentEngagementRange = GetSkillRange(activeSkill);
            aiCharacter.navMeshAgent.stoppingDistance = currentEngagementRange * 0.8f;
        }
    }
    private float GetSkillRange(ActiveSkillInstance skill)
    {
        if (skill.baseSkill.skillTags.Contains(SkillTag.Melee))
        {
            return skill.baseSkill.baseMeleeRange;
        }
        return skill.activeProjectileRange.Value;
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
