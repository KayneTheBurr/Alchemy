using System.Collections.Generic;
using UnityEngine;

namespace KE
{
    public abstract class SkillExecution : MonoBehaviour
    {
        protected ActiveSkillInstance skillData;
        protected CharacterManager character;

        public virtual void Initialize(ActiveSkillInstance activeSkill, CharacterManager characterManager, IDamageable target = null)
        {
            skillData = activeSkill;
            character = characterManager;
        }

        /// <summary>
        /// Calculate and applies all hit damage, DoT, and buildup to a target
        /// </summary>
        protected virtual void ApplySkillDamageAndStatus(IDamageable target, bool isAOESplash = false)
        {
            if (target == null || skillData == null) return;

            //Core Damage Dictionary
            Dictionary<Element, float> corePayload = skillData.GetCoreElementalDamageValues(character);

            // Buckets for damage pass throughs
            Dictionary<Element, float> hitDamage = skillData.GetCalculatedHitDamage(corePayload, character, isAOESplash);
            Dictionary<Element, float> dotPayload = skillData.GetCalculatedDOTPayload(corePayload, character);
            Dictionary<Element, float> buildup = skillData.GetCalculatedBuildup(corePayload, character);

            foreach (var kvp in hitDamage)
            {
                Element el = kvp.Key;
                float damageAmount = kvp.Value;
                float buildupAmount = buildup.ContainsKey(el) ? buildup[el] : 0f;
                float dotAmount = dotPayload.ContainsKey(el) ? dotPayload[el] : 0f;

                if (damageAmount > 0)
                {
                    target.TakeDamage(damageAmount, el, character);
                }

                if (buildupAmount > 0)
                {
                    // Passes the DoT payload seamlessly to the StatusManager
                    target.ApplyBuildup(el, buildupAmount, dotAmount, character);
                }
            }
        }
    }
}
