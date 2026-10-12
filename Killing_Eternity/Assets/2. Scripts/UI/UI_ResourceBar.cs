using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KE
{
    public class UI_ResourceBar : MonoBehaviour
    {
        protected Slider slider;
        protected RectTransform rectTransform;

        [Header("Text Value Display")]
        public TextMeshProUGUI resourceText;

        protected virtual void Awake()
        {
            slider = GetComponent<Slider>();
            rectTransform = GetComponent<RectTransform>();
        }
        protected virtual void Start()
        {

        }

        public virtual void SetStat(float newValue)
        {
            slider.value = newValue;
            UpdateTextDisplay();
        }

        public virtual void SetMaxStat(float maxValue)
        {

            slider.maxValue = maxValue;
            slider.value = maxValue;
            UpdateTextDisplay();

            //refresh player ui hud in their layer group 
            if (PlayerUIManager.instance) PlayerUIManager.instance.playerHUDManager.RefreshHUD();
        }

        private void UpdateTextDisplay()
        {
            if (resourceText != null)
            {
                resourceText.text = $"{Mathf.RoundToInt(slider.value)} / {Mathf.RoundToInt(slider.maxValue)}";
            }
        }
    }
}
