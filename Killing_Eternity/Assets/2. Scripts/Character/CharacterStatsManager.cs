using KE;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    #region Variables
    CharacterManager character;

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

    [Header("Resistances")]
    public CharacterStat bloodDefense = new CharacterStat(0f);
    public CharacterStat bileDefense = new CharacterStat(0f);
    public CharacterStat fireDefense = new CharacterStat(0f);
    public CharacterStat iceDefense = new CharacterStat(0f);
    public CharacterStat sparkDefense = new CharacterStat(0f);
    public CharacterStat stoneDefense = new CharacterStat(0f);

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

    [Header("Additional Combat Multipliers")]
    public CharacterStat critChance = new CharacterStat(5f);             // 5% base
    public CharacterStat critDamageMultiplier = new CharacterStat(1.5f);    // 150% base
    public CharacterStat statusBuildupMultiplier = new CharacterStat(1f);   //defualt to using 100% of base damage

    [Header("Action Speed")]
    public CharacterStat actionSpeed = new CharacterStat(1.0f);

    [Header("Speed Stats")]
    public CharacterStat moveSpeedMultiplier = new CharacterStat(1.0f);

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
    }
    protected virtual void OnDisable()
    {
        currentHealth.OnValueChanged -= CheckHP;
        currentMana.OnValueChanged -= ResetManaRegenTimer;
        currentStamina.OnValueChanged -= ResetStaminaRegenTimer;
    }
    protected virtual void Update()
    {
        RegenerateStamina();
        RegenerateMana();
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
        if (character.isPerformingAction) return;

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
        //if (character.isPerformingAction) return;

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
}
