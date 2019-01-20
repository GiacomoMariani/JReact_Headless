using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace JReact.TimeProgress
{
    /// <summary>
    /// a button to fast finish the progress
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class J_UiView_FastFinishButton : J_Mono_ProgressView
    {
        #region FIELDS AND PROPERTIES
        private Button _thisButton;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Button ThisButton
        {
            get
            {
                if (_thisButton == null) _thisButton = GetComponent<Button>();
                return _thisButton;
            }
        }
        #endregion

        #region BUTTON COMMAND
        //to add further conditions when requested
        protected virtual bool CanBePressed() { return true; }

        //used to press the button
        private void TryPressButton()
        {
            if (CanBePressed()) _progressEvent.FastFinish();
        }
        #endregion

        #region ABSTRACT IMPLEMENTATION
        //activate when the progress is running
        protected override void ProgressStart(J_ProgressEvent progress) { ThisButton.interactable = true; }

        protected override void ProgressUpdate(J_ProgressEvent progress) { }

        //deactivate when the progress is not running
        protected override void ProgressComplete(J_ProgressEvent progress) { ThisButton.interactable = false; }
        #endregion

        #region ENABLE / DISABLE
        protected override void ViewEnabled(J_ProgressEvent progress)
        {
            base.ViewEnabled(progress);
            ThisButton.interactable = _progressEvent.IsRunning;
            ThisButton.onClick.AddListener(TryPressButton);
        }

        protected override void ViewDisabled(J_ProgressEvent progress) { ThisButton.onClick.RemoveListener(TryPressButton); }
        #endregion
    }
}
