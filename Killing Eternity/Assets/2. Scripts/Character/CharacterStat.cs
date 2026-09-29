using System;
using System.Collections.Generic;
using UnityEngine;

namespace KE
{
    public class CharacterStat
    {
        [SerializeField] private float baseValue;

        public float BaseValue
        {
            get => baseValue;
            set
            {
                if (Mathf.Approximately(baseValue, value)) return;
                baseValue = value;
                RecalculateAndNotify();
            }
        }

        public Action<float> OnValueChanged;

        private float lastCalculatedValue;
        private bool isDirty = true; // Only recalculate math if a modifier was added or removed
        private readonly List<StatModifier> statModifiers = new List<StatModifier>();

        public CharacterStat(float initialBaseValue = 0f)
        {
            baseValue = initialBaseValue;
            isDirty = true;
        }

        public float Value
        {
            get
            {
                if (isDirty)
                {
                    lastCalculatedValue = CalculateFinalValue();
                    isDirty = false;
                }
                return lastCalculatedValue;

            }
        }

        public void AddModifier(StatModifier mod)
        {
            statModifiers.Add(mod);
            RecalculateAndNotify();
        }

        public void RemoveAllModifiersFromSource(object source)
        {
            int removedCount = 0;

            // Loop backwards through the list so removing them doesnt cause a problem
            for (int i = statModifiers.Count - 1; i >= 0; i--)
            {
                StatModifier mod = statModifiers[i];

                // remove all modifiers that came from this source
                if (mod.Source == source)
                {
                    statModifiers.RemoveAt(i);
                    removedCount++;
                }
            }

            // if we removbed something, need to recalc
            if (removedCount > 0)
            {
                RecalculateAndNotify();
            }
        }

        private void RecalculateAndNotify()
        {
            float oldValue = lastCalculatedValue;
            lastCalculatedValue = CalculateFinalValue();
            isDirty = false;

            if (!Mathf.Approximately(oldValue, lastCalculatedValue))
            {
                OnValueChanged?.Invoke(lastCalculatedValue);
            }
        }


        private float CalculateFinalValue()
        {
            float finalValue = baseValue;
            float percentAddSum = 0f;

            // assess all modifiers on the stat
            foreach (StatModifier mod in statModifiers)
            {
                if (mod.Type == StatModType.FlatAdd)
                {
                    finalValue += mod.Value;
                }
                else if (mod.Type == StatModType.PercentAdd)
                {
                    percentAddSum += mod.Value;
                }
            }

            // Apply additive multipliers
            finalValue *= (1.0f + percentAddSum);

            // apply multiplicitive mults here at the end
            foreach (StatModifier mod in statModifiers)
            {
                if (mod.Type == StatModType.PercentMult)
                {
                    finalValue *= mod.Value;
                }
            }

            return (float)Math.Round(finalValue, 4);
        }
    }
}
