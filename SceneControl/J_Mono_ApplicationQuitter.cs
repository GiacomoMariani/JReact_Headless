using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.SceneControls
{
    /// <summary>
    /// sends events after quit
    /// </summary>
    public sealed class J_Mono_ApplicationQuitter : MonoBehaviour
    {
        [BoxGroup("Quit Event", true, true), SerializeField, AssetsOnly, Required] private J_Event _applicationQuitting;
        private void OnApplicationQuit() { _applicationQuitting.RaiseEvent(); }
    }
}
