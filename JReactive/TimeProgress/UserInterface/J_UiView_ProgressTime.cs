using TMPro;
using UnityEngine;

namespace JReact.TimeProgress
{
    /// <summary>
    /// converts progress into seconds
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class J_UiView_ProgressTime : J_Mono_ProgressView
    {
        #region FIELDS AND PROPERTIES
        private TextMeshProUGUI _thisText;
        private TextMeshProUGUI ThisText
        {
            get
            {
                if (_thisText == null) _thisText = GetComponentInChildren<TextMeshProUGUI>(true);
                return _thisText;
            }
        }
        #endregion

        #region ABSTRACT IMPLEMENTATION

        protected override void InitThis()
        {
            base.InitThis();
            ThisText.text = _progressEvent == null
                                ? null
                                : _progressEvent.RemainingSeconds.ToTimeString();
        }

        //sets the time if the progress is running
        protected override void ProgressStart(J_ProgressEvent progress) { ThisText.text = progress.RemainingSeconds.ToTimeString(); }

        protected override void ProgressUpdate(J_ProgressEvent progress) { ThisText.text = progress.RemainingSeconds.ToTimeString(); }

        protected override void ProgressComplete(J_ProgressEvent progress) { ThisText.text = null; }
        #endregion
    }
}
