using System;
using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Collections
{
    /// <summary>
    /// a reactive collection that sends events at add and remove
    /// </summary>
    /// <typeparam name="T">the type of this collection</typeparam>
    public abstract class J_ReactiveCollection<T> : ScriptableObject, iResettable, IList<T>
    {
        #region EVENT AND DELEGATES
        private event JGenericDelegate<T> OnAddToCollection;
        private event JGenericDelegate<T> OnRemoveToCollection;
        #endregion

        #region VALUES AND PROPERTIES
        //the list related to this collection
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected List<T> _thisCollection = new List<T>();
        //the amount of elements in the collection
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Count => _thisCollection.Count;
        #endregion

        #region MAIN COMMANDS
        /// this method is used to add an item to the collection
        public void Add(T item)
        {
            //make sure the list has been initiated
            Assert.IsNotNull(_thisCollection, $"{name} Collection not initialized");
            Assert.IsTrue(item != null, $"{name} Null elements are not valid");
            //add the element
            _thisCollection.Add(item);
            //a method to be used on the element changed
            WhatHappensOnAdd(item);
            //send the event
            OnAddToCollection?.Invoke(item);
        }

        /// this method is used to add an item to the collection
        public bool Remove(T item)
        {
            //make sure the element is in the list
            if (!_thisCollection.Contains(item))
            {
                JConsole.Warning($"The element {item} is not in the list", JLogTags.Collection, this);
                return false;
            }

            //add the element
            _thisCollection.Remove(item);
            //a method to be used on the element changed
            ElementRemoved(item);
            //send the event
            OnRemoveToCollection?.Invoke(item);
            return true;
        }

        /// <summary>
        /// this is used to reset the list, it is not automatically called
        /// </summary>
        public virtual void ResetThis()
        {
            //send the remove events for all the items
            for (int i = 0; i < Count; i++)
                OnRemoveToCollection?.Invoke(_thisCollection[i]);

            //just to double check everything is cleared
            _thisCollection.Clear();
        }

        /// <summary>
        /// this process all the elements in this list with a given acation
        /// </summary>
        /// <param name="actionToCall">the action we want to send to all the elements of this list</param>
        public void ProcessWith(JGenericDelegate<T> actionToCall)
        {
            for (int i = 0; i < Count; i++)
                actionToCall(_thisCollection[i]);
        }
        #endregion

        #region VIRTUAL FURTHER IMPLEMENTATION
        //an helper method to be applied if required
        protected virtual void ElementRemoved(T elementToRemove) { }

        //an helper method to be applied if required
        protected virtual void WhatHappensOnAdd(T elementToAdd) { }
        #endregion

        #region GETTERS
        //used to check if this contains a given item
        public virtual bool Contains(T elementToCheck) { return _thisCollection.Contains(elementToCheck); }
        #endregion

        #region SUBSCRIBERS
        public void Subscribe(JGenericDelegate<T> actionToRegister)
        {
            OnAddToCollection    += actionToRegister;
            OnRemoveToCollection += actionToRegister;
        }

        public void UnSubscribe(JGenericDelegate<T> actionToRegister)
        {
            OnAddToCollection    -= actionToRegister;
            OnRemoveToCollection -= actionToRegister;
        }

        public void SubscribeToCollectionAdd(JGenericDelegate<T> actionToRegister) { OnAddToCollection   += actionToRegister; }
        public void UnSubscribeToCollectionAdd(JGenericDelegate<T> actionToRegister) { OnAddToCollection -= actionToRegister; }

        public void SubscribeToCollectionRemove(JGenericDelegate<T> actionToRegister) { OnRemoveToCollection   += actionToRegister; }
        public void UnSubscribeToCollectionRemove(JGenericDelegate<T> actionToRegister) { OnRemoveToCollection -= actionToRegister; }
        #endregion

        #region DISABLE AND RESET
        protected virtual void OnDisable() { ResetThis(); }
        #endregion

        #region IENUMERABLE AND LIST IMPLEMENTATION
        public void Clear() { ResetThis(); }

        public void CopyTo(T[] array, int arrayIndex) { _thisCollection.CopyTo(array, arrayIndex); }

        public bool IsReadOnly => false;

        public IEnumerator<T> GetEnumerator() { return _thisCollection.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public int IndexOf(T item) { return _thisCollection.IndexOf(item); }

        public void Insert(int index, T item)
        {
            for (int i = index; i < Count; i++)
            {
                var next = _thisCollection[i];
                Replace(i, item);
                item = next;
            }
        }

        public void RemoveAt(int index)
        {
            Assert.IsTrue(index < Count, $"{name} has not the given index {index}. List length = {Count}");
            var item = _thisCollection[index];
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
            OnAddToCollection?.Invoke(item);
        }
        #endregion
    }
}
