using KE;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class AIAnimationManager : CharacterAnimationManager
{
    AICharacterManager aiCharacter;

    [Header("AI Movement Settings")]
    [Tooltip("If true, ignores horizontal velocity and forces the AI to only play forward movement animations")]
    public bool forceForwardMovementOnly = true;

    protected override void Awake()
    {
        base.Awake();
        aiCharacter = GetComponent<AICharacterManager>();
    }

    protected override void Update()
    {
        base.Update();

        if (aiCharacter.isDead.Value || aiCharacter.navMeshAgent == null) return;

        UpdateAnimatorMovementParameters(aiCharacter.navMeshAgent.velocity, false);
    }

    private void OnAnimatorMove()
    {
        if (!aiCharacter.applyRootMotion || aiCharacter.isDead.Value) return;

        transform.position += aiCharacter.animator.deltaPosition;
        transform.rotation *= aiCharacter.animator.deltaRotation;

        if (aiCharacter.navMeshAgent != null)
        {
            aiCharacter.navMeshAgent.nextPosition = transform.position;
        }
    }

    public override void UpdateAnimatorMovementParameters(Vector3 moveDirection, bool isSprinting)
    {
        if (forceForwardMovementOnly)
        {
            float speed = moveDirection.magnitude;

            aiCharacter.animator.SetFloat(horizontal, 0f, dampTime, Time.deltaTime);
            aiCharacter.animator.SetFloat(vertical, speed, dampTime, Time.deltaTime);

            float currentMoveSpeedMultiplier = aiCharacter.characterStatsManager.moveSpeedMultiplier.Value;
            aiCharacter.animator.SetFloat(moveSpeedMult, currentMoveSpeedMultiplier);
        }
        else
        {
            base.UpdateAnimatorMovementParameters(moveDirection, isSprinting);
        }
    }
}
