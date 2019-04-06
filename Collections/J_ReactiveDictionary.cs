using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Collections
{
    /// <summary>
    /// a reactive dictionary, quite experimental
    /// </summary>
    /// <typeparam name="TKey">the key type</typeparam>
    /// <typeparam name="TValue">the value type</typeparam>
    public abstract class J_ReactiveDictionary<TKey, TValue> : ScriptableObject,
                                                               iObservable<(TKey key, TValue value)>,
                                                               IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private event JGenericDelegate<(TKey key, TValue value)> OnChange;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected Dictionary<TKey, TValue> _Dictionary { get; } =
            new Dictionary<TKey, TValue>();
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Count => _Dictionary.Count;

        public TValue this[TKey key] { get => _Dictionary[key]; set => Set(key, value); }

        /// <summary>
        /// used to add an item to the dictionary, or change it
        /// </summary>
        public void Set(TKey key, TValue value)
        {
            Assert.IsNotNull(_Dictionary, $"{name} not initiated");
            Internal_UpdateElement(key, value);
            OnChange?.Invoke((key, value));
        }

        /// <summary>
        /// the specific action to be sent when the dictionary is changed
        /// </summary>
        protected virtual void Internal_UpdateElement(TKey key, TValue value) { _Dictionary[key] = value; }

        public void Subscribe(JGenericDelegate<(TKey key, TValue value)> action) { OnChange   += action; }
        public void UnSubscribe(JGenericDelegate<(TKey key, TValue value)> action) { OnChange -= action; }

        public virtual void Clear() => _Dictionary.Clear();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _Dictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
