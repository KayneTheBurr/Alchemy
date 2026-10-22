using System.Security.Cryptography;
using UnityEngine;

namespace KE
{
    public class CharacterCombatManager : MonoBehaviour
    {
        [HideInInspector] public CharacterManager character;

        [Header("Available Skills")]
        public ActiveSkillInstance[] activeSkills = new ActiveSkillInstance[6];

        [Header("Runtime State")]
        public ActiveSkillInstance currentCastingSkill;

        [Header("Combat Transforms")]
        public IDamageable currentTarget;
        public Transform rightWeaponSpawnPoint;
        public Transform leftWeaponSpawnPoint;

        [Header("Combat State")]
        public ObservableVariable<bool> inCombat = new ObservableVariable<bool>(false);
        [Tooltip("Seconds before leaving combat")]
        public float combatTimeoutDuration = 5f;
        public float timeSinceLastCombatAction = 0f;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
            activeSkills = new ActiveSkillInstance[6];
        }

        protected virtual void Start()
        {

        }
        protected virtual void Update()
        {
            if (inCombat.Value)
            {
                timeSinceLastCombatAction += Time.deltaTime;
                if (timeSinceLastCombatAction >= combatTimeoutDuration)
                {
                    inCombat.Value = false;
                }
            }
        }
        public virtual void ResetCombatTimer()
        {
            inCombat.Value = true;
            timeSinceLastCombatAction = 0f;
        }

        public virtual void ExecuteSkill(int index)
        {
            if (index < 0 || index >= activeSkills.Length) return;

            ActiveSkillInstance skill = activeSkills[index];
            if (skill != null)
            {
                skill.AttemptToPerformSkill(character);
            }
        }

        public void AnimationEvent_ExecuteSkill()
        {
            if (currentCastingSkill != null)
            {
                currentCastingSkill.SuccessfullyPerformSkill(character);

                currentCastingSkill = null; 
            }
        }
    }
}
