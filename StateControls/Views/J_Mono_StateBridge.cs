using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.StateControl
{
    public class J_Mono_StateBridge<T> : MonoBehaviour
        where T : J_State
    {
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_StateControl<T> _controls;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private T _default;

        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private T[] _statesToTrack;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private T _lastState;

        private void Awake() => SanityChecks();

        private void SanityChecks()
        {
            Assert.IsNotNull(_controls,      $"{gameObject.name} requires a {nameof(_controls)}");
            Assert.IsNotNull(_default,       $"{gameObject.name} requires a {nameof(_default)}");
            Assert.IsNotNull(_statesToTrack, $"{gameObject.name} requires a {nameof(_statesToTrack)}");

            Assert.IsTrue(Array.IndexOf(_statesToTrack, _default) > -1,
                          $"{name} {_default.name} needs to be in {nameof(_statesToTrack)}");
        }

        // --------------- COMMAND --------------- //
        private void GoToLast() => _controls.SetNewState(_lastState == null
                                                                  ? _default
                                                                  : _lastState);

        // --------------- STATE CHANGES --------------- //
        private void SetLastState()
        {
            for (int i = 0; i < _statesToTrack.Length; i++)
            {
                if (!_statesToTrack[i].IsActive) continue;

                _lastState = _statesToTrack[i];
                return;
            }
        }

        //exits if all the states are inactive
        private void ExitWhenAllDone()
        {
            for (int i = 0; i < _statesToTrack.Length; i++)
                if (_statesToTrack[i].IsActive) return;
            gameObject.SetActive(false);
        }
        
        // --------------- LISTENER SETUP --------------- //
        private void OnEnable()
        {
            GoToLast();
            _statesToTrack.SubscribeToAll(SetLastState);
            _statesToTrack.SubscribeToAllEnd(ExitWhenAllDone);
        }

        

        
        
        private void OnDisable()
        {
            _statesToTrack.UnSubscribeToAll(SetLastState); 
            _statesToTrack.UnSubscribeToAllEnd(ExitWhenAllDone);
        }
        
    }
}
