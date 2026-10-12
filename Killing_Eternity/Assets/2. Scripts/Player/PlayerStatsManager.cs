using UnityEngine;

namespace KE
{
    public class PlayerStatsManager : CharacterStatsManager
    {
        #region Variables
        PlayerManager player;

        [Header("Player Level")]
        public int playerLevel = 1;

        [Header("Core Attributes")]
        public CharacterStat strength = new CharacterStat(5);
        public CharacterStat intelligence = new CharacterStat(5);
        public CharacterStat dexterity = new CharacterStat(5);

        [Header("Resoruces per Attribute")]
        public int healthPerStrength = 5;
        public int staminaPerDexterity = 5;
        public int manaPerIntelligence = 5;



        #endregion

        #region Core Functions
        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }
        protected override void OnEnable()
        {
            base.OnEnable();

            strength.OnValueChanged += OnAttributeChanged;
            intelligence.OnValueChanged += OnAttributeChanged;
            dexterity.OnValueChanged += OnAttributeChanged;

            maxHealth.OnValueChanged += UpdateMaxHealth;
            maxStamina.OnValueChanged += UpdateMaxStamina;
            maxMana.OnValueChanged += UpdateMaxMana;
        }
        protected override void Start()
        {
            currentHealth.OnValueChanged += PlayerUIManager.instance.playerHUDManager.SetNewHealthValue;
            currentStamina.OnValueChanged += PlayerUIManager.instance.playerHUDManager.SetNewStaminaValue;
            currentMana.OnValueChanged += PlayerUIManager.instance.playerHUDManager.SetNewManaValue;

            RecalculateAllAttributes();

            UpdateMaxHealth(maxHealth.Value);
            UpdateMaxStamina(maxStamina.Value);
            UpdateMaxMana(maxMana.Value);

            base.Start();
        }
        protected override void OnDisable()
        {
            base.OnDisable();

            strength.OnValueChanged -= OnAttributeChanged;
            intelligence.OnValueChanged -= OnAttributeChanged;
            dexterity.OnValueChanged -= OnAttributeChanged;

            maxHealth.OnValueChanged -= UpdateMaxHealth;
            maxStamina.OnValueChanged -= UpdateMaxStamina;
            maxMana.OnValueChanged -= UpdateMaxMana;

            if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerHUDManager != null)
            {
                currentHealth.OnValueChanged -= PlayerUIManager.instance.playerHUDManager.SetNewHealthValue;
                currentStamina.OnValueChanged -= PlayerUIManager.instance.playerHUDManager.SetNewStaminaValue;
                currentMana.OnValueChanged -= PlayerUIManager.instance.playerHUDManager.SetNewManaValue;
            }
        }
        #endregion

        private void OnAttributeChanged(float newValue)
        {
            RecalculateAllAttributes();
        }
        public void RecalculateAllAttributes()
        {
            maxHealth.BaseValue = baseHealth + (strength.Value * healthPerStrength);
            maxMana.BaseValue = baseMana + (intelligence.Value * manaPerIntelligence);
            maxStamina.BaseValue = baseStamina + (dexterity.Value * staminaPerDexterity);
        }

        #region Max Resource Update Functions
        private void UpdateMaxHealth(float newMax)
        {
            int maxInt = Mathf.RoundToInt(newMax);

            PlayerUIManager.instance.playerHUDManager.SetMaxHealthValue(maxInt);

            if (currentHealth.Value > maxInt)
            {
                currentHealth.Value = maxInt;
            }
        }
        private void UpdateMaxStamina(float newMax)
        {
            int maxInt = Mathf.RoundToInt(newMax);

            PlayerUIManager.instance.playerHUDManager.SetMaxStaminaValue(maxInt);

            if (currentStamina.Value > maxInt)
            {
                currentStamina.Value = maxInt;
            }
        }
        private void UpdateMaxMana(float newMax)
        {
            int maxInt = Mathf.RoundToInt(newMax);

            PlayerUIManager.instance.playerHUDManager.SetMaxManaValue(maxInt);

            if (currentMana.Value > maxInt)
            {
                currentMana.Value = maxInt;
            }
        }
        #endregion

        #region Use Resource Functions
        public override bool TryConsumeHealth(float amount)
        {
            if (currentHealth.Value >= amount)
            {
                currentHealth.Value -= amount;
                return true;
            }
            return false;
        }
        public override bool TryConsumeMana(float amount)
        {
            if (currentMana.Value >= amount)
            {
                currentMana.Value -= amount;
                return true;
            }
            return false;
        }
        public override bool TryConsumeStamina(float amount)
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
}
