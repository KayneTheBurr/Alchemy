using KE;
using UnityEngine;

public class AICombatManager : CharacterCombatManager
{
    AICharacterManager aiCharacter;

    [Header("AI Skills Loadout")]
    public Skill[] defaultAISkills = new Skill[6];

    protected override void Awake()
    {
        base.Awake();
        aiCharacter = GetComponent<AICharacterManager>();
    }

    protected override void Start()
    {
        base.Start();

        // take the skills slotted and make the Active Instance of the skills here since they dont have updating UI to do so 
        for (int i = 0; i < defaultAISkills.Length; i++)
        {
            if (defaultAISkills[i] != null)
            {
                activeSkills[i] = new ActiveSkillInstance(defaultAISkills[i]);

                // TODO:
                // Multiply activeSkills[i].activeDamageMultiplier, and all other stats here by level for scaling
            }
        }
    }
}
