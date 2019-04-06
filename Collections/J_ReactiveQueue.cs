using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Collections
{
    public class J_ReactiveQueue<T> : J_Service, ICollection, IReadOnlyCollection<T>, iObservable<T>
    {
        private JGenericDelegate<T> OnDequeue;
        private JGenericDelegate<T> OnEnqueue;

        #region FIELDS AND PROPERTIES
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _maxLength = 10;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private T[] _arrayQueue;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _first = 0;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _last = 0;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Count => _last - _first;
        #endregion

        protected override void ActivateThis()
        {
            base.ActivateThis();
            _arrayQueue = new T[_maxLength];
            _first      = 0;
            _last       = 0;
        }

        public void Enqueue(T item)
        {
            int nextIndex = _last.SumRound(1, _maxLength);
            if (nextIndex == _first)
            {
                JLog.Error($"{name} is full with {_last + 1} elements. Cancel enqueue.", JLogTags.Collection, this);
                return;
            }

            _arrayQueue[_last] = item;
            _last              = nextIndex;
            OnEnqueue?.Invoke(item);
        }

        public T Dequeue()
        {
            if (_first == _last)
            {
                JLog.Error($"{name} is empty. Cancel dequeue.", JLogTags.Collection, this);
                return default;
            }

            T item = Peek();
            _arrayQueue[_first] = default;
            _first              = _first.SumRound(1, _maxLength);
            OnDequeue?.Invoke(item);
            return item;
        }

        public T Peek() => _arrayQueue[_first];

        public void Clear()
        {
            for (int i = _first; i < _last; i++)
                _arrayQueue[i] = default;

            _first = 0;
            _last  = 0;
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
            var newArray = (T[]) array;
            for (int i = 0; i < Count; i++)
                newArray[index + i] = _arrayQueue[i];
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = _first; i < _last; i++)
                yield return _arrayQueue[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #region SUBSCRIBERS
        public void Subscribe(JGenericDelegate<T> action) { OnEnqueue   += action; }
        public void UnSubscribe(JGenericDelegate<T> action) { OnEnqueue -= action; }

        public void SubscribeToEnqueue(JGenericDelegate<T> action) { Subscribe(action); }
        public void UnSubscribeToEnqueue(JGenericDelegate<T> action) { UnSubscribe(action); }

        public void SubscribeToDequeue(JGenericDelegate<T> action) { OnDequeue   += action; }
        public void UnSubscribeToDequeue(JGenericDelegate<T> action) { OnDequeue -= action; }
        #endregion
    }
}
