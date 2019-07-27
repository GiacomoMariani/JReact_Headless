using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.SceneControls
{
    [CreateAssetMenu(menuName = "Reactive/Scenes/Scene Change Action")]
    public class J_SceneChangeAction : J_ProcessableAction, iProcessable
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("State Control", true, true, 0), SerializeField, AssetsOnly, Required]
        private J_SceneChanger _sceneChanger;
        [BoxGroup("State Control", true, true, 0), SerializeField, AssetsOnly, Required]
        private string _desiredScene;

        /// <summary>
        /// sets the desired scene
        /// </summary>
        public override void Process() => _sceneChanger.LoadScene(_desiredScene);
    }
}
