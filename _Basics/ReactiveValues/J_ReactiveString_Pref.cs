using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    [CreateAssetMenu(menuName = "Reactive/Basics/Reactive String Pref")]
    public sealed class J_ReactiveString_Pref : J_ReactiveString
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private string _prefName;
        public override string Current
        {
            get
            {
                if (PlayerPrefs.HasKey(_prefName)) return PlayerPrefs.GetString(_prefName);
                PlayerPrefs.SetString(_prefName, _startValue);
                return _startValue;
            }
            set
            {
                PlayerPrefs.SetString(_prefName, value);
                base.Current = value;
            }
        }
    }
}

