using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    [CreateAssetMenu(menuName = "Reactive/Basics/Reactive Float Pref")]
    public sealed class J_ReactiveFloat_Pref : J_ReactiveFloat
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true), SerializeField, Required] private string _prefName;
        public override float Current
        {
            get
            {
                if (PlayerPrefs.HasKey(_prefName)) return PlayerPrefs.GetFloat(_prefName);
                PlayerPrefs.SetFloat(_prefName, _startValue);
                return _startValue;
            }
            set
            {
                PlayerPrefs.SetFloat(_prefName, value);
                base.Current = value;
            }
        }
    }
}
