using System.Collections.Generic;
using UnityEditor.SceneManagement;
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
            if (primarySkill == null || primarySkill.baseSkill == null) return;

            primarySkill.activeElement = primarySkill.baseSkill.skillElement;
            primarySkill.activeAnimation = primarySkill.baseSkill.skillAnimation;
            primarySkill.activePrefab = primarySkill.baseSkill.skillPrefab;

            primarySkill.activeTags = new List<SkillTag>(primarySkill.baseSkill.skillTags);

            primarySkill.activeSkillCost.RemoveAllModifiersFromSource(this);
            primarySkill.activeDamageMultiplier.RemoveAllModifiersFromSource(this);
            primarySkill.activeAoeRadius.RemoveAllModifiersFromSource(this);

            primarySkill.activeResourceType = primarySkill.baseSkill.baseResourceType;
            isSecondarySlotUnlocked = false;

            //REmvoe all modifiers to the skill from this socket
            primarySkill.activeSkillCost.RemoveAllModifiersFromSource(this);
            primarySkill.activeDamageMultiplier.RemoveAllModifiersFromSource(this);

            //NODE EFFECT TYPES: (UPDATE with ENUM changes)
                    //StatModifier,
                    //UnlockSecondarySlot,
                    //Capstone,
                    //ElementalConversion,
                    //AddTag, RemoveTag,
                    //PrefabOverride, AnimationOverride,
                    //ResourceConversion

            //Get all local effects from Patterns
            foreach (SkillNodeEffect effect in allocatedLocalEffects)
            {
                switch (effect.effectType)
                {
                    case NodeEffectType.StatModifier:
                        ApplyStatEffect(effect);
                        break;

                    case NodeEffectType.UnlockSecondarySlot:
                        if (secondarySkill != null)
                        {
                            GetLogicFromSecondarySkillSlot();
                        }
                        break;

                    case NodeEffectType.ElementalConversion:
                        primarySkill.activeElement = effect.newElement;
                        break;

                    case NodeEffectType.AnimationOverride:
                        primarySkill.activeAnimation = effect.newAnimationString;
                        break;

                    case NodeEffectType.PrefabOverride:
                        primarySkill.activePrefab = effect.newPrefab;
                        break;

                    case NodeEffectType.AddTag:
                        if (!primarySkill.activeTags.Contains(effect.tagModifier))
                        {
                            primarySkill.activeTags.Add(effect.tagModifier);
                        }
                        break;

                    case NodeEffectType.RemoveTag:
                        primarySkill.activeTags.Remove(effect.tagModifier);
                        break;

                    case NodeEffectType.ResourceConversion:
                        primarySkill.activeResourceType = effect.newSkillResource;
                        break;

                    case NodeEffectType.Capstone:
                        ApplyMechanicOverride(effect);
                        break;
                }
            }
            
        }

        private void ApplyStatEffect(SkillNodeEffect effect)
        {
            // pass "this" so that it gets wiped the next time it is recalc'd
            StatModifier mod = new StatModifier(effect.modifierValue, effect.modType, this);

            //This switch statement will be giant, and a single script should likely hold just this switch statement?
            switch (effect.statType)
            {
                case StatType.Damage:
                    primarySkill.activeDamageMultiplier.AddModifier(mod);
                    break;
                case StatType.ProjectileSpeed:
                    primarySkill.activeProjectileSpeed.AddModifier(mod);
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
