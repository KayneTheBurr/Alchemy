using KE;
using UnityEngine;
public class Skill : ScriptableObject
{
    [Header("Skill Info")]
    public string skillName;
    [TextArea] public string skillDescription;
    public int skillID;
    public Sprite skillIcon; //get skill from skill database of all skills

    [Header("Skill Costs")]
    public int baseManaCost = 10;

    [Header("Animation")]
    public string skillAnimation; //animation the plays when we start using the skill

    //[Header("Skill Requirements")]
    //weapon, gear, status reqs for using a skill 9Must be actively burning to use) etc

    public virtual void AttemptToPerformSkill(CharacterManager characterPerformingSKill)
    {

    }

    public virtual void SuccessfullyPerformSkill(CharacterManager characterPerformingSkill)
    {

    }
    public virtual bool CanIPerformThisSkill(CharacterManager characterPerformingSKill)
    {
        //check for gear/stat requirements

        //check for enough resource to cast skill (mana, health, shield, etc.)

        return true;
    }
}
