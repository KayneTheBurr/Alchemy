using KE;
using UnityEngine;

public class AIMovementManager : CharacterMovementManager
{
    AICharacterManager aiCharacter;

    [Header("AI Character MOvement Tuning")]
    public float baseMovementSpeed = 3;
    public float rotationSpeed = 50f;

    protected override void Awake()
    {
        base.Awake();
        aiCharacter = GetComponent<AICharacterManager>();
    }

    protected override void Update()
    {
        base.Update();

        if (aiCharacter.isDead.Value || aiCharacter.navMeshAgent == null) return;

        if (!aiCharacter.canMove)
        {
            // STOP THEM MOVING while can move is false
            aiCharacter.navMeshAgent.speed = 0f;
            return;
        }

        //update isMOving flag based on nav mesh velocity 
        bool currentlyMoving = aiCharacter.navMeshAgent.velocity.sqrMagnitude > 0.1f;
        if (isMoving.Value != currentlyMoving)
        {
            isMoving.Value = currentlyMoving;
        }

        float targetSpeed = baseMovementSpeed * aiCharacter.characterStatsManager.moveSpeedMultiplier.Value;

        if (aiCharacter.isPerformingAction && aiCharacter.characterCombatManager.currentCastingSkill != null)
        {
            ActiveSkillInstance castingSkill = aiCharacter.characterCombatManager.currentCastingSkill;

            if (castingSkill.baseSkill.canMoveWhileCasting)
            {
                // appply move speed penalty
                targetSpeed *= castingSkill.activeMoveSpeedPenalty.Value;
            }
            else
            {
                //no movement if they cant move during attack
                targetSpeed = 0f;
            }
        }

        //set speed
        aiCharacter.navMeshAgent.speed = targetSpeed;
    }
}
