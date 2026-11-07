using UnityEngine;
using KE;

public class FrozenEffect : StatusEffect
{
    private GameObject activeVFX;
    private Animator targetAnimator;

    public FrozenEffect()
    {
        statusType = Status.Freeze;
        duration = AlchemyEngine.instance.tuningData.frozenDuration;
    }

    protected override void StartEffect()
    {
        base.StartEffect();
        CharacterManager charManager = target.GetTransform().GetComponent<CharacterManager>();

        //spawn vfx 
        GameObject vfxPrefab = AlchemyEngine.instance.tuningData.frozenVFXPrefab;
        if (vfxPrefab != null)
        {
            activeVFX = Object.Instantiate(vfxPrefab, target.GetTransform());
            if (charManager != null)
            {
                activeVFX.GetComponent<OverlayFX>().targetRenderer = charManager.mainMesh;
                activeVFX.GetComponent<OverlayFX>().InitializeWithMesh();
            }
        }

        // stop movement mid animation
       
        if (charManager != null)
        {
            charManager.canMove = false;
            charManager.canRotate = false;

            if (charManager is AICharacterManager aiManager && aiManager.navMeshAgent != null)
            {
                aiManager.navMeshAgent.isStopped = true;
            }

            targetAnimator = charManager.GetComponentInChildren<Animator>();
            if (targetAnimator != null)
            {
                targetAnimator.speed = 0f;
            }
        }
    }

    public override void RemoveEffect()
    {
        base.RemoveEffect();

        if (activeVFX != null) Object.Destroy(activeVFX);

        CharacterManager charManager = target.GetTransform().GetComponent<CharacterManager>();
        if (charManager != null)
        {
            // Restore movement flags
            charManager.canMove = true;
            charManager.canRotate = true;

            if (charManager is AICharacterManager aiManager && aiManager.navMeshAgent != null)
            {
                aiManager.navMeshAgent.isStopped = false;
            }

            if (targetAnimator != null)
            {
                targetAnimator.speed = 1f;
            }
        }
    }
}
