using Sirenix.OdinInspector;
using UnityEngine.Assertions;

namespace JReact.Selection
{
    /// <summary>
    /// an actor related to a selector
    /// </summary>
    /// <typeparam name="T">a selectable</typeparam>
    public abstract class J_Mono_SelectActor<T> : J_Mono_Actor<T>
        where T : class
    {
        #region FIELDS AND PROPERTIES
        [BoxGroup("Setup", true, true, 0), ReadOnly, ShowInInspector] protected abstract J_Selector<T> _ThisSelector { get; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected T _currentSelection;
        #endregion

        #region INITIALIZATION
        protected override void InitThis()
        {
            base.InitThis();
            TrackSelection();
        }

        protected override void SanityChecks()
        {
            base.SanityChecks();
            Assert.IsNotNull(_ThisSelector, $"{gameObject.name} requires a _ThisSelector");
        }

        private void TrackSelection()
        {
            if (_ThisSelector.Selected != null) UpdateElement(_ThisSelector.Selected);
            _ThisSelector.Subscribe(SelectionUpdate);
        }
        #endregion

        #region VIEW UPDATES
        //update of the selected element
        protected virtual void SelectionUpdate(T selectedElement)
        {
            _currentSelection = selectedElement;
            UpdateElement(selectedElement);
        }
        #endregion

        protected virtual void ResetThis() { _ThisSelector.UnSubscribe(SelectionUpdate); }
        protected virtual void OnDestroy() { ResetThis(); }
    }
}
