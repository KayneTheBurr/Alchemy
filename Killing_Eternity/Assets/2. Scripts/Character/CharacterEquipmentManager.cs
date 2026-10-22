using UnityEngine;
namespace KE
{
    public class CharacterEquipmentManager : MonoBehaviour
    {
        CharacterManager character;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }
    }
}
