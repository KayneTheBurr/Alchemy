using UnityEngine;

namespace KE
{
    public class CharacterInventoryManager : MonoBehaviour
    {
        CharacterManager character;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }
    }
}
