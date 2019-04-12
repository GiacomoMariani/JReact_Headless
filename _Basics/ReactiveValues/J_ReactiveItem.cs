using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    /// <summary>
    /// this is the base reactive item, that can be be registered to, so the subscriber will be able to track its changes
    /// </summary>
    public class J_ReactiveItem<T> : ScriptableObject, iResettable, iObservableValue<T>
    {
        private event Action<T> OnPropertyChange;

        //optionally set a starting value
        [BoxGroup("Setup", true, true, 0), ShowInInspector, SerializeField] protected T _startValue;
        protected T _currentValue;
        [BoxGroup("View", true, true, 5), ShowInInspector, ReadOnly] public bool HasListeners => OnPropertyChange == null;
        [BoxGroup("View", true, true, 5), ShowInInspector, ReadOnly] public virtual T CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                OnPropertyChange?.Invoke(value);
            }
        }

        public virtual void Subscribe(Action<T> actionToSend) { OnPropertyChange   += actionToSend; }
        public virtual void UnSubscribe(Action<T> actionToSend) { OnPropertyChange -= actionToSend; }

        public virtual void ResetThis()
        {
            _currentValue    = _startValue;
            OnPropertyChange = null;
        }
    }
}
