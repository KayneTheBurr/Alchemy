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
    public CharacterStat activeStatusMod;
    public CharacterStat activeAnimationSpeed;
    public CharacterStat activeMoveSpeedPenalty;

    public ActiveSkillInstance(Skill skillSO)
    {
        baseSkill = skillSO;
        activeTags = new List<SkillTag>(skillSO.skillTags);

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
        activeAnimationSpeed = new CharacterStat(skillSO.baseAnimationSpeed);
        activeStatusMod = new CharacterStat(skillSO.baseStatusMod);
        activeMoveSpeedPenalty = new CharacterStat(skillSO.moveSpeedPenalty);
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
            if (!baseSkill.canMoveWhileCasting)
            {
                character.characterController.SetMovementDirection(Vector3.zero);
                character.characterController.SetVelocity(Vector3.zero);
            }
            character.isPerformingAction = true;

            float totalAttackSpeed = baseSkill.baseAnimationSpeed;

            //add in attack speed multipliers here

            character.characterAnimationManager.PlayActionAnimation(
            activeAnimation,
            true,
            baseSkill.applyRootMotion,
            baseSkill.canRotateWhileCasting,
            baseSkill.canMoveWhileCasting,
            totalAttackSpeed
        );
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
                // need targetting to do this 
                if (character.characterCombatManager.currentTarget != null)
                {
                    spawnPos = character.characterCombatManager.currentTarget.GetTransform().position;
                }
                break;
            case SkillSpawnLocation.CursorLocation:
                if (character.characterCombatManager.currentTarget == null)
                {
                    if(InputManager.instance.isGamepadActive)
                    {
                        spawnPos = character.transform.position + character.transform.forward * 3f;
                    }
                    else
                    {
                        Ray ray = PlayerCamera.instance.realCam.ScreenPointToRay(InputManager.instance.lookInput);

                        LayerMask cursorMask = WorldUtilityManager.instance.GetEnviroLayers() | 
                            WorldUtilityManager.instance.GetCharacterLayers();

                        if (Physics.Raycast(ray, out RaycastHit hit, 100f, cursorMask))
                        {
                            spawnPos = hit.point;
                        }
                        else
                        {
                            spawnPos = character.transform.position + character.transform.forward * 3f;
                        }
                    }
                }
                else
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

    #region Damage Calculations
    // Base Damage Calc
    public Dictionary<Element, float> GetCoreElementalDamageValues(CharacterManager character, bool isAOESplash = false)
    {
        Dictionary<Element, float> payload = new Dictionary<Element, float>();

        // Add the elemental damage from base skill 
        payload[activeElement] = baseSkill.baseDamage;

        if (character != null && character.characterStatsManager != null)
        {
            //helper func to add each element to the dictionary first
            void AddFlatDamage(Element el, float amount)
            {
                if (amount > 0)
                {
                    if (!payload.ContainsKey(el)) payload[el] = 0f;
                    payload[el] += amount;
                }
            }

            // 2. Add flat damage from gear
            AddFlatDamage(Element.Blood, character.characterStatsManager.flatAddedBloodDamage.Value);
            AddFlatDamage(Element.Bile, character.characterStatsManager.flatAddedBileDamage.Value);
            AddFlatDamage(Element.Fire, character.characterStatsManager.flatAddedFireDamage.Value);
            AddFlatDamage(Element.Ice, character.characterStatsManager.flatAddedIceDamage.Value);
            AddFlatDamage(Element.Spark, character.characterStatsManager.flatAddedSparkDamage.Value);
            AddFlatDamage(Element.Stone, character.characterStatsManager.flatAddedStoneDamage.Value);

            // apply Crit modifiers here
            float critMultiplier = 1f;
            if (UnityEngine.Random.Range(0f, 100f) <= character.characterStatsManager.critChance.Value)
            {
                critMultiplier = character.characterStatsManager.critDamageMultiplier.Value;
                Debug.Log("Critical Hit!");
            }

            float aoeMult = 1f;
            if (isAOESplash)
            {
                aoeMult = activeAoeDamageMultiplier.Value;
            }

            float totalMultiplier = critMultiplier * aoeMult;

            List<Element> keys = new List<Element>(payload.Keys);
            foreach (Element el in keys)
            {
                payload[el] *= totalMultiplier;
            }
        }
        return payload;
    }

    // Direct Hit Damage
    public Dictionary<Element, float> GetCalculatedHitDamage(Dictionary<Element, float> corePayload, CharacterManager character, bool isAOESplash = false)
    {
        Dictionary<Element, float> hitDamage = new Dictionary<Element, float>(corePayload);

        float hitMultiplier = activeDamageMultiplier.Value;

        if (character != null && character.characterStatsManager != null)
        {
            //always use the all damage multiplier
            hitMultiplier *= character.characterStatsManager.allDamageModifier.Value;

            // Add Spell or Attack damage, dont affect buildup
            // cehck for skill tags attack vs spell vs whatever later just globally using spell damge for now 

            //Really large if statement for different tags here?
            //if Tag.Fire then include fire damage, if Tag.Spell then use spell damage etc.
            if(activeTags.Contains(SkillTag.Spell))
            {
                hitMultiplier *= character.characterStatsManager.spellDamageModifier.Value;
            }
            if (activeTags.Contains(SkillTag.Attack))
            {
                hitMultiplier *= character.characterStatsManager.attackDamageModifier.Value;
            }
        }

        List<Element> keys = new List<Element>(hitDamage.Keys);
        foreach (Element el in keys)
        {
            float elementalMod = 1f;
            if (character != null && character.characterStatsManager != null)
            {
                elementalMod = character.characterStatsManager.GetElementalDamageModifier(el);
            }

            hitDamage[el] *= (hitMultiplier * elementalMod);
        }

        return hitDamage;
    }

    // Damage Over Time
    public Dictionary<Element, float> GetCalculatedDOTPayload(Dictionary<Element, float> corePayload, CharacterManager character)
    {
        Dictionary<Element, float> dotPayload = new Dictionary<Element, float>(corePayload);

        if (character != null && character.characterStatsManager != null)
        {
            float dotMultiplier = character.characterStatsManager.dotDamageMultiplier.Value;
            dotMultiplier *= character.characterStatsManager.allDamageModifier.Value;

            List<Element> keys = new List<Element>(dotPayload.Keys);
            foreach (Element el in keys)
            {
                float elementalMod = character.characterStatsManager.GetElementalDamageModifier(el);

                // add in specific magnitude like toxin/burn damage here

                dotPayload[el] *= (dotMultiplier * elementalMod);
            }
        }

        return dotPayload;
    }

    // Status Application
    public Dictionary<Element, float> GetCalculatedBuildup(Dictionary<Element, float> corePayload, CharacterManager character)
    {
        Dictionary<Element, float> buildupPayload = new Dictionary<Element, float>(corePayload);

        // local conversion ratio from the skill and its modifiers
        float skillConversionRate = activeStatusMod.Value;
        float statModifier = 1f;

        if (character != null && character.characterStatsManager != null)
        {
            // global sttat mod modifier
            statModifier = character.characterStatsManager.statusBuildupMultiplier.Value;
        }

        List<Element> keys = new List<Element>(buildupPayload.Keys);
        foreach (Element el in keys)
        {
            buildupPayload[el] *= (skillConversionRate * statModifier);
        }

        return buildupPayload;
    }
    #endregion

}
