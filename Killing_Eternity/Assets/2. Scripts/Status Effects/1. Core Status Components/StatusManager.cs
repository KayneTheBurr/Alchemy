using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KE
{
    public class StatusManager : MonoBehaviour
    {
        public IDamageable myEntity;

        [Header("Ailment Thresholds")]
        [Tooltip("Characters sync this via StatsManager. Walls use these raw values.")]
        public CharacterStat bleedThreshold = new CharacterStat(100f);
        public CharacterStat toxinThreshold = new CharacterStat(100f);
        public CharacterStat burnThreshold = new CharacterStat(100f);
        public CharacterStat chillThreshold = new CharacterStat(100f);
        public CharacterStat shockThreshold = new CharacterStat(100f);
        public CharacterStat sunderThreshold = new CharacterStat(100f);

        public List<StatusEffect> activeEffects = new List<StatusEffect>();
        public Dictionary<Element, float> buildups = new Dictionary<Element, float>();
        //public Dictionary<Element, float> thresholds = new Dictionary<Element, float>();

        [Header("Stacking Rules")]
        public int bleedStackSizeModifier = 0;
        public int toxinStackSizeModifier = 0;
        public int burnStackSizeModifier = 0;
        public int chillStackSizeModifier = 0;
        public int shockStackSizeModifier = 0;
        public int sunderStackSizeModifier = 0;

        protected virtual void Awake()
        {
            myEntity = GetComponent<IDamageable>();

            foreach (Element el in System.Enum.GetValues(typeof(Element)))
            {
                buildups[el] = 0f; //set the current buildup to zero
            }
        }

        public void SetThresholds(float blood, float bile, float fire, float ice, float spark, float stone)
        {
            bleedThreshold.BaseValue = blood;
            toxinThreshold.BaseValue = bile;
            burnThreshold.BaseValue = fire;
            chillThreshold.BaseValue = ice;
            shockThreshold.BaseValue = spark;
            sunderThreshold.BaseValue = stone;
        }

        public float GetThreshold(Element element)
        {
            switch (element)
            {
                case Element.Blood: return bleedThreshold.Value;
                case Element.Bile: return toxinThreshold.Value;
                case Element.Fire: return burnThreshold.Value;
                case Element.Ice: return chillThreshold.Value;
                case Element.Spark: return shockThreshold.Value;
                case Element.Stone: return sunderThreshold.Value;
                default: return 100f;
            }
        }
        private int GetStackModifier(Status status)
        {
            switch (status)
            {
                case Status.Bleed: return bleedStackSizeModifier;
                case Status.Toxin: return toxinStackSizeModifier;
                case Status.Burn: return burnStackSizeModifier;
                case Status.Chill: return chillStackSizeModifier;
                case Status.Shock: return shockStackSizeModifier;
                case Status.Sunder: return sunderStackSizeModifier;
                default: return 0;
            }
        }

        protected virtual void Update()
        {
            if (myEntity.IsDead()) return;

            for (int i = activeEffects.Count - 1; i >= 0; i--)
            {
                activeEffects[i].UpdateEffect(Time.deltaTime);

                if (activeEffects[i].isFinished)
                {
                    activeEffects[i].RemoveEffect();
                    activeEffects.RemoveAt(i);
                }
            }

            // can add status decay here
        }

        public virtual void AddBuildup(Element element, float amount, float dotAmount, IDamageable source)
        {
            if (myEntity.IsDead()) return;

            buildups[element] += amount;
            //Debug.Log($"Applied {amount} {element} status buildup. " +
            //    $"\n Current buildup: {buildups[element]}" +
            //    $"   Threshold: {GetThreshold(element)}");


            if (buildups[element] >= GetThreshold(element))
            {
                Debug.Log("Trigger Status!");
                buildups[element] = 0f;
                Status incomingStatus = AlchemyEngine.instance.GetBaseStatusForElement(element);
                ApplyStatus(incomingStatus, dotAmount, source);
            }
        }

        private void ApplyStatus(Status incomingStatus, float dotAmount, IDamageable source)
        {
            Status stackingFusion = AlchemyEngine.instance.CheckForStackingEffect(incomingStatus);

            if (stackingFusion != Status.None)
            {
                int baseRequired = AlchemyEngine.instance.GetRequiredStacks(incomingStatus);
                int stackModifier = GetStackModifier(incomingStatus);

                // minimum one stack required 
                int finalRequiredStacks = Mathf.Max(1, baseRequired - stackModifier);

                // get current stacks of the effect
                int currentStacks = activeEffects.Count(e => e.statusType == incomingStatus);

                // does the new stack (+1) exceed meet/exceed our stack limit 
                if (currentStacks + 1 >= finalRequiredStacks)
                {
                    Debug.Log($"Stack Limit Reached: {finalRequiredStacks}x {incomingStatus} triggered {stackingFusion}!");

                    // Consume all existing stacks 
                    for (int i = activeEffects.Count - 1; i >= 0; i--)
                    {
                        if (activeEffects[i].statusType == incomingStatus)
                        {
                            activeEffects[i].RemoveEffect();
                            activeEffects.RemoveAt(i);
                        }
                    }

                    // apply stacked effect then see if that should react to anything else we have 
                    ApplyStatus(stackingFusion, dotAmount, source);
                    return;
                }
            }

            int bestMatchIndex = -1;
            Status bestReaction = incomingStatus;
            int highestPriority = -1;

            for (int i = 0; i < activeEffects.Count; i++)
            {
                Status existingStatus = activeEffects[i].statusType;
                Status reactionResult = AlchemyEngine.instance.CheckForElementalReaction(incomingStatus, existingStatus);

                if (reactionResult != incomingStatus)
                {
                    // alch engine controls priority of mixing elements
                    int priority = AlchemyEngine.instance.GetReactionPriority(reactionResult);

                    if (priority > highestPriority)
                    {
                        highestPriority = priority;
                        bestReaction = reactionResult;
                        bestMatchIndex = i;
                    }
                }
            }

            // if we have a valid reaction, use that and remove the old effect
            if (bestMatchIndex != -1)
            {
                if (AlchemyEngine.instance.ShouldRemoveBaseEffect(bestReaction))
                {
                    activeEffects[bestMatchIndex].RemoveEffect();
                    activeEffects.RemoveAt(bestMatchIndex);
                }

                //triggers the same check for the new effect created, in case it might react to something else we are affected by
                ApplyStatus(bestReaction, dotAmount, source);
            }

            // if we did NOT find a reaction, just apply the base effect
            else
            {
                // No reactions found, just apply the base effect 
                CreateAndStartEffect(incomingStatus, dotAmount, source);
            }
        }

        protected virtual void CreateAndStartEffect(Status finalStatus, float dotAmount, IDamageable source)
        {
            StatusEffect newEffect = null;

            switch (finalStatus)
            {
                // BASE ELEMENTS
                case Status.Bleed: newEffect = new BleedEffect(); break;
                case Status.Toxin: newEffect = new ToxinEffect(); break;
                case Status.Burn: newEffect = new BurnEffect(); break;
                case Status.Chill: newEffect = new ChillEffect(); break;
                //case Status.Shock: newEffect = new ShockEffect(); break;
                case Status.Sunder: newEffect = new SunderEffect(); break;

                // STACKING REACTIONS
                //case Status.Necrosis: newEffect = new NecrosisEffect(); break;
                //case Status.Brittle: newEffect = new BrittleEffect(); break;
                //case Status.Paralyze: newEffect = new ParalyzeEffect(); break;

                //TIER 2
                case Status.ThermalShock: newEffect = new ThermalShockEffect(); break;
                case Status.Wet: newEffect = new WetEffect(); break;

                // TIER 3
                //case Status.Melt: newEffect = new MeltEffect(); break;
                case Status.Freeze: newEffect = new FrozenEffect(); break;

                default:
                    break;
            }

            if (newEffect != null)
            {
                newEffect.ApplyEffect(myEntity, dotAmount, source);
                activeEffects.Add(newEffect);
                Debug.Log($"Applied {newEffect}");
            }
        }
    }
}
