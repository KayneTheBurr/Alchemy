using KE;
using System;
using System.Collections.Generic;
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
    public string activeAnimation;
    public List<SkillTag> activeTags;


    [Header("Localized Stats")]
    public CharacterStat activeSkillCost;
    public CharacterStat activeDamageMultiplier;
    public CharacterStat activeMeleeRange;
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
        activeAnimation = skillSO.skillAnimation;

        activeSkillCost = new CharacterStat(skillSO.baseSkillCost);
        activeDamageMultiplier = new CharacterStat(1.0f);
        activeMeleeRange = new CharacterStat(skillSO.baseMeleeRange);

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
            character.characterCombatManager.currentCastingSkill = this;
            character.characterController.SetMovementDirection(Vector3.zero);
            character.characterController.SetVelocity(Vector3.zero);
            character.isPerformingAction = true;

            character.characterAnimationManager.PlayActionAnimation(activeAnimation, true);
        }
    }

    public void SuccessfullyPerformSkill(CharacterManager character)
    {
        if (activePrefab == null) return;

        Vector3 spawnPos = character.transform.position;
        Quaternion spawnRot = character.transform.rotation;

        switch (baseSkill.spawnLocation)
        {
            case SkillSpawnLocation.CasterRightWeapon:
                spawnPos = character.characterCombatManager.rightWeaponSpawnPoint.position;
                break;
            case SkillSpawnLocation.CasterLeftWeapon:
                spawnPos = character.characterCombatManager.leftWeaponSpawnPoint.position;
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


    public float GetCalculatedDamage(bool isAOESplash = false)
    {
        float damage = baseSkill.baseDamage * activeDamageMultiplier.Value;

        if (isAOESplash)
        {
            damage *= activeAoeDamageMultiplier.Value;
        }

        //crit, crit damage, status application here?
        //all numerical modifiers need to be checked here


        return damage;
    }
}
