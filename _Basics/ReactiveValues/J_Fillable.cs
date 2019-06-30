using System;
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
        // --------------- FIELDS AND PROPERTIES --------------- //
        private event Action<int> OnValueChange;
        private event Action<int> OnMaxChanged;
        private event Action<int> OnMinChanged;

        //optionally set a starting value
        [BoxGroup("Setup", true, true, 0), ShowInInspector, SerializeField] protected int _startAmount;
        [BoxGroup("Setup", true, true, 0), ShowInInspector, SerializeField] protected int _startMax;
        [BoxGroup("Setup", true, true, 0), ShowInInspector, SerializeField] protected int _startMin;

        private int _currentAmount;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Current
        {
            get => _currentAmount;
            set
            {
                if (!CanSetValue(value))
                {
                    JLog.Error($"{CurrentState} Cannot set value {value}.", JLogTags.Fillable, this);
                    return;
                }

                _currentAmount = value;
                OnValueChange?.Invoke(value);
            }
        }
        private int _min;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Min
        {
            get => _min;
            set
            {
                if (!CanSetMinCapacity(value))
                {
                    JLog.Error($"{CurrentState} Cannot set min capacity {value}.", JLogTags.Fillable, this);
                    return;
                }

                _min = value;
                OnMinChanged?.Invoke(value);
            }
        }

        private int _max;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Max
        {
            get => _max;
            set
            {
                if (!CanSetMaxCapacity(value))
                {
                    JLog.Error($"{CurrentState} Cannot set max capacity {value}.", JLogTags.Fillable, this);
                    return;
                }

                _max = value;
                OnMaxChanged?.Invoke(value);
            }
        }

        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public int FreeCapacity => Max - Current;
        private string CurrentState => $"{name}. Min {Min} - Max {Max} - Current {Current}";
        
        // --------------- COMMANDS --------------- //
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
                JLog.Error($"{name} Grant receive only positive amount (use remove). Received {amount}", JLogTags.Fillable, this);
                return -1;
            }

            JLog.Log($"{CurrentState} => Granted Amount: {amount}.", JLogTags.Fillable, this);

            if (!CanAdd(amount))
            {
                JLog.Warning($"{CurrentState} invalid amount for grant {amount}. Setting Max.", JLogTags.Fillable, this);
                int remaining = Current + amount - Max;
                Current = Max;
                return remaining;
            }

            Current += amount;
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
                JLog.Error($"{name} Remove receive only positive amount. Received {amount}", JLogTags.Fillable, this);
                return -1;
            }

            JLog.Log($"{CurrentState} => Removed Amount: {amount}.", JLogTags.Fillable, this);

            if (!HasEnough(amount))
            {
                JLog.Warning($"{CurrentState} invalid amount for remove {amount}. Setting Min.", JLogTags.Fillable, this);
                int remaining = amount - (Current + Min);
                Current = Min;
                return remaining;
            }

            Current -= amount;
            return 0;
        }

        // --------------- PUBLIC CHECKS --------------- //
        /// <summary>
        /// checks if an amount can be added to this fillable
        /// </summary>
        /// <param name="amount">the amount we want to add</param>
        /// <returns>returns true if the request is valid</returns>
        public bool CanAdd(int amount) => Current + amount <= Max;

        /// <summary>
        /// checks if there's enough value
        /// </summary>
        /// <param name="amount">the amount we want to remove</param>
        /// <returns>returns true if we have anough/returns>
        public bool HasEnough(int amount) => Current >= amount;

        public bool CanSetMaxCapacity(int maxToSet) => Current <= maxToSet && Min   <= maxToSet;
        public bool CanSetMinCapacity(int minToSet) => Current >= minToSet && Max   >= minToSet;
        public bool CanSetValue(int value) => value                 >= Min      && value <= Max;

        // --------------- SUBSCRIBERS AND LISTENERS --------------- //
        public virtual void Subscribe(Action<int> action) { OnValueChange   += action; }
        public virtual void UnSubscribe(Action<int> action) { OnValueChange -= action; }

        public virtual void SubscribeToMaxCapacity(Action<int> action) { OnMaxChanged   += action; }
        public virtual void UnSubscribeToMaxCapacity(Action<int> action) { OnMaxChanged -= action; }

        public virtual void SubscribeToMinCapacity(Action<int> action) { OnMinChanged   += action; }
        public virtual void UnSubscribeToMinCapacity(Action<int> action) { OnMinChanged -= action; }

        public virtual void ResetThis()
        {
            _currentAmount = _startAmount;
            _max           = _startMax;
            _min           = _startMin;
            SanityChecks();
        }

        public bool SanityChecks()
        {
            Assert.IsTrue(Current <= Max, $"{name} has a current value {Current} higher than max {Max}");
            Assert.IsTrue(Current >= Min, $"{name} has a current value {Current} lower than min {Min}");
            Assert.IsTrue(Min          <= Max, $"{name} has a min {Min} higher than max {Max}");
            if (Current > Max) return false;
            if (Current < Min) return false;
            if (Min          > Max) return false;

            return true;
        }

        protected virtual void OnDisable() => ResetThis(); 

        private void OnValidate() => ResetThis();
    }
}
