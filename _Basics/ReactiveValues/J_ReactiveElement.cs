using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    /// <summary>
    /// this is the base reactive element, that can be be registered to, so the subscriber will be able to track its changes
    /// </summary>
    public class J_ReactiveElement<T> : ScriptableObject, iResettable, iObservable<T>
    {
        private event JGenericDelegate<T> OnPropertyChange;

        //optionally set a starting value
        [BoxGroup("Setup", true, true, 0), ShowInInspector, SerializeField] protected T _startValue = default(T);
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
        public virtual void Subscribe(JGenericDelegate<T> actionToSend) { OnPropertyChange   += actionToSend; }
        public virtual void UnSubscribe(JGenericDelegate<T> actionToSend) { OnPropertyChange -= actionToSend; }

        public virtual void ResetThis() { _currentValue = _startValue; }
        protected virtual void OnDisable() { ResetThis(); }
        #endregion
    }
}
