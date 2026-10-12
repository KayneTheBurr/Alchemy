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
    public CharacterStat bloodDamageMult = new CharacterStat(1f);
    public CharacterStat bileDamageMult = new CharacterStat(1f);
    public CharacterStat fireDamageMult = new CharacterStat(1f);
    public CharacterStat iceDamageMult = new CharacterStat(1f);
    public CharacterStat sparkDamageMult = new CharacterStat(1f);
    public CharacterStat stoneDamageMult = new CharacterStat(1f);

    [Header("Resistances")]
    public CharacterStat bloodDefense = new CharacterStat(0f);
    public CharacterStat bileDefense = new CharacterStat(0f);
    public CharacterStat fireDefense = new CharacterStat(0f);
    public CharacterStat iceDefense = new CharacterStat(0f);
    public CharacterStat sparkDefense = new CharacterStat(0f);
    public CharacterStat stoneDefense = new CharacterStat(0f);

    [Header("Stamina Regeneration")]
    public float staminaRegenTimer = 0;
    public CharacterStat staminaRegenDelay = new CharacterStat(1f);
    public CharacterStat staminaCostModifier = new CharacterStat(1f);
    public CharacterStat staminaRegenPerSec = new CharacterStat(20f);

    [Header("Mana Regeneration")]
    public float manaRegenTimer = 0;
    public CharacterStat manaRegenDelay = new CharacterStat(.25f);
    public CharacterStat manaCostModifier = new CharacterStat(1f);
    public CharacterStat manaRegenPerSecond = new CharacterStat(10f);

    [Header("Additional Combat Multipliers")]
    public CharacterStat critChance = new CharacterStat(0.05f);             // 5% base
    public CharacterStat critDamageMultiplier = new CharacterStat(1.5f);    // 150% base
    public CharacterStat statusBuildupMultiplier = new CharacterStat(1f);   //defualt to using 100% of base damage

    [Header("Speed Stats")]
    public ObservableVariable<float> moveSpeedMultiplier = new ObservableVariable<float>(1.0f);

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
                float smoothRegenAmount = staminaRegenPerSec.Value * Time.deltaTime;

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
                float smoothRegenAmount = manaRegenPerSecond.Value * Time.deltaTime;

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
}
