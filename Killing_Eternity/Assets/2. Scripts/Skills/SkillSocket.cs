using System.Collections.Generic;
using UnityEngine;

namespace KE
{
    [System.Serializable]
    public class SkillSocket
    {
        public int socketID; // 1 - 6, location on the skill tree itself

        [Header("Skills Socketed")]
        public ActiveSkillInstance primarySkill;
        public Skill secondarySkill;

        public bool isSecondarySlotUnlocked = false;


        public SkillSocket(int id)
        {
            socketID = id;
        }
        public void EquipPrimarySkill(Skill newSkillSO)
        {
            if (newSkillSO == null)
            {
                primarySkill = null;
                return;
            }
            primarySkill = new ActiveSkillInstance(newSkillSO);
        }

        public void RecalculateSocketMath(List<SkillNodeEffect> allocatedLocalEffects)
        {
            if (primarySkill == null) return;

            primarySkill.activeResourceType = primarySkill.baseSkill.baseResourceType;
            isSecondarySlotUnlocked = false;

            //REmvoe all modifiers to the skill from this socket
            primarySkill.activeSkillCost.RemoveAllModifiersFromSource(this);
            primarySkill.activeDamageMultiplier.RemoveAllModifiersFromSource(this);


            //Get all local effects from Patterns
            foreach (SkillNodeEffect effect in allocatedLocalEffects)
            {
                if (effect.effectType == NodeEffectType.StatModifier)
                {
                    ApplyStatEffect(effect);
                }
                else if (effect.effectType == NodeEffectType.UnlockSecondarySlot)
                {
                    isSecondarySlotUnlocked = true;
                }
                else if (effect.effectType == NodeEffectType.CapstoneMechanic)
                {
                    ApplyMechanicOverride(effect);
                }
            }
            
            if (isSecondarySlotUnlocked && secondarySkill != null)
            {
                GetLogicFromSecondarySkillSlot();
            }
        }

        private void ApplyStatEffect(SkillNodeEffect effect)
        {
            // pass "this" so that it gets wiped the next time it is recalc'd
            StatModifier mod = new StatModifier(effect.modifierValue, effect.modType, this);

            switch (effect.statType) //Switch statement for every single stat type possible in the game here
            {
                case StatType.Damage:
                    primarySkill.activeSkillCost.AddModifier(mod);
                    break;
                
                default:
                    break;
            }
        }

        private void ApplyMechanicOverride(SkillNodeEffect effect)
        {
            //EX. Blood Magic Local Node? Blood magic to the entire player skills goes where
            if (effect.mechanicID == "BloodMagic_Local")
            {
                primarySkill.activeResourceType = ResourceType.Health;
            }
        }

        public void GetLogicFromSecondarySkillSlot()
        {

        }
    }
}
