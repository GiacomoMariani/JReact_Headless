using Sirenix.OdinInspector;
using UnityEngine.Assertions;

namespace JReact.Conditions
{
    /// <summary>
    /// a condition that may be attached to anything
    /// </summary>
    public abstract class J_ReactiveCondition : JReactiveBool, iActivable
    {
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _isTracking = false;
        public bool IsActive => _isTracking;

        public void Initialize()
        {
            if (_isTracking)
            {
                JConsole.Warning($"{name} is already tracking.", JLogTags.Conditions, this);
                return;
            }

            if (!_startValue)
                JConsole.Warning($"{name} is a condition and usually it start at false", JLogTags.Conditions, this);
            _currentValue = _startValue;
            _isTracking   = true;
            InitializeCheck();
        }

        protected abstract void InitializeCheck();
        protected abstract void DeInitializeCheck();

        public override void ResetThis()
        {
            base.ResetThis();
            if (!_isTracking) return;
            DeInitializeCheck();
            _isTracking = false;
        }

        public override void Subscribe(JGenericDelegate<bool> actionToSend)
        {
            base.Subscribe(actionToSend);
            if (!_isTracking) Initialize();
        }
    }
}
