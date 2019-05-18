﻿using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    /// <summary>
    /// a base reactive item, that can be be registered to, so the subscriber will be able to track its changes
    /// </summary>
    public class J_ReactiveItem<T> : ScriptableObject, iResettable, jObservableValue<T>
    {
        private event Action<T> OnPropertyChange;

        //optionally set a starting value
        [BoxGroup("Setup", true, true, 0), SerializeField] protected T _startValue;
        protected T _currentValue;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool HasListeners => OnPropertyChange == null;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public virtual T Current
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                OnPropertyChange?.Invoke(value);
            }
        }

        public virtual void Subscribe(Action<T> actionToSend) => OnPropertyChange += actionToSend;
        public virtual void UnSubscribe(Action<T> actionToSend) => OnPropertyChange -= actionToSend;

        public virtual void ResetThis() => _currentValue = _startValue;
    }
}
