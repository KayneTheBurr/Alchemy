using UnityEngine;
namespace KE
{
    public class PlayerUIManager : MonoBehaviour
    {
        public static PlayerUIManager instance;

        [HideInInspector] public PlayerHUDManager playerHUDManager;

        [Header("Active Player")]
        [HideInInspector] public PlayerManager player;

        [Header("UI Flags")]
        public bool menuWindowOpen = false;
        public bool popUpWindowOpen = false;

        private void Awake()
        {
            //one at a time 
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            playerHUDManager = GetComponentInChildren<PlayerHUDManager>();
        }
        public void CloseAllMenuWindows()
        {

        }
    }

    
}
