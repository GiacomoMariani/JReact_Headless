using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Conditions
{
    /// <summary>
    /// a condition that may be attached to anything
    /// </summary>
    public abstract class J_ReactiveCondition : J_State, iObservableValue<bool>
    {
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        public J_ReactiveItem<bool> _Condition = ScriptableObject.CreateInstance<J_ReactiveItem<bool>>();
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        public bool CurrentValue { get => _Condition.CurrentValue; set => _Condition.CurrentValue = value; }

        public static T CreateCondition<T>()
            where T : J_ReactiveCondition
        {
            var condition = CreateInstance<T>();
            return condition;
        }
        
        public override void Activate()
        {
            base.Activate();
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

        public void UnSubscribeToCondition(JGenericDelegate<bool> action) { UnSubscribe(action);}

        public void Subscribe(JGenericDelegate<bool> action)
        {
            if (!IsActive) Activate();
            _Condition.Subscribe(action);
        }

        public void UnSubscribe(JGenericDelegate<bool> action) { _Condition.UnSubscribe(action); }
    }
    
    public class StubCondition : J_ReactiveCondition
    {
        protected override void StartCheckingCondition() { }
        protected override void StopCheckingCondition() { }
    }
}
