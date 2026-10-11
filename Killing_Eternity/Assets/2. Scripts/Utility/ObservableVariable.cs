using System;
using System.Collections.Generic;
using UnityEngine;

namespace KE
{
    [Serializable]
    public class ObservableVariable<T>
    {
        //T can be and class (int, float, string, bool)
        [SerializeField] private T _value;
        public event Action<T, T> OnValueChanged;

        public ObservableVariable(T initialValue = default)
        {
            _value = initialValue;
        }

        public T Value
        {
            get => _value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(_value, value)) return;

                T oldValue = _value;
                _value = value;
                OnValueChanged?.Invoke(oldValue, _value);
            }
        }

        public void SetValueSilent(T value)
        {
            _value = value;
        }

        public void ForceNotify()
        {
            OnValueChanged?.Invoke(_value, _value);
        }

        public static implicit operator T(ObservableVariable<T> variable) => variable.Value;

    }   
}
