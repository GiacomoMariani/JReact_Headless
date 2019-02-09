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
        private event JGenericDelegate<int> OnGain;
        private event JGenericDelegate<int> OnMaxChanged;

        [BoxGroup("State", true, true, 0), SerializeField, AssetsOnly, Required] private J_LevelProgression _levelProgress;

        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] private int _currentExperience;
        public int CurrentAmount
        {
            get => _currentExperience;
            private set
            {
                _currentExperience = value;
                OnGain?.Invoke(_currentExperience);
            }
        }
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public bool IsActive { get; private set; } = false;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public int MaxCapacity
            => _levelProgress.CurrentLevelInfo.ExperienceNeeded;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] private bool _CanGainExperience
            => !_levelProgress.MaxLevelReached;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public int FreeCapacity => MaxCapacity - CurrentAmount;

        [BoxGroup("Debug", true, true, 1000), SerializeField] private bool _debug = false;
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
                JConsole.Warning($"{name} is tracking already. Cancel command.", JLogTags.LevelSystem, this);
                return;
            }

            SanityChecks();

            IsActive      = true;
            CurrentAmount = 0;

            _levelProgress.Subscribe(LevelUpdate);
        }

        private void SanityChecks() { Assert.IsNotNull(_levelProgress, $"{name} requires a _levelProgress"); }

        private void LevelUpdate(J_LevelState previousLevel, J_LevelState currentLevel)
        {
            OnMaxChanged?.Invoke(currentLevel.ExperienceNeeded);
            CurrentAmount = 0;
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
                JConsole.Log($"{name} Granted Experience: {amountToAdd}. Current: {CurrentAmount}. For next Level: {MaxCapacity}",
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
                    CurrentAmount = _levelProgress.CurrentLevelInfo.ExperienceNeeded;
                    return -1;
                }

                amountToAdd -= FreeCapacity;
                //if we have a level up we will also get the event of UpdateMax and the MaxCapacity will be directly updated
                _levelProgress.GainLevel();
            }

            CurrentAmount = amountToAdd;

            return 0;
        }

        //checks if the command is valid and sends some log if not
        private bool CanAdd(int amountToAdd)
        {
            Assert.IsTrue(MaxCapacity > 0,
                          $"{name} max experience needs to be higher than 0 for {_levelProgress.CurrentLevelInfo.name}. Value {MaxCapacity}");
            if (MaxCapacity <= 0) return false;
            Assert.IsTrue(amountToAdd > 0, $"{name} requires a positive value. Value: {amountToAdd}");
            if (amountToAdd <= 0) return false;
            if (!_CanGainExperience)
            {
                //the command could be valid reached this point, but a warning could be useful
                JConsole.Warning($"{name} is max level, cannot grant {amountToAdd} experience");
                return false;
            }

            return true;
        }
        #endregion

        #region SUBSCRIBERS
        public void Subscribe(JGenericDelegate<int> action) { OnGain                      += action; }
        public void UnSubscribe(JGenericDelegate<int> action) { OnGain                    -= action; }
        public void SubscribeToMaxCapacity(JGenericDelegate<int> action) { OnMaxChanged   += action; }
        public void UnSubscribeToMaxCapacity(JGenericDelegate<int> action) { OnMaxChanged -= action; }
        #endregion

        #region DISABLE AND RESET
        private void OnDisable()
        {
            if (!IsActive) ResetThis();
        }

        public void ResetThis()
        {
            if (!IsActive)
            {
                JConsole.Warning($"{name} is not tracking. Cancel command.", JLogTags.LevelSystem, this);
                return;
            }

            _levelProgress.UnSubscribe(LevelUpdate);
            IsActive = false;
        }
        #endregion
    }
}
