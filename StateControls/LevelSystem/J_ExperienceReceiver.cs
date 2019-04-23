﻿using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.StateControl.LevelSystem
{
    /// <summary>
    /// the experience of a single element
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Level System/Experience Receiver")]
    public class J_ExperienceReceiver : ScriptableObject, iFillable, iActivable
    {
        #region FIELDS AND PROPERTIES
        private event Action<int> OnGain;
        private event Action<int> OnMaxChanged;

        [BoxGroup("State", true, true, 0), SerializeField, AssetsOnly, Required] private J_LevelProgression _levelProgress;

        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] private int _currentExperience;
        public int Current
        {
            get => _currentExperience;
            private set
            {
                _currentExperience = value;
                OnGain?.Invoke(_currentExperience);
            }
        }
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public bool IsActive { get; private set; }
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public int Max
            => _levelProgress.CurrentLevelInfo.ExperienceNeeded;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] private bool _CanGainExperience
            => !_levelProgress.MaxLevelReached;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public int FreeCapacity => Max - Current;

        [BoxGroup("Debug", true, true, 1000), SerializeField] private bool _debug;
        #endregion

        #region CREATION
        public static J_ExperienceReceiver Create(J_LevelProgression levelProgress)
        {
            var experience = CreateInstance<J_ExperienceReceiver>();
            experience._levelProgress = levelProgress;
            return experience;
        }
        #endregion

        #region TRACK METHODS
        public void Activate()
        {
            if (IsActive)
            {
                JLog.Warning($"{name} is tracking already. Cancel command.", JLogTags.LevelSystem, this);
                return;
            }

            SanityChecks();

            IsActive     = true;
            Current = 0;

            _levelProgress.Subscribe(LevelUpdate);
        }

        private void SanityChecks() { Assert.IsNotNull(_levelProgress, $"{name} requires a _levelProgress"); }

        private void LevelUpdate((J_LevelState previous, J_LevelState current) transition)
        {
            OnMaxChanged?.Invoke(transition.current.ExperienceNeeded);
            Current = 0;
        }
        #endregion

        #region EXPERIENCE PROGRESS
        /// <summary>
        /// grants an amount of experience to the player
        /// </summary>
        /// <param name="amountToAdd">the amount to be given</param>
        public int Grant(int amountToAdd)
        {
            if (_debug)
                JLog.Log($"{name} Granted Experience: {amountToAdd}. Current: {Current}. For next Level: {Max}",
                             JLogTags.LevelSystem, this);

            // --------------- CHECKERS --------------- //
            if (!CanAdd(amountToAdd)) return -1;

            // --------------- KEEP ADDING UNTIL WE HAVE EXPERIENCE--------------- //
            //recursively add experience
            while (amountToAdd >= FreeCapacity)
            {
                // --------------- STOP AT MAX --------------- //
                if (!_CanGainExperience)
                {
                    Current = _levelProgress.CurrentLevelInfo.ExperienceNeeded;
                    return -1;
                }

                amountToAdd -= FreeCapacity;
                //if we have a level up we will also get the event of UpdateMax and the MaxCapacity will be directly updated
                _levelProgress.GainLevel();
            }

            Current = amountToAdd;

            return 0;
        }

        public int Remove(int amount)
        {
            JLog.Error($"{name} cannot be removed", JLogTags.LevelSystem, this);
            return -1;
        }

        //checks if the command is valid and sends some log if not
        private bool CanAdd(int amountToAdd)
        {
            Assert.IsTrue(Max > 0,
                          $"{name} max experience needs to be higher than 0 for {_levelProgress.CurrentLevelInfo.name}. Value {Max}");

            if (Max <= 0) return false;
            Assert.IsTrue(amountToAdd > 0, $"{name} requires a positive value. Value: {amountToAdd}");
            if (amountToAdd <= 0) return false;
            if (!_CanGainExperience)
            {
                //the command could be valid reached this point, but a warning could be useful
                JLog.Warning($"{name} is max level, cannot grant {amountToAdd} experience");
                return false;
            }

            return true;
        }
        
        public void End()
        {
            if (!IsActive)
            {
                JLog.Warning($"{name} is not tracking. Cancel command.", JLogTags.LevelSystem, this);
                return;
            }

            _levelProgress.UnSubscribe(LevelUpdate);
            IsActive = false;
        }
        #endregion

        #region SUBSCRIBERS
        public void Subscribe(Action<int> action) { OnGain                      += action; }
        public void UnSubscribe(Action<int> action) { OnGain                    -= action; }
        public void SubscribeToMaxCapacity(Action<int> action) { OnMaxChanged   += action; }
        public void UnSubscribeToMaxCapacity(Action<int> action) { OnMaxChanged -= action; }
        #endregion

        #region DISABLE AND RESET
        private void OnDisable()
        {
            if (!IsActive) ResetThis();
        }

        public void ResetThis() {if (IsActive) End(); }
        #endregion
    }
}
