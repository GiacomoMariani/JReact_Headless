using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact
{
    /// <summary>
    /// acts as a container with a min and max value
    /// </summary>
    public class J_Fillable : ScriptableObject, iResettable, iFillable
    {
        #region FIELDS AND PROPERTIES
        private event JGenericDelegate<int> OnValueChange;
        private event JGenericDelegate<int> OnMaxChanged;
        private event JGenericDelegate<int> OnMinChanged;

        //optionally set a starting value
        [BoxGroup("Setup", true, true, 0), ShowInInspector, SerializeField] protected int _startAmount = 0;
        [BoxGroup("Setup", true, true, 0), ShowInInspector, SerializeField] protected int _startMax = 0;
        [BoxGroup("Setup", true, true, 0), ShowInInspector, SerializeField] protected int _startMin = 0;

        private int _currentAmount;
        [BoxGroup("View", true, true, 5), ShowInInspector, ReadOnly]
        public int CurrentAmount
        {
            get => _currentAmount;
            private set
            {
                if (!CanSetValue(value))
                {
                    JConsole.Error($"{CurrentState}Cannot set value {value}.", JLogTags.Fillable, this);
                    return;
                }

                _currentAmount = value;
                OnValueChange?.Invoke(value);
            }
        }
        private int _minCapacity;
        [BoxGroup("View", true, true, 5), ShowInInspector, ReadOnly]
        public int MinCapacity
        {
            get => _minCapacity;
            private set
            {
                if (!CanSetMinCapacity(value))
                {
                    JConsole.Error($"{CurrentState}Cannot set min capacity {value}.", JLogTags.Fillable, this);
                    return;
                }

                _minCapacity = value;
                OnMinChanged?.Invoke(value);
            }
        }

        private int _maxCapacity;
        [BoxGroup("View", true, true, 5), ShowInInspector, ReadOnly]
        public int MaxCapacity
        {
            get => _maxCapacity;
            private set
            {
                if (!CanSetMaxCapacity(value))
                {
                    JConsole.Error($"{CurrentState}Cannot set max capacity {value}.", JLogTags.Fillable, this);
                    return;
                }

                _maxCapacity = value;
                OnMaxChanged?.Invoke(value);
            }
        }

        public int FreeCapacity => MaxCapacity - CurrentAmount;
        private string CurrentState => $"{name}. Min {MinCapacity} - Max {MaxCapacity} - Current {CurrentAmount}";
        #endregion

        #region COMMANDS
        public static J_Fillable CreateInstance(int amount, int max, int min)
        {
            var fillable                                    = CreateInstance<J_Fillable>();
            fillable._startAmount = fillable._currentAmount = amount;
            fillable._startMin    = fillable._minCapacity   = amount;
            fillable._startMax    = fillable._maxCapacity   = amount;
            fillable.ResetThis();
            return fillable;
        }

        public bool Grant(int amount)
        {
            JConsole.Log($"{CurrentState} => Granted Amount: {amount}.", JLogTags.Fillable, this);

            if (!CanAdd(amount))
            {
                JConsole.Error($"{CurrentState} invalid amount {amount}. Cancel command.", JLogTags.Fillable, this);
                return false;
            }

            CurrentAmount += amount;
            return true;
        }

        public void SetMax(int max) { MaxCapacity                 = max; }
        public void SetMin(int min) { MinCapacity                 = min; }
        public void SetCurrentAmount(int current) { CurrentAmount = current; }
        #endregion

        #region PUBLIC CHECKS
        /// <summary>
        /// checks if an amount can be added to this fillable
        /// </summary>
        /// <param name="amount">the amount we want to add</param>
        /// <returns>returns true if the request is valid</returns>
        public bool CanAdd(int amount) { return (CurrentAmount + amount) <= MaxCapacity; }

        public bool CanSetMaxCapacity(int maxToSet) { return CurrentAmount <= maxToSet    && MinCapacity <= maxToSet; }
        public bool CanSetMinCapacity(int minToSet) { return CurrentAmount >= minToSet    && MaxCapacity >= minToSet; }
        public bool CanSetValue(int value) { return value                  >= MinCapacity && value       <= MaxCapacity; }
        #endregion

        #region SUBSCRIBERS AND LISTENERS
        public virtual void Subscribe(JGenericDelegate<int> actionToSend) { OnValueChange               += actionToSend; }
        public virtual void UnSubscribe(JGenericDelegate<int> actionToSend) { OnValueChange             -= actionToSend; }
        public virtual void SubscribeToMaxCapacity(JGenericDelegate<int> actionToSend) { OnMaxChanged   += actionToSend; }
        public virtual void UnSubscribeToMaxCapacity(JGenericDelegate<int> actionToSend) { OnMaxChanged -= actionToSend; }
        public virtual void SubscribeToMinCapacity(JGenericDelegate<int> actionToSend) { OnMinChanged   += actionToSend; }
        public virtual void UnSubscribeToMinCapacity(JGenericDelegate<int> actionToSend) { OnMinChanged -= actionToSend; }

        public virtual void ResetThis()
        {
            _currentAmount = _startAmount;
            _maxCapacity   = _startMax;
            _minCapacity   = _startMin;
            SanityChecks(CurrentAmount, MinCapacity, MaxCapacity);
        }

        private void SanityChecks(int value, int min, int max)
        {
            Assert.IsTrue(value <= max, $"{name} has a current value {value} higher than max {max}");
            Assert.IsTrue(value >= min, $"{name} has a current value {value} lower than min {min}");
            Assert.IsTrue(min <= max, $"{name} has a min {min} higher than max {max}");
        }

        protected virtual void OnDisable() { ResetThis(); }
        #endregion
    }
}
