using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.StateControls.LevelSystem
{
    //this class stores all the level progress for the player
    [CreateAssetMenu(menuName = "Reactive/Level System/Full Progression")]
    public class J_LevelProgression : J_AsbtractStateControl<J_LevelState>
    {
        #region VALUES AND PROPERTIES
        //-----------------> SETUP CAPACITOR AND CAPACITOR VIEW

        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_ExperienceReceiver _experience;
        

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _currentLevelIndex;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int PreviousExperience { get; private set; }

        // --------------- BOOK KEEPING --------------- //
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public int TotalExperience
            => PreviousExperience + _experience.CurrentAmount;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public int CurrentLevel => _currentLevelIndex + 1;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public int MaxLevel => _validStates.Length    + 1;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public J_LevelState CurrentLevelInfo
            => GetLevelData(CurrentLevel);
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public bool MaxLevelReached => CurrentLevel == MaxLevel;
        #endregion

        public override void Initialize()
        {
            _currentLevelIndex = 0;
            PreviousExperience = 0;
            GetLevelData(MaxLevel).SetMaxLevel();
            base.Initialize();
        }

        public J_LevelState GetLevelData(int level) { return _validStates[level - 1]; }

        /// <summary>
        /// adds a number of levels to the current progression
        /// </summary>
        /// <param name="levelToAdd">the levels to be added</param>
        public void GainLevel(int levelToAdd = 1)
        {
            for (int i = 0; i < levelToAdd; i++)
            {
                if (MaxLevelReached)
                {
                    JConsole.Warning($"{name} has reached max level of {MaxLevel}. Stop raising.", JLogTags.LevelSystem, this);
                    return;
                }

                GainOneLevel();
            }
        }

        //sends all the events related to a single level
        private void GainOneLevel()
        {
            // --------------- STORE PREVIOUS --------------- // 
            PreviousExperience += CurrentLevelInfo.ExperienceNeeded;

            // --------------- SET NEXT LEVEL --------------- //
            //updating the index we also update CurrentLevelInfo
            _currentLevelIndex++;
            //this command will also raise the enter and exit events of the other levels
            SetNewState(CurrentLevelInfo);

            JConsole.Log($"{name} reached level {CurrentLevel}", JLogTags.LevelSystem, this);
        }

        /// <summary>
        /// sets a level directly
        /// </summary>
        /// <param name="level">the level we want to set</param>
        /// <param name="callAllEvents">mostly used for loading, we might not require the events</param>
        public void ForceSetLevel(int level, bool callAllEvents = true)
        {
            // --------------- SAFE CHECKS --------------- //
            //we initialize it forcefully if not initialized, if so we start from level 1
            if (!IsActive) Initialize();
            if (AboveMaxLevel(level)) level = MaxLevel;
            if (AlreadyAboveLevel(level)) return;

            // --------------- SET LEVEL --------------- //
            if (callAllEvents) GainLevel(level - CurrentLevel);
            else _currentLevelIndex = level - 1;
            JConsole.Log($"{name} forced to level {CurrentLevel}", JLogTags.LevelSystem, this);
        }

        private bool AboveMaxLevel(int level)
        {
            Assert.IsTrue(MaxLevel >= level, $"{name} Level {level + 1} is higher than max {MaxLevel}");
            if (level > MaxLevel)
            {
                JConsole.Warning($"{name} Current level {CurrentLevel} is above {level}. Setting Max", JLogTags.LevelSystem, this);
                return true;
            }

            return false;
        }


        private bool AlreadyAboveLevel(int level)
        {
            if (level > MaxLevel)
            {
                JConsole.Warning($"{name} Current level {CurrentLevel} is above {level}. Cancel command.", JLogTags.LevelSystem, this);
                return true;
            }

            return false;
        }
    }
}
