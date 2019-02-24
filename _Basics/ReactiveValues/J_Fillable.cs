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
        public int CurrentValue
        {
            get => _currentAmount;
            private set
            {
                if (!CanSetValue(value))
                {
                    JConsole.Error($"{CurrentState} Cannot set value {value}.", JLogTags.Fillable, this);
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
                    JConsole.Error($"{CurrentState} Cannot set min capacity {value}.", JLogTags.Fillable, this);
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
                    JConsole.Error($"{CurrentState} Cannot set max capacity {value}.", JLogTags.Fillable, this);
                    return;
                }

                _maxCapacity = value;
                OnMaxChanged?.Invoke(value);
            }
        }

        public int FreeCapacity => MaxCapacity - CurrentValue;
        private string CurrentState => $"{name}. Min {MinCapacity} - Max {MaxCapacity} - Current {CurrentValue}";
        #endregion

        #region COMMANDS
        public static J_Fillable CreateInstance(int max, int amount = 0, int min = 0)
        {
            var fillable                                    = CreateInstance<J_Fillable>();
            fillable._startAmount = fillable._currentAmount = amount;
            fillable._startMin    = fillable._minCapacity   = min;
            fillable._startMax    = fillable._maxCapacity   = max;
            fillable.ResetThis();
            return fillable;
        }

        /// <summary>
        /// grants an amount and returns the remaining if any
        /// </summary>
        /// <param name="amount">the amount to add</param>
        /// <returns>the remaining, -1 is considered error</returns>
        public int Grant(int amount)
        {
            if (amount <= 0)
            {
                JConsole.Error($"{name} Grant receive only positive amount (use remove). Received {amount}", JLogTags.Fillable, this);
                return -1;
            }

            JConsole.Log($"{CurrentState} => Granted Amount: {amount}.", JLogTags.Fillable, this);

            if (!CanAdd(amount))
            {
                JConsole.Warning($"{CurrentState} invalid amount for grant {amount}. Setting Max.", JLogTags.Fillable, this);
                int remaining = CurrentValue + amount - MaxCapacity;
                CurrentValue = MaxCapacity;
                return remaining;
            }

            CurrentValue += amount;
            return 0;
        }

        /// <summary>
        /// removes an amount and returns the remaining if any
        /// </summary>
        /// <param name="amount">the amount to remove</param>
        /// <returns>the remaining, -1 is considered error</returns>
        public int Remove(int amount)
        {
            if (amount <= 0)
            {
                JConsole.Error($"{name} Remove receive only positive amount. Received {amount}", JLogTags.Fillable, this);
                return -1;
            }

            JConsole.Log($"{CurrentState} => Removed Amount: {amount}.", JLogTags.Fillable, this);

            if (!HasEnough(amount))
            {
                JConsole.Warning($"{CurrentState} invalid amount for remove {amount}. Setting Min.", JLogTags.Fillable, this);
                int remaining = amount - (CurrentValue + MinCapacity);
                CurrentValue = MinCapacity;
                return remaining;
            }

            CurrentValue -= amount;
            return 0;
        }

        public void SetMax(int max) { MaxCapacity                 = max; }
        public void SetMin(int min) { MinCapacity                 = min; }
        public void SetCurrentAmount(int current) { CurrentValue = current; }
        #endregion

        #region PUBLIC CHECKS
        /// <summary>
        /// checks if an amount can be added to this fillable
        /// </summary>
        /// <param name="amount">the amount we want to add</param>
        /// <returns>returns true if the request is valid</returns>
        public bool CanAdd(int amount) { return (CurrentValue + amount) <= MaxCapacity; }

        /// <summary>
        /// checks if there's enough value
        /// </summary>
        /// <param name="amount">the amount we want to remove</param>
        /// <returns>returns true if we have anough/returns>
        public bool HasEnough(int amount) { return CurrentValue >= amount; }

        public bool CanSetMaxCapacity(int maxToSet) { return CurrentValue <= maxToSet    && MinCapacity <= maxToSet; }
        public bool CanSetMinCapacity(int minToSet) { return CurrentValue >= minToSet    && MaxCapacity >= minToSet; }
        public bool CanSetValue(int value) { return value                  >= MinCapacity && value       <= MaxCapacity; }
        #endregion

        #region SUBSCRIBERS AND LISTENERS
        public virtual void SubscribeToWindChange(JGenericDelegate<int> actionToSend) { OnValueChange               += actionToSend; }
        public virtual void UnSubscribeToWindChange(JGenericDelegate<int> actionToSend) { OnValueChange             -= actionToSend; }
        public virtual void SubscribeToMaxCapacity(JGenericDelegate<int> actionToSend) { OnMaxChanged   += actionToSend; }
        public virtual void UnSubscribeToMaxCapacity(JGenericDelegate<int> actionToSend) { OnMaxChanged -= actionToSend; }
        public virtual void SubscribeToMinCapacity(JGenericDelegate<int> actionToSend) { OnMinChanged   += actionToSend; }
        public virtual void UnSubscribeToMinCapacity(JGenericDelegate<int> actionToSend) { OnMinChanged -= actionToSend; }

        public virtual void ResetThis()
        {
            _currentAmount = _startAmount;
            _maxCapacity   = _startMax;
            _minCapacity   = _startMin;
            SanityChecks(CurrentValue, MinCapacity, MaxCapacity);
        }

        private void SanityChecks(int value, int min, int max)
        {
            Assert.IsTrue(value <= max, $"{name} has a current value {value} higher than max {max}");
            Assert.IsTrue(value >= min, $"{name} has a current value {value} lower than min {min}");
            Assert.IsTrue(min   <= max, $"{name} has a min {min} higher than max {max}");
        }

        protected virtual void OnDisable() { ResetThis(); }
        #endregion
    }
}
