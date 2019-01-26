using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Selection
{
    /// <summary>
    /// selects one item
    /// </summary>
    /// <typeparam name="T">type of the selectable item</typeparam>
    public abstract class J_Selector<T> : ScriptableObject, iObservable<T>, iResettable
        where T : class
    {
        #region FIELDS AND PROPERTIES
        private event JGenericDelegate<T> OnSelect;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected T _selected;
        public T Selected
        {
            get => _selected;
            protected set
            {
                //deselects if required
                if (_selected != null) Deselect();
                //set the value
                _selected = value;
                if (value != null) ActOnSelection(value);
                //send event
                OnSelect?.Invoke(value);
            }
        }
        #endregion

        /// <summary>
        /// selects an item
        /// </summary>
        /// <param name="item">the item selected</param>
        public void SelectThis(T item) { Selected = item; }

        /// any logic to be applied on the selected item
        protected virtual void ActOnSelection(T item) { }

        /// <summary>
        /// deselects the selected item
        /// </summary>
        protected virtual void Deselect() { _selected = null; }

        #region DISABLE AND RESET
        private void OnDisable() { ResetThis(); }
        public virtual void ResetThis() { Deselect(); }
        #endregion

        #region SUBSCRIBERS
        public void Subscribe(JGenericDelegate<T> actionToAdd) { OnSelect += actionToAdd; }
        public void UnSubscribe(JGenericDelegate<T> actionToRemove) { OnSelect -= actionToRemove; }
        #endregion
    }
}
