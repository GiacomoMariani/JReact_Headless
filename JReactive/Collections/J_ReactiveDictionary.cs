using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Collections
{
    /// <summary>
    /// a reactive dictionary, quite experimental
    /// </summary>
    /// <typeparam name="TKey">the key type</typeparam>
    /// <typeparam name="TValue">the value type</typeparam>
    public abstract class J_ReactiveDictionary<TKey, TValue> : ScriptableObject, iResettable
    {
        #region EVENTS
        private event JGenericDelegate<TKey> OnDictionaryChange;
        #endregion

        #region VALUES AND PROPERTIES
        //the dictionary related to this collection
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        protected Dictionary<TKey, TValue> _thisDictionary = new Dictionary<TKey, TValue>();
        private int _count;
        private int _count1;
        public Dictionary<TKey, TValue> ThisDictionary => _thisDictionary;
        //main accessor
        public TValue this[TKey index] => _thisDictionary[index];
        #endregion

        #region MAIN COMMANDS
        /// <summary>
        /// used to add an item to the dictionary, or change it
        /// </summary>
        public void UpdateElement(TKey key, TValue value)
        {
            Assert.IsNotNull(_thisDictionary, $"{name} not initiated");
            Internal_UpdateElement(key, value);
            OnDictionaryChange?.Invoke(key);
        }

        /// <summary>
        /// the specific action to be sent when the dictionary is changed
        /// </summary>
        protected virtual void Internal_UpdateElement(TKey keyToChange, TValue valueToChange) { ThisDictionary[keyToChange] = valueToChange; }
        #endregion

        #region SUBSCRIBERS
        public void Subscribe(JGenericDelegate<TKey> actionToSend) { OnDictionaryChange += actionToSend; }
        public void UnSubscribe(JGenericDelegate<TKey> actionToSend) { OnDictionaryChange -= actionToSend; }
        #endregion

        #region DISABLE AND RESET
        //we reset this on disable
        protected virtual void OnDisable() { ResetThis(); }
        public virtual void ResetThis() { ThisDictionary.Clear(); }
        #endregion       
    }
}
