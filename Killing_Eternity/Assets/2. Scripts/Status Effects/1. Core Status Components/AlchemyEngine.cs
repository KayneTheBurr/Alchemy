using UnityEngine;

namespace KE
{
    public class AlchemyEngine : MonoBehaviour
    {
        public static AlchemyEngine instance;

        [Header("Status Tuning")]
        public AlchemyTuningData tuningData;

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        public Status CheckForStackingEffect(Status incomingStatus)
        {
            switch (incomingStatus)
            {
                case Status.Toxin: return Status.Necrosis;
                case Status.Chill: return Status.Brittle;
                case Status.Shock: return Status.Paralyze;
                default: return Status.None;
            }
        }
        // The values here will apply to ALL characters, for character specific limits, reduce the number
        //required inside the status manager script
        public int GetRequiredStacks(Status status)
        {
            switch (status)
            {
                case Status.Toxin: return 5;
                case Status.Chill: return 5;
                case Status.Shock: return 5;
                default: return 999; 
            }
        }

        public Status CheckForElementalReaction(Status incomingEffect, Status existingEffect)
        {
            var output = Status.None;

            switch (incomingEffect)
            {
                // --- BLEED TRIGGERED EFFECTS ---
                case Status.Bleed:
                    switch(existingEffect)
                    {
                        case Status.Bleed: output = Status.Hemmorhage; break;
                        case Status.Shock: output = Status.Galvanize; break;
                        case Status.Necrosis: output = Status.Sepsis; break;
                        case Status.Fissure: output = Status.FissureInfusion; break;
                    }
                    break;

                // --- TOXIN TRIGGERED EFFECTS ---
                case Status.Toxin:
                    switch (existingEffect)
                    {
                        case Status.Freeze: output = Status.Dissolve; break;
                        case Status.Fissure: output = Status.FissureInfusion; break;
                        case Status.Paralyze: output = Status.Contagion; break;
                    }
                    break;

                // --- BURN TRIGGERED EFFECTS ---
                case Status.Burn:
                    switch (existingEffect)
                    {
                        case Status.Chill: output = Status.Wet; break;
                        case Status.Sunder: output = Status.Shell; break;
                        case Status.Necrosis: output = Status.ToxicBlaze; break;
                        case Status.Freeze: output = Status.Melt; break;
                        case Status.Fissure: output = Status.FissureInfusion; break;
                    }
                    break;

                // --- CHILL TRIGGERED EFFECTS ---
                case Status.Chill:
                    switch (existingEffect)
                    {
                        case Status.Burn: output = Status.ThermalShock; break;
                        case Status.Wet: output = Status.Freeze; break;
                        case Status.Molten: output = Status.Petrify; break;
                        case Status.Fissure: output = Status.FissureInfusion; break;
                        case Status.Paralyze: output = Status.Ionize; break;
                        case Status.ToxicBlaze: output = Status.Extinguish; break;
                    }
                    break;

                // --- SHOCK TRIGGERED EFFECTS
                case Status.Shock:
                    switch (existingEffect)
                    {
                        case Status.Bleed: output = Status.Galvanize; break;
                        case Status.Sunder: output = Status.Battery; break;
                        case Status.Fissure: output = Status.FissureInfusion; break;
                    }
                    break;

                // --- SUNDER TRIGGERED EFFECTS ---
                case Status.Sunder:
                    switch (existingEffect)
                    {
                        case Status.Burn: output = Status.Molten; break;
                        case Status.Sunder: output = Status.Fissure; break;
                        case Status.Freeze: output = Status.Shatter; break;
                        case Status.Fissure: output = Status.Stabilize; break;
                        case Status.Sepsis: output = Status.Rupture; break;
                    }
                    break;
                

                default:
                    output = Status.None;
                    break;
            }

            return output;
        }

        public int GetReactionPriority(Status status)
        {
            switch (status)
            {
                // Tier 3
                case Status.Sepsis:
                case Status.ToxicBlaze:
                case Status.Dissolve:
                case Status.Contagion:
                case Status.Melt:
                case Status.Petrify:
                case Status.Extinguish:
                case Status.Ionize:
                case Status.Shatter:
                case Status.Stabilize:
                case Status.Rupture:
                case Status.FissureInfusion:
                    return 30;

                // Tier 2
                case Status.Hemmorhage:
                case Status.Galvanize:
                case Status.Necrosis:
                case Status.Wet:
                case Status.Shell:
                case Status.ThermalShock:
                case Status.Brittle:
                case Status.Freeze:
                case Status.Paralyze:
                case Status.Battery:
                case Status.Fissure:
                case Status.Molten:
                    return 20;

                // Tier 1: Base Elements
                case Status.Bleed:
                case Status.Toxin:
                case Status.Burn:
                case Status.Chill:
                case Status.Shock:
                case Status.Sunder:
                    return 10;

                default:
                    return 0;
            }
        }


        public Status GetBaseStatusForElement(Element element)
        {
            switch (element)
            {
                case Element.Blood: return Status.Bleed;
                case Element.Bile: return Status.Toxin;
                case Element.Fire: return Status.Burn;
                case Element.Ice: return Status.Chill;
                case Element.Spark: return Status.Shock;
                case Element.Stone: return Status.Sunder;
                default: return Status.Bleed;
            }
        }

        public bool ShouldRemoveBaseEffect(Status newEffect)
        {
            switch (newEffect)
            {
                case Status.Electrocute:
                    // add and future effects that do NOT consume the existing status effect here
                    return false;

                default:
                    return true;
            }
        }
    }
}