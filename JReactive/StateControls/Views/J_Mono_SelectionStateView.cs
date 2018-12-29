using UnityEngine;

namespace JReact.StateControls
{
    /// <summary>
    /// a view related to a selected element
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class J_Mono_SelectionStateView<T> : MonoBehaviour where T : class
    {
        protected J_SelectionState<T> SelectionState => GetSelectionState();
        protected abstract J_SelectionState<T> GetSelectionState();

        private void UpdateThis() { SelectionUpdate(SelectionState.SelectedItem); }

        /// <summary>
        /// what happens when the selection changes
        /// </summary>
        /// <param name="selectedItem"></param>
        protected abstract void SelectionUpdate(iSelectable<T> selectedItem);
        
        /// <summary>
        /// what happens when player exit the selection
        /// </summary>
        protected abstract void ExitState();

        #region LISTENERS
        //start and stop tracking on enable
        private void OnEnable()
        {
            if (SelectionState.SelectedItem != null) SelectionUpdate(SelectionState.SelectedItem);
            SelectionState.Subscribe(UpdateThis);
            SelectionState.SubscribeToExitEvent(ExitState);
        }

        private void OnDisable()
        {
            SelectionState.UnSubscribe(UpdateThis);
            SelectionState.UnSubscribeToExitEvent(ExitState);
        }
        #endregion
    }
}