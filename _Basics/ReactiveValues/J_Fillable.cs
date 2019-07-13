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
    public class J_Fillable : ScriptableObject, iFillable
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        private event Action<int> OnValueChange;
        private event Action<int> OnMaxChanged;
        private event Action<int> OnMinChanged;

        //optionally set a starting value
        [BoxGroup("Setup", true, true, 0), SerializeField] protected int _startAmount;
        [BoxGroup("Setup", true, true, 0), SerializeField] protected int _startMax = 100;
        [BoxGroup("Setup", true, true, 0), SerializeField] protected int _startMin;

        private int _current;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Current
        {
            get => _current;
            set
            {
                if (!CanSetValue(value))
                {
                    JLog.Error($"{CurrentState} Cannot set value {value}.", JLogTags.Fillable, this);
                    return;
                }

                _current = value;
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

        // --------------- INITIATION AND INSTANTIATION --------------- //
        public static J_Fillable CreateInstance(int max, int amount = 0, int min = 0)
        {
            var fillable                              = CreateInstance<J_Fillable>();
            fillable._startAmount = fillable._current = amount;
            fillable._startMin    = fillable._min     = min;
            fillable._startMax    = fillable._max     = max;
            fillable.InitiateFillable();
            return fillable;
        }

        public virtual void InitiateFillable()
        {
            _current = _startAmount;
            _max     = _startMax;
            _min     = _startMin;
            SanityChecks();
        }

        public bool SanityChecks()
        {
            Assert.IsTrue(Current <= Max, $"{name} has a current value {Current} higher than max {Max}");
            Assert.IsTrue(Current >= Min, $"{name} has a current value {Current} lower than min {Min}");
            Assert.IsTrue(Min     <= Max, $"{name} has a min {Min} higher than max {Max}");
            if (Current > Max) return false;
            if (Current < Min) return false;
            if (Min     > Max) return false;

            return true;
        }

        // --------------- COMMANDS --------------- //
        /// <summary>
        /// grants an amount and returns the remaining if going above max
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
        /// removes an amount and returns the remaining if going below min
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
        public bool CanAdd(int amount) => Current + amount <= Max;
        public bool HasEnough(int amount) => Current       >= amount;
        public int HowMuchToReach(int amount) => amount - Current;

        public bool CanSetMaxCapacity(int maxToSet) => Current <= maxToSet && Min   <= maxToSet;
        public bool CanSetMinCapacity(int minToSet) => Current >= minToSet && Max   >= minToSet;
        public bool CanSetValue(int value) => value            >= Min      && value <= Max;

        // --------------- SUBSCRIBERS AND LISTENERS --------------- //
        public virtual void Subscribe(Action<int> action) { OnValueChange   += action; }
        public virtual void UnSubscribe(Action<int> action) { OnValueChange -= action; }

        public virtual void SubscribeToMaxCapacity(Action<int> action) { OnMaxChanged   += action; }
        public virtual void UnSubscribeToMaxCapacity(Action<int> action) { OnMaxChanged -= action; }

        public virtual void SubscribeToMinCapacity(Action<int> action) { OnMinChanged   += action; }
        public virtual void UnSubscribeToMinCapacity(Action<int> action) { OnMinChanged -= action; }
    }
}
