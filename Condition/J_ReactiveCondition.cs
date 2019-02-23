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
        public void SubscribeToCondition(JGenericDelegate<bool> action) { Subscribe(action); }

        public void UnSubscribeToCondition(JGenericDelegate<bool> action) { UnSubscribe(action); }

        public void Subscribe(JGenericDelegate<bool> action)
        {
            if (!IsActive) Activate();
            _condition.Subscribe(action);
        }

        public void UnSubscribe(JGenericDelegate<bool> action) { _condition.UnSubscribe(action); }

        #region OPERATORS
        public static bool operator true(J_ReactiveCondition item) { return item.CurrentValue; }
        public static bool operator false(J_ReactiveCondition item) { return !item.CurrentValue; }
        #endregion
    }
}
