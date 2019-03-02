using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact
{
    /// <summary>
    /// acts as a container with a min and max value
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Basics/Fillable", fileName = "Fillable")]
    public class J_Fillable : ScriptableObject, iResettable, iFillable
    {
        #region FIELDS AND PROPERTIES
        private event JGenericDelegate<int> OnValueChange;
        private event JGenericDelegate<int> OnMaxChanged;
        private event JGenericDelegate<int> OnMinChanged;

        //optionally set a starting value
        [BoxGroup("Setup", true, true, 0), ShowInInspector, SerializeField] protected int _startAmount = 0;
        [BoxGroup("Setup", true, true, 0), ShowInInspector, SerializeField] protected int _startMax    = 0;
        [BoxGroup("Setup", true, true, 0), ShowInInspector, SerializeField] protected int _startMin    = 0;

        private int _currentAmount;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        public int CurrentValue
        {
            get => _currentAmount;
            set
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
        private int _min;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        public int Min
        {
            get => _min;
            set
            {
                if (!CanSetMinCapacity(value))
                {
                    JConsole.Error($"{CurrentState} Cannot set min capacity {value}.", JLogTags.Fillable, this);
                    return;
                }

                _min = value;
                OnMinChanged?.Invoke(value);
            }
        }

        private int _max;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        public int Max
        {
            get => _max;
            set
            {
                if (!CanSetMaxCapacity(value))
                {
                    JConsole.Error($"{CurrentState} Cannot set max capacity {value}.", JLogTags.Fillable, this);
                    return;
                }

                _max = value;
                OnMaxChanged?.Invoke(value);
            }
        }

        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public int FreeCapacity => Max - CurrentValue;
        private string CurrentState
            => $"{name}. Min {Min} - Max {Max} - Current {CurrentValue}";
        #endregion

        #region COMMANDS
        public static J_Fillable CreateInstance(int max, int amount = 0, int min = 0)
        {
            var fillable                                    = CreateInstance<J_Fillable>();
            fillable._startAmount = fillable._currentAmount = amount;
            fillable._startMin    = fillable._min           = min;
            fillable._startMax    = fillable._max           = max;
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
                int remaining = CurrentValue + amount - Max;
                CurrentValue = Max;
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
                int remaining = amount - (CurrentValue + Min);
                CurrentValue = Min;
                return remaining;
            }

            CurrentValue -= amount;
            return 0;
        }
        #endregion

        #region PUBLIC CHECKS
        /// <summary>
        /// checks if an amount can be added to this fillable
        /// </summary>
        /// <param name="amount">the amount we want to add</param>
        /// <returns>returns true if the request is valid</returns>
        public bool CanAdd(int amount) { return (CurrentValue + amount) <= Max; }

        /// <summary>
        /// checks if there's enough value
        /// </summary>
        /// <param name="amount">the amount we want to remove</param>
        /// <returns>returns true if we have anough/returns>
        public bool HasEnough(int amount) { return CurrentValue >= amount; }

        public bool CanSetMaxCapacity(int maxToSet) { return CurrentValue <= maxToSet && Min   <= maxToSet; }
        public bool CanSetMinCapacity(int minToSet) { return CurrentValue >= minToSet && Max   >= minToSet; }
        public bool CanSetValue(int       value)    { return value        >= Min      && value <= Max; }
        #endregion

        #region SUBSCRIBERS AND LISTENERS
        public virtual void Subscribe(JGenericDelegate<int>   action) { OnValueChange += action; }
        public virtual void UnSubscribe(JGenericDelegate<int> action) { OnValueChange -= action; }

        public virtual void SubscribeToMaxCapacity(JGenericDelegate<int>   action) { OnMaxChanged += action; }
        public virtual void UnSubscribeToMaxCapacity(JGenericDelegate<int> action) { OnMaxChanged -= action; }

        public virtual void SubscribeToMinCapacity(JGenericDelegate<int>   action) { OnMinChanged += action; }
        public virtual void UnSubscribeToMinCapacity(JGenericDelegate<int> action) { OnMinChanged -= action; }

        public virtual void ResetThis()
        {
            _currentAmount = _startAmount;
            _max           = _startMax;
            _min           = _startMin;
            SanityChecks();
        }

        public bool SanityChecks()
        {
            Assert.IsTrue(CurrentValue <= Max, $"{name} has a current value {CurrentValue} higher than max {Max}");
            Assert.IsTrue(CurrentValue >= Min, $"{name} has a current value {CurrentValue} lower than min {Min}");
            Assert.IsTrue(Min          <= Max, $"{name} has a min {Min} higher than max {Max}");
            if (CurrentValue > Max) return false;
            if (CurrentValue < Min) return false;
            if (Min          > Max) return false;

            return true;
        }

        protected virtual void OnDisable() { ResetThis(); }

        private void OnValidate() { ResetThis(); }
        #endregion
    }
}
