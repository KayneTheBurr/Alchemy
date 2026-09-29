using System;
using System.Collections.Generic;
using UnityEngine;

namespace KE
{
    [System.Serializable]
    public class StatModifier
    {
        public float Value;
        public StatModType Type;
        public object Source; //source of the mult

        public StatModifier(float value, StatModType type, object source = null)
        {
            Value = value;
            Type = type;
            Source = source;
        }
    }
}
