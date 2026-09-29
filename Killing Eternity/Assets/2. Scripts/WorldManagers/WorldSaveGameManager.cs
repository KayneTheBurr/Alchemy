using UnityEngine;
using UnityEngine.SceneManagement;

namespace KE
{
    public class WorldSaveGameManager : MonoBehaviour
    {
        public static WorldSaveGameManager instance;
        [SerializeField] int worldSceneIndex = 1;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void StartNewCharacter()
        {
            SceneManager.LoadScene(worldSceneIndex);
        }
    }
}
