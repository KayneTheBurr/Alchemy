using KE;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UIElements;

public class CharacterStatsManager : MonoBehaviour
{
    #region Variables

    #region References
    CharacterManager character;
    #endregion

    #region Resources
    [Header("Core Resources Pools (Current)")]
    public ObservableVariable<float> currentHealth = new ObservableVariable<float>(100);
    public ObservableVariable<float> currentStamina = new ObservableVariable<float>(100);
    public ObservableVariable<float> currentMana = new ObservableVariable<float>(100);

    [Header("Core Resource Pools (MAX)")]
    public CharacterStat maxHealth = new CharacterStat(100f);
    public CharacterStat maxStamina = new CharacterStat(100f);
    public CharacterStat maxMana = new CharacterStat(100f);

    [Header("Base Resource Levels (Before attribte levels) ")]
    public float baseHealth = 100;
    public float baseMana = 100;
    public float baseStamina = 100;

    [Header("Stamina Regeneration")]
    public float staminaRegenTimer = 0;
    public CharacterStat staminaRegenDelay = new CharacterStat(1f);
    public CharacterStat staminaCostModifier = new CharacterStat(1f);
    public CharacterStat staminaRegenPerSec = new CharacterStat(20f);
    public float inCombatStaminaRegenScaler = 1.0f;

    [Header("Mana Regeneration")]
    public float manaRegenTimer = 0;
    public CharacterStat manaRegenDelay = new CharacterStat(.25f);
    public CharacterStat manaCostModifier = new CharacterStat(1f);
    public CharacterStat manaRegenPerSecond = new CharacterStat(10f);
    public float inCombatManaRegenScaler = 1.0f;
    #endregion

    #region Damage
    [Header("Damage Multipliers (1.0 = 100%)")]
    public CharacterStat allDamageModifier = new CharacterStat(1f);
    public CharacterStat spellDamageModifier = new CharacterStat(1f);
    public CharacterStat attackDamageModifier = new CharacterStat(1f);

    public CharacterStat directDamageMultiplier = new CharacterStat(1f);
    public CharacterStat dotDamageMultiplier = new CharacterStat(1f);

    public CharacterStat bloodDamageMult = new CharacterStat(1f);
    public CharacterStat bileDamageMult = new CharacterStat(1f);
    public CharacterStat fireDamageMult = new CharacterStat(1f);
    public CharacterStat iceDamageMult = new CharacterStat(1f);
    public CharacterStat sparkDamageMult = new CharacterStat(1f);
    public CharacterStat stoneDamageMult = new CharacterStat(1f);

    [Header("Flat Added Damage")]
    public CharacterStat flatAddedBloodDamage = new CharacterStat(0f);
    public CharacterStat flatAddedBileDamage = new CharacterStat(0f);
    public CharacterStat flatAddedFireDamage = new CharacterStat(0f);
    public CharacterStat flatAddedIceDamage = new CharacterStat(0f);
    public CharacterStat flatAddedSparkDamage = new CharacterStat(0f);
    public CharacterStat flatAddedStoneDamage = new CharacterStat(0f);


    [Header("Additional Combat Multipliers")]
    public CharacterStat critChance = new CharacterStat(5f);             // 5% base
    public CharacterStat critDamageMultiplier = new CharacterStat(1.5f);    // 150% base
    public CharacterStat statusBuildupMultiplier = new CharacterStat(1f);   //defualt to using 100% of base damage

    #endregion

    #region Core Defense 
    [Header("Armour")]
    public CharacterStat armourAmount = new CharacterStat(0f);
    public float maxArmorMitigation = 0.90f;
    public float armorTuningFactor = 10f;
    public CharacterStat armourAppliedToAllElements = new CharacterStat(0f);
    public float averageDamageReceivedBase = 20f;
    public float damageRecievedIncreasePerLevel = 15f;

    [Header("Evasion")]
    public CharacterStat evasionRating = new CharacterStat(0f);
    public float maxEvasionChance = 95;
    public float evasionResetWindow = 3f;
    [Tooltip("Scaling value on character level to scale up evasion per level needed")]
    public float evasionScalingConstant = 25f;
    private float evasionEntropy = 0f;
    private float timeSinceLastEvasionCheck = 0f;
     
