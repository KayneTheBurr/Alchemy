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

        //update isMOving flag based on nav mesh velocity 
        bool currentlyMoving = aiCharacter.navMeshAgent.velocity.sqrMagnitude > 0.1f;
        if (isMoving.Value != currentlyMoving)
        {
            isMoving.Value = currentlyMoving;
        }

        //scale navmesh speed with movespeed stats manager value
        aiCharacter.navMeshAgent.speed = baseMovementSpeed * aiCharacter.characterStatsManager.moveSpeedMultiplier.Value;
    }
}
