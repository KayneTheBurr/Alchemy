using KE;
using UnityEngine;

public enum NormalAIState { Idle, Chase, Attack }
public class NormalAIManager : BaseAIManager
{
    [Header("AI State")]
    public NormalAIState currentState = NormalAIState.Idle;

    [Header("Targeting Info")]
    public float detectionRadius = 15f;

    private float currentEngagementRange = 2f;
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
        // Stop moving while idling
        aiCharacter.navMeshAgent.isStopped = true;

        if (PlayerCamera.instance.player != null && Vector3.Distance(transform.position, PlayerCamera.instance.player.transform.position) <= detectionRadius)
        {
            currentTarget = PlayerCamera.instance.player;
            EvaluateCombatOptions();
            currentState = SwitchState(NormalAIState.Chase);
        }
    }

    private void ChaseTarget()
    {
        if (currentTarget == null || currentTarget.isDead.Value)
        {
            currentState = SwitchState(NormalAIState.Idle);
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);

        if (distanceToTarget <= currentEngagementRange)
        {
            currentState = SwitchState(NormalAIState.Attack);
        }
        else
        {
            aiCharacter.navMeshAgent.isStopped = false;
            aiCharacter.navMeshAgent.SetDestination(currentTarget.transform.position);
            SnapRotationTowards(aiCharacter.navMeshAgent.steeringTarget, 10f);
        }
    }

    private void AttemptAttack()
    {
        if (currentTarget == null || currentTarget.isDead.Value)
        {
            currentState = SwitchState(NormalAIState.Idle);
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);

        // If the player rolled away, re-evaluate skills and resume chase
        if (distanceToTarget > currentEngagementRange)
        {
            EvaluateCombatOptions();
            currentState = SwitchState(NormalAIState.Chase);
            return;
        }

        // Hard lock rotation to the target while attacking
        aiCharacter.navMeshAgent.isStopped = true;
        SnapRotationTowards(currentTarget.transform.position, 25f);

        // Execute the skill we decided on during EvaluateCombatOptions
        aiCharacter.characterCombatManager.ExecuteSkill(queuedSkillIndex);
    }

    private void EvaluateCombatOptions()
    {
        // Reads the equipped skills to determine how close it needs to get.
        // Defaults to index 0. If it's a projectile, it stops at the projectile range.

        queuedSkillIndex = 0;
        ActiveSkillInstance activeSkill = aiCharacter.characterCombatManager.activeSkills[queuedSkillIndex];

        if (activeSkill != null)
        {
            // Check if it's a melee skill using the SkillTags list you built earlier
            if (activeSkill.baseSkill.skillTags.Contains(SkillTag.Melee))
            {
                currentEngagementRange = activeSkill.baseSkill.baseMeleeRange;
            }
            else
            {
                // For projectiles, stop a bit short of maximum distance
                currentEngagementRange = activeSkill.baseSkill.projectileSpeed * activeSkill.baseSkill.baseDuration * 0.8f;
            }
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
}
