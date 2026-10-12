using UnityEngine;

namespace KE
{
    public abstract class SkillExecution : MonoBehaviour
    {
        protected ActiveSkillInstance skillData;
        protected CharacterManager character;

        public virtual void Initialize(ActiveSkillInstance activeSkill, CharacterManager characterManager)
        {
            skillData = activeSkill;
            character = characterManager;
        }
    }
}
