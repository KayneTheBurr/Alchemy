using KE;
using UnityEngine;

public class AIInventoryManager : CharacterInventoryManager
{
    AICharacterManager aiCharacter;

    protected override void Awake()
    {
        base.Awake();
        aiCharacter = GetComponent<AICharacterManager>();
    }
}
