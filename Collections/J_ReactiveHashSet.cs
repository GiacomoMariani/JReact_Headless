using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Collections
{
    public abstract class J_ReactiveHashSet<T> : ScriptableObject, jObservable<T>, IEnumerable<T>, ISet<T>
    {
        // --------------- EVENTS --------------- //
        private event Action<T> OnAdd;
        private event Action<T> OnRemove;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected HashSet<T> _ThisHashSet { get; } = new HashSet<T>();
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Length => _ThisHashSet.Count;
        public int Count => _ThisHashSet.Count;

        // --------------- MAIN COMMANDS --------------- //
        public bool Add(T item)
        {
            Assert.IsNotNull(_ThisHashSet, $"{name} hashset not initialized");
            Assert.IsTrue(item != null, $"{name} Null elements are not valid");
            if (!_ThisHashSet.Add(item)) return false;
            //a virtual method if we want to add further actions
            WhatHappensOnAdd(item);
            OnAdd?.Invoke(item);
            return true;
        }

        public bool Remove(T item)
        {
            if (!_ThisHashSet.Contains(item))
            {
                JLog.Warning($"The element {item} is not in the hashset", JLogTags.Collection, this);
                return false;
            }

            _ThisHashSet.Remove(item);
            //a virtual method if we want to add further actions
            WhatHappensOnRemove(item);
            OnRemove?.Invoke(item);
            return true;
        }

        public virtual void Clear()
        {
            foreach (T t in _ThisHashSet) OnRemove?.Invoke(t);

            _ThisHashSet.Clear();
        }

        /// <summary>
        /// process all the elements in this list with a given action
        /// </summary>
        /// <param name="actionToCall">the action used on all the items</param>
        public void ProcessWith(Action<T> actionToCall)
        {
            foreach (T t in _ThisHashSet) actionToCall(t);
        }

        // --------------- FURTHER IMPLEMENTATIONS AND HELPERS --------------- //
        //virtual methods to be applied if required
        protected virtual void WhatHappensOnRemove(T elementToRemove) {}
        protected virtual void WhatHappensOnAdd(T    elementToAdd)    {}

        public virtual bool Contains(T elementToCheck) => _ThisHashSet.Contains(elementToCheck);

        #region SUBSCRIBERS
        public void Subscribe(Action<T> actionToRegister)
        {
            OnAdd    += actionToRegister;
            OnRemove += actionToRegister;
        }

        public void UnSubscribe(Action<T> actionToRegister)
        {
            OnAdd    -= actionToRegister;
            OnRemove -= actionToRegister;
        }

        public void SubscribeToAdd(Action<T>   actionToRegister) { OnAdd += actionToRegister; }
        public void UnSubscribeToAdd(Action<T> actionToRegister) { OnAdd -= actionToRegister; }

        public void SubscribeToRemove(Action<T>   actionToRegister) { OnRemove += actionToRegister; }
        public void UnSubscribeToRemove(Action<T> actionToRegister) { OnRemove -= actionToRegister; }
        #endregion

        #region IENUMERABLE AND SET IMPLEMENTATION
        public void CopyTo(T[] array, int arrayIndex) { _ThisHashSet.CopyTo(array, arrayIndex); }

        public bool IsReadOnly => false;

        public IEnumerator<T> GetEnumerator() => _ThisHashSet.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void ExceptWith(IEnumerable<T> other) => _ThisHashSet.ExceptWith(other);

        public void IntersectWith(IEnumerable<T> other) => _ThisHashSet.IntersectWith(other);

        public bool IsProperSubsetOf(IEnumerable<T> other) => _ThisHashSet.IsProperSubsetOf(other);

        public bool IsProperSupersetOf(IEnumerable<T> other) => _ThisHashSet.IsProperSupersetOf(other);

        public bool IsSubsetOf(IEnumerable<T> other) => _ThisHashSet.IsSubsetOf(other);

        public bool IsSupersetOf(IEnumerable<T> other) => _ThisHashSet.IsSupersetOf(other);

        public bool Overlaps(IEnumerable<T> other) => _ThisHashSet.Overlaps(other);

        public bool SetEquals(IEnumerable<T> other) => _ThisHashSet.SetEquals(other);

        public void SymmetricExceptWith(IEnumerable<T> other) => _ThisHashSet.SymmetricExceptWith(other);

        public void UnionWith(IEnumerable<T> other) => _ThisHashSet.UnionWith(other);

        void ICollection<T>.Add(T item)
        {
            if (item != null) Add(item);
        }
        #endregion
    }
}
