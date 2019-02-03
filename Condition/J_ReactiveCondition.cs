using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Conditions
{
    /// <summary>
    /// a condition that may be attached to anything
    /// </summary>
    public abstract class J_ReactiveCondition : J_State
    {
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        public J_ReactiveElement<bool> _Condition = CreateInstance<J_ReactiveElement<bool>>();
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        public bool CurrentValue { get => _Condition.CurrentValue; set => _Condition.CurrentValue = value; }

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

        public void SubscribeToCondition(JGenericDelegate<bool> actionToSend)
        {
            if (!IsActive) Activate();
            _Condition.Subscribe(actionToSend);
        }

        public void UnSubscribeToCondition(JGenericDelegate<bool> actionToSend) { _Condition.UnSubscribe(actionToSend); }
    }
}
