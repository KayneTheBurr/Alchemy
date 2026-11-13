using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace KE
{
    public class PlayerHUDManager : MonoBehaviour
    {
        [SerializeField] CanvasGroup[] fullHUDCanvasGroup;

        [Header("Stat bars")]
        [SerializeField] UI_ResourceBar healthBar;
        [SerializeField] UI_ResourceBar manaBar;
        [SerializeField] UI_ResourceBar staminaBar;
        [SerializeField] UI_ResourceBar shieldBar;

        [Header("Target HP Bar")]
        public CanvasGroup targetHPCanvasGroup;
        public Slider targetHealthSlider;
        public TMP_Text targetNameText;

        private CharacterManager currentCharacter;
        private DamageableEnv damageableObject;

        #region Core Functions
        private void Start()
        {
            ClearTarget();
        }
        private void OnDestroy()
        {
            if (currentCharacter != null && currentCharacter.characterStatsManager != null)
            {
                currentCharacter.characterStatsManager.currentHealth.OnValueChanged -= UpdateTargetHealthBar;
            }
            if(damageableObject != null)
            { 
                damageableObject.currentHealth.OnValueChanged -= UpdateTargetHealthBar;
            }
        }
        #endregion

        #region HUD Controls
        public void RefreshHUD()
        {
            healthBar.gameObject.SetActive(false);
            healthBar.gameObject.SetActive(true);
            manaBar.gameObject.SetActive(false);
            manaBar.gameObject.SetActive(true);
            staminaBar.gameObject.SetActive(false);
            staminaBar.gameObject.SetActive(true);
            shieldBar.gameObject.SetActive(false);
            shieldBar.gameObject.SetActive(true);
        }
        public void ToggleHUD(bool status)
        {
            //fade hud in / out (CURRENTLY NO FADE JUST POP IN OUT )
            if (status)
            {
                foreach (var canvas in fullHUDCanvasGroup)
                {
                    canvas.alpha = 1;
                }

            }
            else
            {
                foreach (var canvas in fullHUDCanvasGroup)
                {
                    canvas.alpha = 0;
                }
            }
        }
        #endregion

        #region Primary Resources
        // --- PRIMARY RESOURCES ---
        public void SetNewHealthValue(float oldHealth, float newHealth)
        {
            healthBar.SetStat(newHealth);
        }
        public void SetMaxHealthValue(float maxHealth)
        {
            healthBar.SetMaxStat(maxHealth);
        }
        public void SetNewStaminaValue(float oldStamina, float newStamina)
        {
            staminaBar.SetStat(newStamina);
        }
        public void SetMaxStaminaValue(float maxStamina)
        {
            staminaBar.SetMaxStat(maxStamina);
        }
        public void SetNewManaValue(float oldMana, float newMana)
        {
            manaBar.SetStat(newMana);
        }
        public void SetMaxManaValue(float maxMana)
        {
            manaBar.SetMaxStat(maxMana);
        }
        #endregion

        #region Shield
        public void SetNewShieldValue(float oldShield, float newShield)
        {
            shieldBar.SetStat(newShield);
        }
        public void SetMaxShieldValue(float maxShield)
        {
            if (maxShield <= 0)
            {
                shieldBar.gameObject.SetActive(false);
            }
            else
            {
                // Only turn it on if it was hidden, then set the value
                if (!shieldBar.gameObject.activeSelf)
                {
                    shieldBar.gameObject.SetActive(true);
                }
                shieldBar.SetMaxStat(maxShield);
            }
        }
        #endregion

        #region Target Health Bar
        public void SetTarget(IDamageable newTarget)
        {
            // Clear old target first
            if (currentCharacter != null && currentCharacter.characterStatsManager != null)
            {
                currentCharacter.characterStatsManager.currentHealth.OnValueChanged -= UpdateTargetHealthBar;
            }
            if (damageableObject != null)
            {
                damageableObject.currentHealth.OnValueChanged -= UpdateTargetHealthBar;
            }

            // hide UI if new target is null or dead
            if (newTarget == null || newTarget.IsDead())
            {
                ClearTarget();
                return;
            }

            currentCharacter = newTarget.GetTransform().GetComponent<CharacterManager>();
            damageableObject = newTarget.GetTransform().GetComponent<DamageableEnv>();

            if (currentCharacter != null && currentCharacter.characterStatsManager != null)
            {
                targetNameText.text = currentCharacter.characterName;

                float maxHealth = currentCharacter.characterStatsManager.maxHealth.Value;
                float currentHealth = currentCharacter.characterStatsManager.currentHealth.Value;

                targetHealthSlider.maxValue = maxHealth;
                targetHealthSlider.value = currentHealth;

                // add listener to look for health changes
                currentCharacter.characterStatsManager.currentHealth.OnValueChanged += UpdateTargetHealthBar;

                // show ui health bar
                targetHPCanvasGroup.alpha = 1f;
                targetHPCanvasGroup.interactable = false;
                targetHPCanvasGroup.blocksRaycasts = false;
            }
            else if (damageableObject != null)
            {
                targetNameText.text = damageableObject.objectName;

                float maxHealth = damageableObject.maxHealth;
                float currentHealth = damageableObject.currentHealth.Value;

                targetHealthSlider.maxValue = maxHealth;
                targetHealthSlider.value = currentHealth;

                // add listener to look for health changes
                damageableObject.currentHealth.OnValueChanged += UpdateTargetHealthBar;

                // show ui health bar
                targetHPCanvasGroup.alpha = 1f;
            }
            else
            {
                ClearTarget();
            }
        }
        public void ClearTarget()
        {
            targetHPCanvasGroup.alpha = 0f;
            currentCharacter = null;
            damageableObject = null;
        }
        private void UpdateTargetHealthBar(float oldHealth, float newHealth)
        {
            targetHealthSlider.value = newHealth;

            // hide if they die while targeted
            if (newHealth <= 0)
            {
                ClearTarget();
            }
        }
        #endregion
    }
}
