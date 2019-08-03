using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.TimeProgress.Pause
{
    /// <summary>
    /// sends the application in pause when out of focus
    /// </summary>
    public class J_Mono_OutOfFocusPause : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_PauseEvent _pauseEvent;

        // --------------- FOCUS --------------- //
        //catch the out of focus event
        private void OnApplicationFocus(bool focus)
        {
            if (_pauseEvent.IsPaused != focus) return;
            if (!focus) _pauseEvent.StartPause();
            else _pauseEvent.EndPause();
        }
    }
}
