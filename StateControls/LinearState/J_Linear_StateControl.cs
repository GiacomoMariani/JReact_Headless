using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.StateControl.LinearState
{
    public abstract class J_Linear_StateControl<T> : J_Service
        where T : J_State
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private T[] _allStates;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int CurrentIndex { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private J_StateControl<T> _stateControl;

        protected override void ActivateThis()
        {
            JLog.Trace($"{name} ------------------- LINEAR CONTROL START", JLogTags.State, this);
            if (!_allStates.ArrayIsValid() ||
                _allStates.Length < 2)
            {
                JLog.Error($"{name} {nameof(_allStates)} needs at least 2 states. Cancel activation", JLogTags.State, this);
                return;
            }

            _stateControl = J_StateControl<T>.Create(_allStates.SubArray(1, _allStates.Length - 1), _allStates[0]);
            CurrentIndex  = 0;
            base.ActivateThis();
        }

        [Button(ButtonSizes.Medium)]
        public void MoveNext()
        {
            //stop if we reached the last line
            if (CurrentIndex >= _allStates.Length)
            {
                _stateControl.End();
                End();
                return;
            }

            //otherwise move ahead
            _stateControl.SetNewState(_allStates[CurrentIndex]);
            CurrentIndex++;
        }
    }
}
