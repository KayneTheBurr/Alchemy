using UnityEngine;

namespace KE
{
    public class PlayerStatsManager : CharacterStatsManager
    {
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

        [Header("Speed Stats")]
        public ObservableVariable<float> moveSpeedMultiplier = new ObservableVariable<float>(1.0f);



        protected override void OnEnable()
        {
            base.OnEnable();

            strength.OnValueChanged += OnAttributeChanged;
            intelligence.OnValueChanged += OnAttributeChanged;
            dexterity.OnValueChanged += OnAttributeChanged;

            maxHealth.OnValueChanged += UpdateMaxHealth;
            maxStamina.OnValueChanged += UpdateMaxStamina;
            maxMana.OnValueChanged += UpdateMaxMana;

            currentHealth.OnValueChanged += PlayerUIManager.instance.playerHUDManager.SetNewHealthValue;
            currentStamina.OnValueChanged += PlayerUIManager.instance.playerHUDManager.SetNewStaminaValue;
            currentMana.OnValueChanged += PlayerUIManager.instance.playerHUDManager.SetNewManaValue;
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

            currentHealth.OnValueChanged -= PlayerUIManager.instance.playerHUDManager.SetNewHealthValue;
            currentStamina.OnValueChanged -= PlayerUIManager.instance.playerHUDManager.SetNewStaminaValue;
            currentMana.OnValueChanged -= PlayerUIManager.instance.playerHUDManager.SetNewManaValue;
        }


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


        private void UpdateMaxHealth(float newMax)
        {
            int maxInt = Mathf.RoundToInt(newMax);

            PlayerUIManager.instance.playerHUDManager.SetMaxHealthValue(maxInt);

            if (currentHealth > maxInt)
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

    }
}
