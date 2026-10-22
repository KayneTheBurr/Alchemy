using KE;
using UnityEngine;

public class AIEquipmentManager : CharacterEquipmentManager
{
    AICharacterManager aiCharacter;

    protected override void Awake()
    {
        base.Awake();
        aiCharacter = GetComponent<AICharacterManager>();
    }
}
