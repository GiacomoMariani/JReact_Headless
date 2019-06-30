using System;
using Sirenix.OdinInspector;

namespace JReact.Conditions
{
    /// <summary>
    /// a condition that may be attached to anything, it starts checking automatically at the first subscriber
    /// </summary>
    public abstract class J_ReactiveCondition : J_Service, jObservableValue<bool>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private J_ReactiveBool _condition;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool Current
        {
            get
            {
                UpdateCondition();
                return _condition.Current;
            }
            set => _condition.Current = value;
        }
        
        // --------------- INSTANTIATION --------------- //
        public static T CreateCondition<T>()
            where T : J_ReactiveCondition
        {
            var condition = CreateInstance<T>();
            condition._condition = CreateInstance<J_ReactiveBool>();
            return condition;
        }

        // --------------- ACTIVATION --------------- //
        protected override void ActivateThis()
        {
            if (_condition == null) _condition = CreateInstance<J_ReactiveBool>();
            Current = false;
            StartCheckingCondition();
            base.ActivateThis();
        }
        
        protected override void EndThis()
        {
            base.EndThis();
            StopCheckingCondition();
        }
        
        // --------------- IMPLEMENTATION --------------- //
        protected abstract void StartCheckingCondition();
        protected abstract void StopCheckingCondition();

        //used to update the condition before checking it
        protected virtual void UpdateCondition() {}

        
        // --------------- SUBSCRIBERS --------------- //
        //helpers to make this more readable
        public void SubscribeToCondition(Action<bool> action) { Subscribe(action); }

        public void UnSubscribeToCondition(Action<bool> action) { UnSubscribe(action); }

        public void Subscribe(Action<bool> action)
        {
            if (!IsActive) Activate();
            _condition.Subscribe(action);
        }

        public void UnSubscribe(Action<bool> action)
        {
            _condition.UnSubscribe(action); 
            if(!_condition.HasListeners) End();
        }

        // --------------- OPERATORS OVERLOAD --------------- //
        public static bool operator true(J_ReactiveCondition item) => item.Current;
        public static bool operator false(J_ReactiveCondition item) => !item.Current;

        public static implicit operator bool(J_ReactiveCondition condition) => condition.Current;
    }
}
