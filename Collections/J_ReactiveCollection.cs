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
    public abstract class J_ReactiveCollection<T> : ScriptableObject, iResettable, IList<T>, iObservable<T>
    {
        #region EVENT AND DELEGATES
        private event JGenericDelegate<T> OnAdd;
        private event JGenericDelegate<T> OnRemove;
        #endregion

        #region VALUES AND PROPERTIES
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected List<T> _thisCollection = new List<T>();
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Count => _thisCollection.Count;
        #endregion

        #region MAIN COMMANDS
        // add an item to the collection
        public void Add(T item)
        {
            Assert.IsNotNull(_thisCollection, $"{name} Collection not initialized");
            Assert.IsTrue(item != null, $"{name} Null elements are not valid");
            _thisCollection.Add(item);
            //a method to be used on the element changed
            WhatHappensOnAdd(item);
            OnAdd?.Invoke(item);
        }

        /// remove an item to the collection
        public bool Remove(T item)
        {
            if (!_thisCollection.Contains(item))
            {
                JConsole.Warning($"The element {item} is not in the list", JLogTags.Collection, this);
                return false;
            }

            _thisCollection.Remove(item);
            //a method to be used on the element changed
            ElementRemoved(item);
            OnRemove?.Invoke(item);
            return true;
        }

        // reset the list
        public virtual void ResetThis()
        {
            //send the remove events for all the items
            for (int i = 0; i < Count; i++)
                OnRemove?.Invoke(_thisCollection[i]);

            //just to double check everything is cleared
            _thisCollection.Clear();
        }

        /// <summary>
        /// process all the elements in this list with a given action
        /// </summary>
        /// <param name="actionToCall">the action we want to send to all the elements of this list</param>
        public void ProcessWith(JGenericDelegate<T> actionToCall)
        {
            for (int i = 0; i < Count; i++)
                actionToCall(_thisCollection[i]);
        }
        #endregion

        #region VIRTUAL FURTHER IMPLEMENTATION
        //helper methods to be applied if required
        protected virtual void ElementRemoved(T elementToRemove) {}
        protected virtual void WhatHappensOnAdd(T elementToAdd) {}
        #endregion

        #region GETTERS
        //checks if this contains a given item
        public virtual bool Contains(T elementToCheck) => _thisCollection.Contains(elementToCheck);
        #endregion

        #region SUBSCRIBERS
        public void Subscribe(JGenericDelegate<T> actionToRegister)
        {
            OnAdd    += actionToRegister;
            OnRemove += actionToRegister;
        }

        public void UnSubscribe(JGenericDelegate<T> actionToRegister)
        {
            OnAdd    -= actionToRegister;
            OnRemove -= actionToRegister;
        }

        public void SubscribeToAdd(JGenericDelegate<T> actionToRegister) { OnAdd   += actionToRegister; }
        public void UnSubscribeToAdd(JGenericDelegate<T> actionToRegister) { OnAdd -= actionToRegister; }

        public void SubscribeToRemove(JGenericDelegate<T> actionToRegister) { OnRemove   += actionToRegister; }
        public void UnSubscribeToRemove(JGenericDelegate<T> actionToRegister) { OnRemove -= actionToRegister; }
        #endregion

        #region DISABLE AND RESET
        protected virtual void OnDisable() { ResetThis(); }
        #endregion

        #region IENUMERABLE AND LIST IMPLEMENTATION
        public void Clear() { ResetThis(); }

        public void CopyTo(T[] array, int arrayIndex) { _thisCollection.CopyTo(array, arrayIndex); }

        public bool IsReadOnly => false;

        public IEnumerator<T> GetEnumerator() => _thisCollection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int IndexOf(T item) => _thisCollection.IndexOf(item);

        public void Insert(int index, T item)
        {
            for (int i = index; i < Count; i++)
            {
                T next = _thisCollection[i];
                Replace(i, item);
                item = next;
            }
        }

        public void RemoveAt(int index)
        {
            Assert.IsTrue(index < Count, $"{name} has not the given index {index}. List length = {Count}");
            T item = _thisCollection[index];
            Remove(item);
        }

        public T this[int index] { get => _thisCollection[index]; set => Replace(index, value); }

        private void Replace(int index, T item)
        {
            Assert.IsNotNull(_thisCollection, $"{name} Collection not initialized");
            Assert.IsTrue(item != null, $"{name} Null elements are not valid");
            RemoveAt(index);
            _thisCollection.Insert(index, item);
            WhatHappensOnAdd(item);
            OnAdd?.Invoke(item);
        }
        #endregion
    }
}
