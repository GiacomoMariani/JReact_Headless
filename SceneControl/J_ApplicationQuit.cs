using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.SceneControls
{
    [CreateAssetMenu(menuName = "Reactive/Scenes/Quit")]
    public sealed class J_ApplicationQuit : ScriptableObject , jObservable
    {
        private event Action OnQuit;
        
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _exitCode = 0;

        [ButtonGroup("Commands", 200), Button("Activate", ButtonSizes.Medium)]
        public void Quit()
        {
            //send the event before the quit
            OnQuit?.Invoke();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        public void Subscribe(Action   actionToSubscribe) { OnQuit += actionToSubscribe; }
        public void UnSubscribe(Action actionToSubscribe) { OnQuit -= actionToSubscribe; }
    }
}
