using KE;
using UnityEngine;

public class AIAnimationManager : CharacterAnimationManager
{
    AICharacterManager aiCharacter;

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
}
