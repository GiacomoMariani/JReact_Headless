using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.StateControl.LevelSystem
{
    /// <summary>
    /// a level is like a state, an entity may reach or move out of a given level
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Level System/Level")]
    public class J_LevelState : J_State, iResettable
    {
        public virtual string LevelName => name;

        [BoxGroup("Setup", true, true, 0), SerializeField] private int _experienceNeeded;
        public int ExperienceNeeded { get => _experienceNeeded; private set => _experienceNeeded = value; }

        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public int PreviousExperience { get; private set; }
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public int TotalExperienceNeeded
            => PreviousExperience + ExperienceNeeded;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public bool IsMaxLevel { get; private set; }

        private void SanityCheck() { Assert.IsTrue(ExperienceNeeded > 0, "One level experience is not positive. Level: " + name); }

        // --------------- SETUP COMMANDS --------------- //
        internal void SetPreviousExperience(int experience) { PreviousExperience = experience; }

        //used to set this as max level
        internal void SetMaxLevel(bool max) { IsMaxLevel = max; }

        //helper to set the max experience on this
        public void SetExperienceNeeded(int experience)
        {
            ExperienceNeeded = experience;
            SanityCheck();
        }
    }
}
