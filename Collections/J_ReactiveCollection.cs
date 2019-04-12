using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Collections
{
    /// <summary>
    /// a reactive collection that sends events at add and remove
    /// </summary>
    /// <typeparam name="T">the type of this collection</typeparam>
    public abstract class J_ReactiveCollection<T> : ScriptableObject, IList<T>, iObservable<T>
    {
        #region VALUES AND PROPERTIES
        // --------------- EVENTS --------------- //
        private event Action<T> OnAdd;
        private event Action<T> OnRemove;
        
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected List<T> _ThisCollection { get; } = new List<T>();
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Count => _ThisCollection.Count;
        #endregion

        #region MAIN COMMANDS
        public void Add(T item)
        {
            Assert.IsNotNull(_ThisCollection, $"{name} Collection not initialized");
            Assert.IsTrue(item != null, $"{name} Null elements are not valid");
            _ThisCollection.Add(item);
            //a virtual method if we want to add further actions
            WhatHappensOnAdd(item);
            OnAdd?.Invoke(item);
        }

        public bool Remove(T item)
        {
            if (!_ThisCollection.Contains(item))
            {
                JLog.Warning($"The element {item} is not in the list", JLogTags.Collection, this);
                return false;
            }

            _ThisCollection.Remove(item);
            //a virtual method if we want to add further actions
            WhatHappensOnRemove(item);
            OnRemove?.Invoke(item);
            return true;
        }

        public virtual void Clear()
        {
            //send the remove events for all the items
            for (int i = 0; i < Count; i++)
                OnRemove?.Invoke(_ThisCollection[i]);
            _ThisCollection.Clear();
        }

        /// <summary>
        /// process all the elements in this list with a given action
        /// </summary>
        /// <param name="actionToCall">the action used on all the items</param>
        public void ProcessWith(JGenericDelegate<T> actionToCall)
        {
            for (int i = 0; i < Count; i++)
                actionToCall(_ThisCollection[i]);
        }
        #endregion

        #region VIRTUAL FURTHER IMPLEMENTATION
        //virtual methods to be applied if required
        protected virtual void WhatHappensOnRemove(T elementToRemove) {}
        protected virtual void WhatHappensOnAdd(T elementToAdd) {}
        #endregion

        #region GETTERS
        public virtual bool Contains(T elementToCheck) => _ThisCollection.Contains(elementToCheck);
        #endregion

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

        public void SubscribeToAdd(Action<T> actionToRegister) { OnAdd   += actionToRegister; }
        public void UnSubscribeToAdd(Action<T> actionToRegister) { OnAdd -= actionToRegister; }

        public void SubscribeToRemove(Action<T> actionToRegister) { OnRemove   += actionToRegister; }
        public void UnSubscribeToRemove(Action<T> actionToRegister) { OnRemove -= actionToRegister; }
        #endregion

        #region IENUMERABLE AND LIST IMPLEMENTATION
        public void CopyTo(T[] array, int arrayIndex) { _ThisCollection.CopyTo(array, arrayIndex); }

        public bool IsReadOnly => false;

        public IEnumerator<T> GetEnumerator() => _ThisCollection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int IndexOf(T item) => _ThisCollection.IndexOf(item);

        public void Insert(int index, T item)
        {
            for (int i = index; i < Count; i++)
            {
                T next = _ThisCollection[i];
                Replace(i, item);
                item = next;
            }
        }

        public void RemoveAt(int index)
        {
            Assert.IsTrue(index < Count, $"{name} has not the given index {index}. List length = {Count}");
            T item = _ThisCollection[index];
            Remove(item);
        }

        public T this[int index] { get => _ThisCollection[index]; set => Replace(index, value); }

        private void Replace(int index, T item)
        {
            Assert.IsNotNull(_ThisCollection, $"{name} Collection not initialized");
            Assert.IsTrue(item != null, $"{name} Null elements are not valid");
            RemoveAt(index);
            _ThisCollection.Insert(index, item);
            WhatHappensOnAdd(item);
            OnAdd?.Invoke(item);
        }
        #endregion
    }
}
