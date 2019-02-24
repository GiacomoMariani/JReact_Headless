using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Conditions
{
    /// <summary>
    /// a condition that may be attached to anything
    /// </summary>
    public abstract class J_ReactiveCondition : J_Service, iObservableValue<bool>
    {
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private J_ReactiveBool _condition;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        public bool CurrentValue { get => _condition.CurrentValue; set => _condition.CurrentValue = value; }

        public static T CreateCondition<T>()
            where T : J_ReactiveCondition
        {
            var condition = CreateInstance<T>();
            condition._condition = CreateInstance<J_ReactiveBool>();
            return condition;
        }

        public override void Activate()
        {
            base.Activate();
            if (_condition == null) _condition = CreateInstance<J_ReactiveBool>();
            CurrentValue = false;
            StartCheckingCondition();
        }

        protected abstract void StartCheckingCondition();
        protected abstract void StopCheckingCondition();

        public override void End()
        {
            base.End();
            StopCheckingCondition();
        }

        //helpers to make this more readable
        public void SubscribeToCondition(JGenericDelegate<bool> action) { SubscribeToWindChange(action); }

        public void UnSubscribeToCondition(JGenericDelegate<bool> action) { UnSubscribeToWindChange(action); }

        public void SubscribeToWindChange(JGenericDelegate<bool> action)
        {
            if (!IsActive) Activate();
            _condition.SubscribeToWindChange(action);
        }

        public void UnSubscribeToWindChange(JGenericDelegate<bool> action) { _condition.UnSubscribeToWindChange(action); }

        #region OPERATORS
        public static bool operator true(J_ReactiveCondition item) { return item.CurrentValue; }
        public static bool operator false(J_ReactiveCondition item) { return !item.CurrentValue; }
        #endregion
    }
}
