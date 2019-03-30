using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    /// <summary>
    /// quick resetter for various purposes, when an object requires manual reset
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Basics/Resetter")]
    public class J_Resetter : ScriptableObject
    {
        //the elements to be reset
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private ScriptableObject[] _scriptableObjects;

        /// <summary>
        /// used to reset all elements
        /// </summary>
        public void ResetAll() { ResetThisGroup(_scriptableObjects); }

        private void ResetThisGroup(ScriptableObject[] group)
        {
            for (int i = 0; i < group.Length; i++)
            {
                if (group[i] is iResettable)
                {
                    ((iResettable) group[i]).ResetThis();
                    JLog.Log("Resetting object \"" + group[i], JLogTags.Reset, this);
                }
                else
                {
                    JLog.Warning("The scriptable object \"" + group[i] + "\" is marked for reset, but is not an iResettable",
                                     JLogTags.Reset, this);
                }
            }
        }
    }
}
