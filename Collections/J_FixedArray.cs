using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Collections
{
    /// <summary>
    /// a fixed array to be shared between classes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class J_FixedArray<T> : J_Service
    {
        #region VALUES AND PROPERTIES
        // --------------- EVENTS --------------- //
        private event JGenericDelegate<(int index, T oldElement, T newElement)> OnChange;

        [BoxGroup("Setup", true, true, 0), SerializeField] protected T[] _thisArray;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Length { get; private set; }

        // --------------- ARRAY --------------- //
        public T this[int index] { get => _thisArray[index]; set => AddAtIndex(index, value); }
        #endregion

        private void AddAtIndex(int index, T item)
        {
            Assert.IsNotNull(_thisArray, $"{name} Array not initialized");
            Assert.IsTrue(index < Length, $"{name} length is {Length}, not valid index => {index}");

            T previousItem = _thisArray[index];
            _thisArray[index] = item;
            WhatHappensOnChange(index, previousItem, item);
            OnChange?.Invoke((index, previousItem, item));
        }

        protected virtual void WhatHappensOnChange(int index, T previousItem, T item) {}

        protected override void ActivateThis()
        {
            base.ActivateThis();
            Length = _thisArray.Length;
        }

        /// <summary>
        /// process all the non null elements with an action
        /// </summary>
        /// <param name="actionToCall">the action we want to send to non null items</param>
        public void ProcessWith(JGenericDelegate<T> actionToCall)
        {
            for (int i = 0; i < Length; i++)
                if (_thisArray[i] != null)
                    actionToCall(_thisArray[i]);
        }

        public void Clear() { ResetThis(); }

        public void CopyTo(T[] array, int arrayIndex) { _thisArray.CopyTo(array, arrayIndex); }

        private int IndexOf(T item)
        {
            for (int i = 0; i < Length; i++)
            {
                if (_thisArray[i] != null) continue;
                return i;
            }

            //we reach this point the is not found
            JConsole.Warning($"{name} item not found: {item}");
            return -1;
        }

        #region SUBSCRIBERS
        public void Subscribe(JGenericDelegate<(int index, T oldElement, T newElement)> action)
        {
            if (!IsActive) Activate();
            OnChange += action;
        }

        public void UnSubscribe(JGenericDelegate<(int index, T oldElement, T newElement)> action) { OnChange -= action; }
        #endregion
    }
}
