using UnityEngine;
namespace KE
{
    public class PlayerHUDManager : MonoBehaviour
    {
        [SerializeField] CanvasGroup[] canvasGroup;

        [Header("Stat bars")]
        [SerializeField] UI_ResourceBar healthBar;
        [SerializeField] UI_ResourceBar manaBar;
        [SerializeField] UI_ResourceBar staminaBar;

        public void RefreshHUD()
        {
            healthBar.gameObject.SetActive(false);
            healthBar.gameObject.SetActive(true);
            manaBar.gameObject.SetActive(false);
            manaBar.gameObject.SetActive(true);
            staminaBar.gameObject.SetActive(false);
            staminaBar.gameObject.SetActive(true);
        }
        public void ToggleHUD(bool status)
        {
            //fade hud in / out (CURRENTLY NO FADE JUST POP IN OUT )
            if (status)
            {
                foreach (var canvas in canvasGroup)
                {
                    canvas.alpha = 1;
                }

            }
            else
            {
                foreach (var canvas in canvasGroup)
                {
                    canvas.alpha = 0;
                }
            }
        }

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
    }
}
