using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Collections
{
    public abstract class J_ReactiveStack<T> : J_Service, ICollection, IReadOnlyCollection<T>
    {
        // --------------- EVENTS --------------- //
        private Action<T> OnPop;
        private Action<T> OnPush;

        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true), SerializeField] private int _maxLength = 10;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private T[] _arrayStack;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _index;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Count => _index;

        // --------------- MAIN COMMANDS --------------- //
        protected override void ActivateThis()
        {
            base.ActivateThis();
            _arrayStack = new T[_maxLength];
            _index      = 0;
        }

        public void Push(T item)
        {
            if (_index < _maxLength)
            {
                JLog.Error($"{name} is full with {_index + 1} elements. Cancel push.", JLogTags.Collection, this);
                return;
            }

            _arrayStack[_index] = item;
            _index++;
            OnPush?.Invoke(item);
        }

        public T Pop()
        {
            if (_index < 0)
            {
                JLog.Error($"{name} is empty. Cancel pop.", JLogTags.Collection, this);
                return default;
            }

            T item = Peek();
            _arrayStack[_index - 1] = default;
            _index--;
            return item;
        }

        public T Peek() => _arrayStack[_index - 1];

        public void Clear()
        {
            for (int i = _index - 1; i >= 0; i--) _arrayStack[i] = default;

            _index = 0;
        }

        public override void ResetThis()
        {
            base.ResetThis();
            Clear();
        }

        // --------------- QUEUE IMPLEMENTATION --------------- //
        public bool IsSynchronized => false;
        public object SyncRoot => false;

        public void CopyTo(Array array, int index)
        {
            var newArray                                        = (T[]) array;
            for (int i = 0; i < Count; i++) newArray[index + i] = _arrayStack[i];
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++) yield return _arrayStack[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
