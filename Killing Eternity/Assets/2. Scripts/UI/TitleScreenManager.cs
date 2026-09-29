using System.Collections;
using UnityEngine;

namespace KE
{
    public class TitleScreenManager : MonoBehaviour
    {
        [Header("Press Start Menu")]
        public CanvasGroup pressStartCanvasGroup;
        private Coroutine fadeOutPressStartCoroutine;
        public float pressStartPolishDelayTime = 0.5f;
        public float pressStartFadeTime = 1f;

        [Header("Character Menu")]
        [SerializeField] GameObject[] characterMenuButtons;
        public CanvasGroup characterMenuCanvasGroup;
        private Coroutine fadeInCharacterMenuCoroutine;
        public float characterMenuFadeInDelay = 1f;
        public float characterMenuFadeTime = 1f;


        //Press Start Menu
        public void StartGame()
        {
            //do what we need to start the project up: spawn player in now?


            //Fade the title out
            FadeOutPressStartMenu();
            FadeInCharacterMenu();
        }
        private void FadeOutPressStartMenu()
        {
            if(fadeOutPressStartCoroutine != null) StopCoroutine(fadeOutPressStartCoroutine);

            fadeOutPressStartCoroutine = StartCoroutine(FadeOutPressStartMenuCoroutine(pressStartFadeTime));
        }
        private IEnumerator FadeOutPressStartMenuCoroutine(float duration)
        {
            float timeElapsed = 0;
            float startingAlpha = pressStartCanvasGroup.alpha;

            //on button pressed things(SFX, vfx)

            //delay while sfx/vfx play
            yield return new WaitForSeconds(pressStartPolishDelayTime);

            while(timeElapsed < duration)
            {
                pressStartCanvasGroup.alpha = Mathf.Lerp(startingAlpha, 0, timeElapsed /  duration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            pressStartCanvasGroup.alpha = 0;
            yield return new WaitForSeconds(characterMenuFadeInDelay);
        }


        //Character Menu
        public void StartNewCharacter()
        {
            WorldSaveGameManager.instance.StartNewCharacter();
        }
        private void FadeInCharacterMenu()
        {
            if (fadeInCharacterMenuCoroutine != null) StopCoroutine(fadeInCharacterMenuCoroutine);

            fadeInCharacterMenuCoroutine = StartCoroutine(FadeInCharactertMenuCoroutine(characterMenuFadeTime));

            for(int i = 0; i < characterMenuButtons.Length; i++)
            {
                if (characterMenuButtons[i] == null) continue;
                characterMenuButtons[i].SetActive(true);
            }
        }
        private IEnumerator FadeInCharactertMenuCoroutine(float duration)
        {
            yield return new WaitForSeconds(pressStartPolishDelayTime + characterMenuFadeInDelay);

            float timeElapsed = 0;
            float startingAlpha = 0;

            while (timeElapsed < duration)
            {
                characterMenuCanvasGroup.alpha = Mathf.Lerp(startingAlpha, 1, timeElapsed / duration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            characterMenuCanvasGroup.alpha = 1;
        }
    }
}
