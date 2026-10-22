using UnityEngine;

namespace KE
{
    public abstract class BaseAIManager : MonoBehaviour
    {
        [HideInInspector] public AICharacterManager aiCharacter;

        [Header("Targeting & Alignment")]
        public Faction teamAlignment = Faction.Enemy;
        public CharacterManager currentTarget;

        [Header("AI State Tick Rate")]
        [Tooltip("How often the AI evaluates its logic (e.g., 0.1 = 10 times per second)")]
        public float aiTickRate = 0.1f;
        protected float aiTickTimer = 0f;

        protected virtual void Awake()
        {
            aiCharacter = GetComponent<AICharacterManager>();
        }
        protected virtual void Update()
        {
            if (aiCharacter.isDead.Value || aiCharacter.isPerformingAction) return;

            aiTickTimer += Time.deltaTime;
            if (aiTickTimer >= aiTickRate)
            {
                aiTickTimer = 0f;
                ProcessStateMachine();
            }
        }

        protected abstract void ProcessStateMachine();

    }
}