    [Header("Regenerative Shield")]
    public CharacterStat shieldAmount = new CharacterStat(0f);
    public ObservableVariable<float> currentShield = new ObservableVariable<float>(0f);
    private float shieldRegenTimer = 0;
    public CharacterStat shieldRegenDelay = new CharacterStat(1f);
    public CharacterStat shieldRegenPerSec = new CharacterStat(20f);
    public float inCombatShieldRegenScaler = 1.0f;

    [Header("Global Defense Layers")]
    public CharacterStat allDamageReduction = new CharacterStat(1f);
    #endregion

    #region Sustain
    [Header("Recoup")]
    public CharacterStat lifeRecoup = new CharacterStat(0f);
    public CharacterStat manaRecoup = new CharacterStat(0f);
    public CharacterStat shieldRecoup = new CharacterStat(0f);
    public CharacterStat staminaRecoup = new CharacterStat(0f);
    public CharacterStat recoupDuration = new CharacterStat(4.0f);
    private List<DamageRecoupInstance> activeRecoups = new List<DamageRecoupInstance>();
    private struct DamageRecoupInstance
    {
        public float remainingTime;
        public float lifePerSecond;
        public float manaPerSecond;
        public float shieldPerSecond;
        public float staminaPerSecond;
    }

    [Header("Resource On Hit")]
    public CharacterStat lifeOnHit = new CharacterStat(0f);
    public CharacterStat manaOnHit = new CharacterStat(0f);
    public CharacterStat staminaOnHit = new CharacterStat(0f);
    public CharacterStat shieldOnHit = new CharacterStat(0f);

    [Header("Resource Steal")]
    public CharacterStat lifeStealAmount = new CharacterStat(0f);
    public CharacterStat manaStealAmount = new CharacterStat(0f);
    public CharacterStat staminaStealAmount = new CharacterStat(0f);
    public CharacterStat shieldStealAmount = new CharacterStat(0f);

    [Header("Life Regeneration")]
    public CharacterStat flatLifeRegenPerSec = new CharacterStat(0f);
    public CharacterStat percentLifeRegenPerSec = new CharacterStat(0f);
    #endregion

    #region Elemental Defenses
    [Header("Resistances")]
    public CharacterStat bloodDefense = new CharacterStat(0f);
    public CharacterStat bileDefense = new CharacterStat(0f);
    public CharacterStat fireDefense = new CharacterStat(0f);
    public CharacterStat iceDefense = new CharacterStat(0f);
    public CharacterStat sparkDefense = new CharacterStat(0f);
    public CharacterStat stoneDefense = new CharacterStat(0f);

    [Header("Max Elemental Resistances")]
    public CharacterStat maxBloodResistance = new CharacterStat(100f);
    public CharacterStat maxBileResistance = new CharacterStat(100F);
    public CharacterStat maxFireResistance = new CharacterStat(100f);
    public CharacterStat maxIceResistance = new CharacterStat(100f);
    public CharacterStat maxSparkResistance = new CharacterStat(100f);
    public CharacterStat maxStoneResistance = new CharacterStat(100f);

    [Header("Elemental Damage Taken Multipliers")]
    public CharacterStat bloodDamageTakenMult = new CharacterStat(1f);
    public CharacterStat bileDamageTakenMult = new CharacterStat(1f);
    public CharacterStat fireDamageTakenMult = new CharacterStat(1f);
    public CharacterStat iceDamageTakenMult = new CharacterStat(1f);
    public CharacterStat sparkDamageTakenMult = new CharacterStat(1f);
    public CharacterStat stoneDamageTakenMult = new CharacterStat(1f);

    [Header("Ailment Thresholds (Status Resistance)")]
    [Tooltip("How much buildup is required to trigger the status effect.")]
    public CharacterStat bloodThreshold = new CharacterStat(100f);
    public CharacterStat bileThreshold = new CharacterStat(100f);
    public CharacterStat fireThreshold = new CharacterStat(100f);
    public CharacterStat iceThreshold = new CharacterStat(100f);
    public CharacterStat sparkThreshold = new CharacterStat(100f);
    public CharacterStat stoneThreshold = new CharacterStat(100f);
    #endregion

    #region Speed
    [Header("Action Speed")]
    public CharacterStat actionSpeed = new CharacterStat(1.0f);

