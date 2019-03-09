using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Collections
{
    /// <summary>
    /// an array that acts as a reactive collection with a constrains (length and null values), with add and remove events 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class J_ReactiveArray<T> : J_Service
        where T : class
    {
        #region EVENT AND DELEGATES
        private event JGenericDelegate<T> OnAdd;
        private event JGenericDelegate<T> OnRemove;
        #endregion

        #region VALUES AND PROPERTIES
        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private int _length = 50;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected T[] _thisArray;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Length => _thisArray?.Length ?? 0;

        // --------------- ARRAY --------------- //
        public T this[int index] { get => _thisArray[index]; set => AddAtIndex(index, value); }
        #endregion

        #region MAIN COMMANDS
        protected override void ActivateThis()
        {
            base.ActivateThis();
            Assert.IsTrue(_length > 0, $"{name} requires a positive number. Given {_length}");
            _thisArray = new T[_length];
        }

        /// <summary>
        /// add the element and returns the index
        /// </summary>
        /// <param name="item">the item to add</param>
        /// <returns>the index where this is added</returns>
        public int Add(T item)
        {
            
            Assert.IsTrue(item != null, $"{name} Null elements are not valid");

            //find the first empty place
            int index = IndexOf(null);
            // --------------- ADD CONFIRM --------------- //
            if (index != -1)
            {
                AddAtIndex(index, item);
                return index;
            }

            // --------------- ADD ERROR --------------- //
            JConsole.Warning($"{name} array is full. Item {item}");
            return -1;
        }

        /// <summary>
        /// remove an element from the array, returning its index
        /// </summary>
        /// <param name="item">the item we want to remove</param>
        /// <returns>returns the index of the item, or -1 if nothing is found</returns>
        public int Remove(T item)
        {
            Assert.IsTrue(IsActive, $"{name} has not been initialized");
            Assert.IsTrue(item != null, $"{name} cannot remove null elements");
            //find the item
            int index = IndexOf(item);
            // --------------- REMOVE CONFIRM --------------- //
            if (index != -1)
            {
                RemoveAt(index);
                return index;
            }
            // --------------- REMOVE ERROR --------------- //
            JConsole.Warning($"{name} there is not such item in the array. Item {item}");
            return -1;
        }

        /// <summary>
        /// reset the array
        /// </summary>
        public override void ResetThis()
        {
            base.ResetThis();
            for (int i = 0; i < _length; i++) RemoveAt(i);
        }

        /// <summary>
        /// process all the non null elements with an action
        /// </summary>
        /// <param name="actionToCall">the action we want to send to all the non null items</param>
        public void ProcessWith(JGenericDelegate<T> actionToCall)
        {
            for (int i = 0; i < Length; i++)
                if (_thisArray[i] != null)
                    actionToCall(_thisArray[i]);
        }
        #endregion

        #region VIRTUAL FURTHER IMPLEMENTATION
        //helper methods to be applied if required
        protected virtual void ElementRemoved(T elementToRemove) {}
        protected virtual void WhatHappensOnAdd(T elementToAdd) {}
        #endregion

        #region HELPERS
        public void Clear() { ResetThis(); }

        public void CopyTo(T[] array, int arrayIndex) { _thisArray.CopyTo(array, arrayIndex); }

        private int IndexOf(T item)
        {
            for (int i = 0; i < _length; i++)
            {
                if (_thisArray[i] != null) continue;
                return i;
            }

            //we reach this point the is not found
            JConsole.Warning($"{name} item not found: {item}");
            return -1;
        }

        private void RemoveAt(int index)
        {
            Assert.IsTrue(IsActive, $"{name} has not been initialized");
            Assert.IsNotNull(_thisArray, $"{name} Array not initialized");
            Assert.IsTrue(index < Length, $"{name} has not the given index {index}. List length = {Length}");
            T item = _thisArray[index];
            if (item == null)
            {
                JConsole.Warning($"{name} wants to remove element at {index}, but it is null. Cancel command");
                return;
            }
            _thisArray[index] = null;
            ElementRemoved(item);
            OnRemove?.Invoke(item);
        }

        private void AddAtIndex(int index, T item)
        {
            Assert.IsTrue(IsActive, $"{name} has not been initialized");
            Assert.IsNotNull(_thisArray, $"{name} Array not initialized");
            Assert.IsTrue(item  != null,   $"{name} Null elements are not valid");
            Assert.IsTrue(index < Length, $"{name} length is {Length}, not valid index => {index}");

            if (_thisArray[index] != null) RemoveAt(index);
            _thisArray[index] = item;
            WhatHappensOnAdd(item);
            OnAdd?.Invoke(item);
        }
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
    }
}
