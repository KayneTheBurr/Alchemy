using System;
using UnityEngine;

namespace KE
{
    [Serializable]
    public class ObservableVariable
    {
        public enum VarType { Float, Int, Bool, String }

        [SerializeField] private VarType _type = VarType.Float;
        public VarType Type => _type;

        //var types
        [SerializeField] private float _f;
        [SerializeField] private int _i;
        [SerializeField] private bool _b;
        [SerializeField] private string _s = "";

        //events by type
        public event Action<float, float> OnFloatChanged;
        public event Action<int, int> OnIntChanged;
        public event Action<bool, bool> OnBoolChanged;
        public event Action<string, string> OnStringChanged;

        public event Action<VarType, object, object> OnValueChanged;


        public ObservableVariable() { }
        public ObservableVariable(float initial) { _type = VarType.Float; _f = initial; }
        public ObservableVariable(int initial) { _type = VarType.Int; _i = initial; }
        public ObservableVariable(bool initial) { _type = VarType.Bool; _b = initial; }
        public ObservableVariable(string initial) { _type = VarType.String; _s = initial ?? ""; }

        //Get Fucntions
        public float GetFloat() => _f;
        public int GetInt() => _i;
        public bool GetBool() => _b;
        public string GetString() => _s;



        public void SetFloat(float v)
        {
            if (_type != VarType.Float)
            {
                Debug.LogWarning("Set Float called on a non float Observable Variable!");
                return;
            }

            if (Mathf.Approximately(_f, v)) return;

            float old = _f; _f = v;
            OnFloatChanged?.Invoke(old, _f);
            OnValueChanged?.Invoke(VarType.Float, old, _f);
        }
        public void SilentSetFloat(float v) { _f = v; }

        public void SetInt(int v)
        {
            if (_type != VarType.Int)
            {
                Debug.LogWarning("Set Int called on a non int Observable Variable!");
                return;
            }
            if (_i == v) return;
            int old = _i; _i = v;
            OnIntChanged?.Invoke(old, _i);
            OnValueChanged?.Invoke(VarType.Int, old, _i);
        }
        public void SilentSetInt(int v) { _i = v; }

        public void SetBool(bool v)
        {
            if (_type != VarType.Bool)
            {
                Debug.LogWarning("Set Bool called on a non bool Observable Variable!");
                return;
            }
            if (_b == v) return;
            bool old = _b; _b = v;
            OnBoolChanged?.Invoke(old, _b);
            OnValueChanged?.Invoke(VarType.Bool, old, _b);
        }
        public void SilentSetBool(bool v) { _b = v; }

        public void SetString(string v)
        {
            if (_type != VarType.String)
            {
                Debug.LogWarning("Set String called on a non string Observable Variable!");
                return;
            }
            if (string.Equals(_s, v, StringComparison.Ordinal)) return;
            string old = _s; _s = v ?? "";
            OnStringChanged?.Invoke(old, _s);
            OnValueChanged?.Invoke(VarType.String, old, _s);
        }
        public void SilentSetString(string v) { _s = v; }


        public void ForceNotify()
        {
            switch (_type)
            {
                case VarType.Float: OnFloatChanged?.Invoke(_f, _f); OnValueChanged?.Invoke(VarType.Float, _f, _f); break;
                case VarType.Int: OnIntChanged?.Invoke(_i, _i); OnValueChanged?.Invoke(VarType.Int, _i, _i); break;
                case VarType.Bool: OnBoolChanged?.Invoke(_b, _b); OnValueChanged?.Invoke(VarType.Bool, _b, _b); break;
                case VarType.String: OnStringChanged?.Invoke(_s, _s); OnValueChanged?.Invoke(VarType.String, _s, _s); break;
            }
        }
    }
}
