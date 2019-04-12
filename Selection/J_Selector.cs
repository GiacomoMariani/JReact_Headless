using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Selection
{
    /// <summary>
    /// selects one item
    /// </summary>
    /// <typeparam name="T">type of the selectable item</typeparam>
    public abstract class J_Selector<T> : ScriptableObject, iObservable<T>, iResettable
    {
        #region FIELDS AND PROPERTIES
        private event Action<T> OnSelect;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private T _selected;
        public T Selected
        {
            get => _selected;
            private set
            {
                //deselects if required
                if (_selected != null) ActOnDeselection(_selected);
                //set the value
                _selected = value;
                if (_selected != null) ActOnSelection(_selected);
                //send event
                OnSelect?.Invoke(value);
            }
        }
        #endregion

        /// <summary>
        /// selects an item
        /// </summary>
        /// <param name="item">the item selected</param>
        public void Select(T item) { Selected = item; }

        /// any logic to be applied on the selected item
        protected virtual void ActOnSelection(T item) {}

        /// <summary>
        /// deselects the selected item
        /// </summary>
        public void Deselect() { Selected = default; }

        //any logic to apply on the deselected item
        protected virtual void ActOnDeselection(T item) {}

        #region DISABLE AND RESET
        private void OnDisable() { ResetThis(); }
        public virtual void ResetThis() { Deselect(); }
        #endregion

        #region SUBSCRIBERS
        public void Subscribe(Action<T> actionToAdd) { OnSelect      += actionToAdd; }
        public void UnSubscribe(Action<T> actionToRemove) { OnSelect -= actionToRemove; }
        #endregion
    }
}
