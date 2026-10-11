using UnityEngine;
using UnityEngine.UI;

namespace KE
{
    public class UI_ResourceBar : MonoBehaviour
    {
        protected Slider slider;
        protected RectTransform rectTransform;

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
        }

        public virtual void SetMaxStat(float maxValue)
        {

            slider.maxValue = maxValue;
            slider.value = maxValue;

            //refresh player ui hud in their layer group 
            if (PlayerUIManager.instance) PlayerUIManager.instance.playerHUDManager.RefreshHUD();
        }
    }
}
