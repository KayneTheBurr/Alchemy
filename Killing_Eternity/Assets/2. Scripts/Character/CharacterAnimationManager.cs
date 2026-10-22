using KE;
using System;
using UnityEngine;

public class CharacterAnimationManager : MonoBehaviour
{
    CharacterManager character;

    int vertical;
    int horizontal;

    [Header("Animations State Tracking")]
    public string currentAnimation;
    public string lastDamageAnimationPlayed;


    [Header("Animation Tolerance")]
    public float idleToWalkTolerance = 0.1f;

    int moveSpeedMult;


    [Header("Damping")]
    public float dampTime = 0.1f;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
        moveSpeedMult = Animator.StringToHash("MoveSpeedModifier");
    }
    protected virtual void Update()
    {

    }

    public virtual void UpdateAnimatorMovementParameters(Vector3 moveDirection, bool isSprinting)
    {
        float h = 0;
        float v = 0;

        if (moveDirection != Vector3.zero)
        {
            Vector3 localMove = transform.InverseTransformDirection(moveDirection);
            //localMove.Normalize();

            h = SnapAxis(localMove.x);
            v = SnapAxis(localMove.z);

            if (isSprinting)
            {
                h *= 2f;
                v *= 2f;
            }
        }

        //Pass values in to the animator to update correct anim to play 
        character.animator.SetFloat(horizontal, h, dampTime, Time.deltaTime);
        character.animator.SetFloat(vertical, v, dampTime, Time.deltaTime);

        //Increase Movement Animator State by same percent as our base movepseed amount
        float currentMoveSpeedMultiplier = character.characterStatsManager.moveSpeedMultiplier;
        character.animator.SetFloat(moveSpeedMult, currentMoveSpeedMultiplier);
    }
    private float SnapAxis(float value)
    {
        float abs = Mathf.Abs(value);
        if (abs < idleToWalkTolerance) return 0f;

        float sign = Mathf.Sign(value);

        if (abs <= 0.5f) return 0.5f * sign;

        return 1.0f * sign;
    }

    public virtual void PlayActionAnimation(string targetAnimation, bool isPerformingAction, bool applyRootMotion = false, bool canRotate = false, bool canMove = false)
    {
        character.isPerformingAction = isPerformingAction;
        character.canMove = canMove;
        character.canRotate = canRotate;

        character.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0.2f);
        currentAnimation = targetAnimation;
    }
}
