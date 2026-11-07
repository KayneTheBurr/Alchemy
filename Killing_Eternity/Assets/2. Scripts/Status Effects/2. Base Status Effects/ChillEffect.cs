using KE;
using UnityEngine;

public class ChillEffect : StatusEffect
{
    //public float slowPercentage = -0.30f;
    private CharacterManager affectedCharacter;

    public ChillEffect()
    {
        statusType = Status.Chill;
        duration = AlchemyEngine.instance.tuningData.chillDuration;
    }

    protected override void StartEffect()
    {
        //Debug.Log("Chill Applied");
        MonoBehaviour targetMono = target as MonoBehaviour;

        float slowPercentage = -1 * AlchemyEngine.instance.tuningData.chillAmount;

        if (targetMono != null)
        {
            affectedCharacter = targetMono.GetComponent<CharacterManager>();
        }

        if (affectedCharacter != null && affectedCharacter.characterStatsManager != null)
        {
            // Apply the debuff
            StatModifier actionSlow = new StatModifier(slowPercentage, StatModType.PercentAdd, this);
            StatModifier moveSlow = new StatModifier(slowPercentage, StatModType.PercentAdd, this);

            affectedCharacter.characterStatsManager.actionSpeed.AddModifier(actionSlow);
            affectedCharacter.characterStatsManager.moveSpeedMultiplier.AddModifier(moveSlow);
        }

        // add in the frost vfx here
    }

    public override void RemoveEffect()
    {
        if (affectedCharacter != null && affectedCharacter.characterStatsManager != null)
        {
            affectedCharacter.characterStatsManager.actionSpeed.RemoveAllModifiersFromSource(this);
            affectedCharacter.characterStatsManager.moveSpeedMultiplier.RemoveAllModifiersFromSource(this);
        }
    }
}
