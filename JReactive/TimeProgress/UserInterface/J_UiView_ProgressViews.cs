using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.TimeProgress
{
    /// <summary>
    /// show or hide a specific element
    /// </summary>
    [RequireComponent(typeof(J_Mono_ViewActivator))]
    public class J_UiView_ProgressViews : J_Mono_ProgressView
    {
        #region FIELDS AND PROPERTIES
        //the views related to this progress
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private J_Mono_ViewActivator _view;
        private J_Mono_ViewActivator ThisView
        { get { if (_view == null) _view = GetComponent<J_Mono_ViewActivator>(); return _view; } }
        #endregion

        #region ABSTRACT IMPLEMENTATION
        protected override void ProgressComplete(J_ProgressEvent progress) { ThisView.ActivateView(false); }

        protected override void ProgressStart(J_ProgressEvent progress) { ThisView.ActivateView(true); }

        protected override void ProgressUpdate(J_ProgressEvent progress) { }

        protected override void ViewEnabled(J_ProgressEvent progress)
        {
            base.ViewEnabled(progress);
            ThisView.ActivateView(_progressEvent != null && _progressEvent.IsRunning);
        }
        #endregion
    }
}
