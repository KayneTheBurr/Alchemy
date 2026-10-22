using KE;
using UnityEngine;

public class AIStatsManager : CharacterStatsManager
{
    AICharacterManager aiCharacter;

    protected override void Awake()
    {
        base.Awake();
        aiCharacter = GetComponent<AICharacterManager>();
    }

    public override void CheckHP(float oldHP, float newHP)
    {
        base.CheckHP(oldHP, newHP);

        // if the enemy takes damage, they move into the combat state 
        if (newHP < oldHP && !aiCharacter.isDead.Value)
        {
            aiCharacter.characterCombatManager.ResetCombatTimer();
        }
    }
}
