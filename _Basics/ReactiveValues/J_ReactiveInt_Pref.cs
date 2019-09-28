using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    [CreateAssetMenu(menuName = "Reactive/Basics/Reactive Int Pref")]
    public sealed class J_ReactiveInt_Pref : J_ReactiveInt
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true), SerializeField, Required] private string _prefName;
        public override int Current
        {
            get
            {
                if (PlayerPrefs.HasKey(_prefName)) return PlayerPrefs.GetInt(_prefName);
                PlayerPrefs.SetInt(_prefName, _startValue);
                return _startValue;
            }
            set
            {
                PlayerPrefs.SetInt(_prefName, value);
                base.Current = value;
            }
        }
    }
}
