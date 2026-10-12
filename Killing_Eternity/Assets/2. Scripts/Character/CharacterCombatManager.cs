using UnityEngine;

namespace KE
{
    public class CharacterCombatManager : MonoBehaviour
    {
        public CharacterManager character;

        [Header("Available Skills")]
        public ActiveSkillInstance[] activeSkills = new ActiveSkillInstance[6];

        [Header("Combat Transforms")]
        public IDamageable currentTarget;
        public Transform weaponSpawnPoint;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Start()
        {

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
    }
}