    [Header("Speed Stats")]
    public CharacterStat moveSpeedMultiplier = new CharacterStat(1.0f);
    #endregion

    #endregion

    #region Core Functions
    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }
    protected virtual void Start()
    {
        currentHealth.Value = maxHealth.Value;
        currentStamina.Value = maxStamina.Value;
        currentMana.Value = maxMana.Value;

        SyncStatusThresholds();
    }
    protected virtual void OnEnable()
    {
        currentHealth.OnValueChanged += CheckHP;
        currentMana.OnValueChanged += ResetManaRegenTimer;
        currentStamina.OnValueChanged += ResetStaminaRegenTimer;
        currentShield.OnValueChanged += ResetShieldRegenTimer;
    }
    protected virtual void OnDisable()
    {
        currentHealth.OnValueChanged -= CheckHP;
        currentMana.OnValueChanged -= ResetManaRegenTimer;
        currentStamina.OnValueChanged -= ResetStaminaRegenTimer;
        currentShield.OnValueChanged -= ResetShieldRegenTimer;
    }
    protected virtual void Update()
    {
        RegenerateStamina();
        RegenerateMana();
        RegenerateShield();
        TrackEvasionEntropy();
        RegenerateHealth();
        CalculateRecoupEffects();
    }
    #endregion

    #region Core Resources
    // --- HEALTH ---
    public virtual void CheckHP(float oldHP, float newHP)
    {
        if (character.isDead.Value) return;

        if (currentHealth.Value <= 0)
        {
            character.isDead.Value = true;
            character.StartCoroutine(character.HandleDeathEvents());
        }

        if (currentHealth.Value > maxHealth.Value)
        {
            currentHealth.Value = maxHealth.Value;
        }
    }
    public virtual void RegenerateStamina()
    {
        if (!character) return;
        if (character.characterMovementManager.isSprinting) return;

        staminaRegenTimer += Time.deltaTime;

        if (staminaRegenTimer >= staminaRegenDelay.Value)
        {
            if (currentStamina.Value < maxStamina.Value)
            {
                float regenRate = staminaRegenPerSec.Value;
                if(character.characterCombatManager.inCombat.Value)
                {
                    regenRate *= inCombatStaminaRegenScaler;
                }

                float smoothRegenAmount = regenRate * Time.deltaTime;
                currentStamina.Value = Mathf.Min(maxStamina.Value, currentStamina.Value + smoothRegenAmount);
            }
        }
    }
    public virtual void RegenerateMana()
    {
        if (!character) return;

        manaRegenTimer += Time.deltaTime;
        if (manaRegenTimer >= manaRegenDelay.Value)
        {
            if (currentMana.Value < maxMana.Value)
            {
                float regenRate = manaRegenPerSecond.Value;
                if (character.characterCombatManager.inCombat.Value)
                {
                    regenRate *= inCombatManaRegenScaler;
                }

                float smoothRegenAmount = regenRate * Time.deltaTime;

                currentMana.Value = Mathf.Min(maxMana.Value, currentMana.Value + smoothRegenAmount);
            }
        }
    }
    public virtual void ResetStaminaRegenTimer(float oldValue, float newValue)
    {
        //reset stamina regen timer only if we used stamina -> stamina went down
        //dont reset if stamina is already going up 
        if (newValue < oldValue)
        {
            staminaRegenTimer = 0;
        }

    }
    public virtual void ResetManaRegenTimer(float oldValue, float newValue)
    {
        //reset mana regen timer only if we used mana -> mana went down
        //dont reset if already going up 
        if (newValue < oldValue)
        {
            manaRegenTimer = 0;
        }

    }
    #endregion

    #region Use Resource Functions
    public virtual bool TryConsumeHealth(float amount)
    {
        if (currentHealth.Value >= amount)
        {
            currentHealth.Value -= amount;
            return true;
        }
        return false;
    }
    public virtual bool TryConsumeMana(float amount)
    {
        if (currentMana.Value >= amount)
        {
            currentMana.Value -= amount;
            return true;
        }
        return false;
    }
    public virtual bool TryConsumeStamina(float amount)
    {
        if (currentStamina.Value >= amount)
        {
            currentStamina.Value -= amount;
            return true;
        }
        return false;
    }
    #endregion

    #region Armour
    public float CalculateArmorMitigation(float incomingDamage, Element element)
    {
        if (incomingDamage <= 0) return 0f;

        float effectiveArmor = 0f;

        switch (element)
        {
            // Default armour only applies to "Physical" damage
            case Element.Blood:
            case Element.Stone:
                effectiveArmor = armourAmount.Value;
                break;

            //other elements only get armour mitigation with affixes 
            case Element.Bile:
            case Element.Fire:
            case Element.Ice:
            case Element.Spark:
                effectiveArmor = armourAmount.Value * (armourAppliedToAllElements.Value / 100f);
                break;
        }

        if (effectiveArmor <= 0) return 0f;

        float mitigation = effectiveArmor / (effectiveArmor + (incomingDamage * armorTuningFactor));

        return Mathf.Min(mitigation, maxArmorMitigation);
    }

    public float GetEstimatedArmorMitigation()
    {
        float currentArmor = armourAmount.Value;
        if (currentArmor <= 0) return 0f;

        float expectedIncomingDamage = averageDamageReceivedBase + (character.characterLevel * damageRecievedIncreasePerLevel);

        float estimatedMitigation = currentArmor / (currentArmor + (expectedIncomingDamage * armorTuningFactor));

        return Mathf.Min(estimatedMitigation, maxArmorMitigation);
    }
    #endregion

    #region Evasion
    /// <summary>
    /// Read the characters evasion amounts, and return the % chance to evade
    /// </summary>
    /// <returns></returns>
    public float CalculateEvasionChance(int attackerLevel)
    {
        float currentEvasion = evasionRating.Value;

        if (currentEvasion <= 0) return 0f;

        // take the evasion rating, and scale it by character level of the attacker
        // need more evasion rating agaisnt higher level opponents
        float expectedAccuracy = attackerLevel * evasionScalingConstant;

        //logarithmic curve, VERY high diminishing returns aropund the 85% range:
        // Evasion / (Evasion + ExpectedAccuracy)
        float evasionChance = (currentEvasion / (currentEvasion + expectedAccuracy)) * 100f;

        //hard cap in case its REALLY high 
        return Mathf.Min(evasionChance, maxEvasionChance);
    }
    /// <summary>
    /// Determine if the evade occurs for this hit, determined by evasion entropy and characters evasion chance
    /// </summary>
    /// <param name="evasionChance"></param>
    /// <returns></returns>
    public bool CheckEvaded(float evasionChance)
    {
        bool evadedAttack = false;

        // need to check how long since we took damage, or keep a running timer in update on the evasion entropy
        if(timeSinceLastEvasionCheck >= evasionResetWindow)
        {
            // if entropy needs to be rolled, roll  it here
            evasionEntropy = Random.Range(0, 100);
        }

        timeSinceLastEvasionCheck = 0;

        float chanceToHit = 100 - evasionChance;
        evasionEntropy += chanceToHit;

        if(evasionEntropy >= 100)
        {
            evasionEntropy -= 100f;
            evadedAttack = false;
        }
        else
        {
            evadedAttack = true;
        }

        return evadedAttack;
    }

    private void TrackEvasionEntropy()
    {
        //called in update
        timeSinceLastEvasionCheck += Time.deltaTime;
    }
    #endregion

    #region Shield
    public virtual void RegenerateShield()
    {
        if (!character) return;
        if(character.isDead.Value) return;

        shieldRegenTimer += Time.deltaTime;
        if (shieldRegenTimer >= shieldRegenDelay.Value)
        {
            if (currentShield.Value < shieldAmount.Value)
            {
                float regenRate = shieldRegenPerSec.Value;
                if (character.characterCombatManager.inCombat.Value)
                {
                    regenRate *= inCombatShieldRegenScaler;
                }

                float smoothRegenAmount = regenRate * Time.deltaTime;

                currentShield.Value = Mathf.Min(shieldAmount.Value, currentShield.Value + smoothRegenAmount);
            }
        }
    }
    public virtual void ResetShieldRegenTimer(float oldValue, float newValue)
    {
        //reset shield regen timer only if we lose shield -> shield went down
        //dont reset if already going up 
        if (newValue < oldValue)
        {
            shieldRegenTimer = 0;
        }

    }
    #endregion

    #region Elemental Resistance and Thresholds
    public float GetResistance(Element element)
    {
        switch (element)
        {
            case Element.Blood: return bloodDefense.Value;
            case Element.Bile: return bileDefense.Value;
            case Element.Fire: return fireDefense.Value;
            case Element.Ice: return iceDefense.Value;
            case Element.Spark: return sparkDefense.Value;
            case Element.Stone: return stoneDefense.Value;
            default: return 0f;
        }
    }
    public float GetElementalDamageTaken(Element element)
    {
        switch (element)
        {
            case Element.Blood: return bloodDamageTakenMult.Value;
            case Element.Bile: return bileDamageTakenMult.Value;
            case Element.Fire: return fireDamageTakenMult.Value;
            case Element.Ice: return iceDamageTakenMult.Value;
            case Element.Spark: return sparkDamageTakenMult.Value;
            case Element.Stone: return stoneDamageTakenMult.Value;
            default: return 0f;
        }
    }

    public float GetAilmentThreshold(Element element)
    {
        switch (element)
        {
            case Element.Blood: return bloodThreshold.Value;
            case Element.Bile: return bileThreshold.Value;
            case Element.Fire: return fireThreshold.Value;
            case Element.Ice: return iceThreshold.Value;
            case Element.Spark: return sparkThreshold.Value;
            case Element.Stone: return stoneThreshold.Value;
            default: return 100f;
        }
    }
    public void SyncStatusThresholds()
    {
        if (character.statusManager != null)
        {
            character.statusManager.SetThresholds(
                bloodThreshold.Value,
                bileThreshold.Value,
                fireThreshold.Value,
                iceThreshold.Value,
                sparkThreshold.Value,
                stoneThreshold.Value
            );
        }
    }

    public float GetMaxResistance(Element element)
    {
        switch (element)
        {
            case Element.Blood: return maxBloodResistance.Value;
            case Element.Bile: return maxBileResistance.Value;
            case Element.Fire: return maxFireResistance.Value;
            case Element.Ice: return maxIceResistance.Value;
            case Element.Spark: return maxSparkResistance.Value;
            case Element.Stone: return maxStoneResistance.Value;
            default: return 100f;
        }
    }

    #endregion

    #region Damage
    public float GetElementalDamageModifier(Element element)
    {
        switch (element)
        {
            case Element.Blood: return bloodDamageMult.Value;
            case Element.Bile: return bileDamageMult.Value;
            case Element.Fire: return fireDamageMult.Value;
            case Element.Ice: return iceDamageMult.Value;
            case Element.Spark: return sparkDamageMult.Value;
            case Element.Stone: return stoneDamageMult.Value;
            default: return 1f;
        }
    }
    #endregion

    #region Sustain Functions
    public void CalculateResourceSteal(float finalDamageDealt)
    {
        if (finalDamageDealt <= 0) return;

        if (lifeStealAmount.Value > 0)
        {
            float stealAmount = finalDamageDealt * (lifeStealAmount.Value / 100f);
            currentHealth.Value = Mathf.Min(maxHealth.Value, currentHealth.Value + stealAmount);
        }

        if (manaStealAmount.Value > 0)
        {
            float stealAmount = finalDamageDealt * (manaStealAmount.Value / 100f);
            currentMana.Value = Mathf.Min(maxMana.Value, currentMana.Value + stealAmount);
        }

        if (staminaStealAmount.Value > 0)
        {
            float stealAmount = finalDamageDealt * (staminaStealAmount.Value / 100f);
            currentStamina.Value = Mathf.Min(maxStamina.Value, currentStamina.Value + stealAmount);
        }

        if (shieldStealAmount.Value > 0)
        {
            float stealAmount = finalDamageDealt * (shieldStealAmount.Value / 100f);
            currentShield.Value = Mathf.Min(shieldAmount.Value, currentShield.Value + stealAmount);
        }
    }
    public void CalculateResourceOnHit()
    {
        if (lifeOnHit.Value > 0)
            currentHealth.Value = Mathf.Min(maxHealth.Value, currentHealth.Value + lifeOnHit.Value);

        if (manaOnHit.Value > 0)
            currentMana.Value = Mathf.Min(maxMana.Value, currentMana.Value + manaOnHit.Value);

        if (staminaOnHit.Value > 0)
            currentStamina.Value = Mathf.Min(maxStamina.Value, currentStamina.Value + staminaOnHit.Value);

        if (shieldOnHit.Value > 0)
            currentShield.Value = Mathf.Min(shieldAmount.Value, currentShield.Value + shieldOnHit.Value);
    }
    public void RegenerateHealth()
    {
        if (character.isDead.Value) return;

        float regenRate = flatLifeRegenPerSec.Value;

        if (percentLifeRegenPerSec.Value > 0)
        {
            regenRate += maxHealth.Value * (percentLifeRegenPerSec.Value / 100f);
        }

        if (regenRate > 0 && currentHealth.Value < maxHealth.Value)
        {
            currentHealth.Value = Mathf.Min(maxHealth.Value, currentHealth.Value + (regenRate * Time.deltaTime));
        }
    }
    /// <summary>
    /// Called inside TakeDamage whenever the character loses health/shield.
    /// Pass in the TOTAL final damage the character just absorbed.
    /// </summary>
    public void AddRecoupInstance(float damageAbsorbed)
    {
        if (damageAbsorbed <= 0) return;

        // If we have literally zero recoup stats, don't bother creating an instance
        if (lifeRecoup.Value <= 0 && manaRecoup.Value <= 0 &&
            shieldRecoup.Value <= 0 && staminaRecoup.Value <= 0) return;

        activeRecoups.Add(new DamageRecoupInstance
        {
            remainingTime = recoupDuration.Value,

            // (Damage * (Recoup% / 100)) / Duration
            lifePerSecond = (damageAbsorbed * (lifeRecoup.Value / 100f)) / recoupDuration.Value,
            manaPerSecond = (damageAbsorbed * (manaRecoup.Value / 100f)) / recoupDuration.Value,
            shieldPerSecond = (damageAbsorbed * (shieldRecoup.Value / 100f)) / recoupDuration.Value,
            staminaPerSecond = (damageAbsorbed * (staminaRecoup.Value / 100f)) / recoupDuration.Value
        });
    }

    /// <summary>
    ///  Processes all active recoup instances of all types
    /// </summary>
    public void CalculateRecoupEffects()
    {
        if (character.isDead.Value || activeRecoups.Count == 0) return;

        float totalLifeThisFrame = 0f;
        float totalManaThisFrame = 0f;
        float totalShieldThisFrame = 0f;
        float totalStaminaThisFrame = 0f;

        // gop through list in reverse so the oldest effects are removed first and doesnt break fo rloop 
        for (int i = activeRecoups.Count - 1; i >= 0; i--)
        {
            DamageRecoupInstance instance = activeRecoups[i];

            totalLifeThisFrame += instance.lifePerSecond * Time.deltaTime;
            totalManaThisFrame += instance.manaPerSecond * Time.deltaTime;
            totalShieldThisFrame += instance.shieldPerSecond * Time.deltaTime;
            totalStaminaThisFrame += instance.staminaPerSecond * Time.deltaTime;

            instance.remainingTime -= Time.deltaTime;

            if (instance.remainingTime <= 0)
            {
                activeRecoups.RemoveAt(i);
            }
            else
            {
                // Update the struct in the list with the new remaining time
                activeRecoups[i] = instance;
            }
        }

        // Apply accumulated totals, respecting maximum caps
        if (totalLifeThisFrame > 0 && currentHealth.Value < maxHealth.Value)
            currentHealth.Value = Mathf.Min(maxHealth.Value, currentHealth.Value + totalLifeThisFrame);

        if (totalManaThisFrame > 0 && currentMana.Value < maxMana.Value)
            currentMana.Value = Mathf.Min(maxMana.Value, currentMana.Value + totalManaThisFrame);

        if (totalShieldThisFrame > 0 && currentShield.Value < shieldAmount.Value)
            currentShield.Value = Mathf.Min(shieldAmount.Value, currentShield.Value + totalShieldThisFrame);

        if (totalStaminaThisFrame > 0 && currentStamina.Value < maxStamina.Value)
            currentStamina.Value = Mathf.Min(maxStamina.Value, currentStamina.Value + totalStaminaThisFrame);
    }

    #endregion

}