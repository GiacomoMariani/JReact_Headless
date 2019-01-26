using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    /// <summary>
    /// this is the base reactive element, that can be be registered to, so the subscriber will be able to track its changes
    /// </summary>
    public class J_ReactiveElement<T> : ScriptableObject, iResettable, iObservable<T>
    {
        //the event raised by this property
        private event JGenericDelegate<T> OnPropertyChange;

        //optionally set a starting value
        [BoxGroup("Setup", true, true, 0), ShowInInspector, SerializeField] protected T _startValue = default(T);
        //the current value to track, sends the event when changed
        protected T _currentValue;
        [BoxGroup("View", true, true, 5), ShowInInspector, ReadOnly]
        public virtual T CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                OnPropertyChange?.Invoke(value);
            }
        }
        #region SUBSCRIBERS AND LISTENERS
        //a way to subscribe and unsubscribe to this property
        public virtual void Subscribe(JGenericDelegate<T> actionToSend) { OnPropertyChange   += actionToSend; }
        public virtual void UnSubscribe(JGenericDelegate<T> actionToSend) { OnPropertyChange -= actionToSend; }

        //reset methods
        public virtual void ResetThis() { _currentValue = _startValue; }
        protected virtual void OnDisable() { ResetThis(); }
        #endregion
    }
}
