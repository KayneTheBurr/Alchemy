using KE;
using System;
using UnityEngine;

[Serializable]
public class ActiveSkillInstance
{
    public Skill baseSkill;

    [Header("Dynamic Stats")]
    public ResourceType activeResourceType;
    public float currentCooldownTimer;
    public Element activeElement;
    public GameObject activePrefab;


    [Header("Localized Stats")]
    public CharacterStat activeSkillCost;
    public CharacterStat activeDamageMultiplier;
    public CharacterStat activeCooldown;
    public CharacterStat activeAoeRadius;
    public CharacterStat activeDuration;
    public CharacterStat activeAoeDamageMultiplier;
    public CharacterStat activeProjectileSpeed;

    public ActiveSkillInstance(Skill skillSO)
    {
        baseSkill = skillSO;

        // State Initialization
        activeResourceType = skillSO.baseResourceType;
        activeElement = skillSO.skillElement;
        currentCooldownTimer = 0f;
        activePrefab = skillSO.skillPrefab;

        activeSkillCost = new CharacterStat(skillSO.baseSkillCost);
        activeDamageMultiplier = new CharacterStat(1.0f);

        activeCooldown = new CharacterStat(skillSO.baseCooldown);
        activeAoeRadius = new CharacterStat(skillSO.baseAoeRadius);
        activeAoeDamageMultiplier = new CharacterStat(skillSO.baseAoeDamageMult);
        activeDuration = new CharacterStat(skillSO.baseDuration);
        activeProjectileSpeed = new CharacterStat(skillSO.projectileSpeed);
    }

    public bool CanIPerformSkill(CharacterManager character)
    {
        if (currentCooldownTimer > 0) return false;

        //either update the skill cost in real time all the time, or check here
        float finalCost = activeSkillCost.Value;

        switch (activeResourceType)
        {
            case ResourceType.Mana:
                return character.characterStatsManager.currentMana.Value >= finalCost;

            case ResourceType.Stamina:
                return character.characterStatsManager.currentStamina.Value >= finalCost;

            case ResourceType.Health:
                return character.characterStatsManager.currentHealth.Value > finalCost;
            
            default:
                return false;
        }
    }

    public void AttemptToPerformSkill(CharacterManager character)
    {
        if (character.isPerformingAction || character.characterMovementManager.isDodging) return;

        if (CanIPerformSkill(character))
        {
            DeductResourceCost(character);

            character.characterAnimationManager.PlayActionAnimation(baseSkill.skillAnimation, true);
        }
    }

    public void SuccessfullyPerformSkill(CharacterManager character)
    {
        if (activePrefab == null) return;

        Vector3 spawnPos = character.transform.position;
        Quaternion spawnRot = character.transform.rotation;

        switch (baseSkill.spawnLocation)
        {
            case SkillSpawnLocation.CasterWeapon:
                spawnPos = character.characterCombatManager.weaponSpawnPoint.position;
                break;
            case SkillSpawnLocation.CasterFeet:
                spawnPos = character.transform.position;
                break;
            case SkillSpawnLocation.TargetEnemy:
                // Assuming your CombatManager tracks lock-on or hovered targets
                if (character.characterCombatManager.currentTarget != null)
                {
                    spawnPos = character.characterCombatManager.currentTarget.GetTransform().position;
                }
                break;
        }
        GameObject skillObj = UnityEngine.Object.Instantiate(activePrefab, spawnPos, spawnRot);

        SkillExecution executionLogic = skillObj.GetComponent<SkillExecution>();
        if (executionLogic != null) executionLogic.Initialize(this, character);

    }

    private void DeductResourceCost(CharacterManager character)
    {
        float finalCost = activeSkillCost.Value;

        switch (activeResourceType)
        {
            case ResourceType.Mana: character.characterStatsManager.TryConsumeMana(finalCost); break;
            case ResourceType.Stamina: character.characterStatsManager.TryConsumeStamina(finalCost); break;
            case ResourceType.Health: character.characterStatsManager.TryConsumeHealth(finalCost); break;
        }
    }
}
