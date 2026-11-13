using UnityEngine;
using KE;

public class FrozenEffect : StatusEffect
{
    private Animator targetAnimator;
    private Renderer targetRenderer; private Material[] originalMaterials;

    public FrozenEffect()
    {
        statusType = Status.Freeze;
        duration = AlchemyEngine.instance.tuningData.frozenDuration;
    }

    protected override void StartEffect()
    {
        base.StartEffect();
        CharacterManager charManager = target.GetTransform().GetComponent<CharacterManager>();
        DamageableEnv envObj = target.GetTransform().GetComponent<DamageableEnv>();

        // material swap when frozen
        if (charManager != null) targetRenderer = charManager.mainMesh;
        else if (envObj != null) targetRenderer = envObj.mainMesh;
        if (targetRenderer != null && AlchemyEngine.instance.tuningData.frozenMaterial != null)
        {
            // Cache the exact material array the object had before freezing
            originalMaterials = targetRenderer.sharedMaterials;

            // Create a new array of the same length, filled entirely with the ice material
            Material[] frozenMats = new Material[originalMaterials.Length];
            for (int i = 0; i < frozenMats.Length; i++)
            {
                frozenMats[i] = AlchemyEngine.instance.tuningData.frozenMaterial;
            }

            targetRenderer.sharedMaterials = frozenMats;
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
        if (targetRenderer != null && originalMaterials != null)
        {
            targetRenderer.sharedMaterials = originalMaterials;
        }

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
