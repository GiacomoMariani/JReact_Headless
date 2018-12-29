using MEC;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JReact.SceneControls
{
    /// <summary>
    /// this class is used to change the scene
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Scenes/Scene Changer")]
    public class J_SceneChanger : ScriptableObject
    {
        #region FIELDS AND PROPERTIES
        private event JSceneChange OnSceneChange;
        private event JFloatDelegate OnLoadProgress;

        //to make sure we save one first scene
        [BoxGroup("State", true, true, 5), ReadOnly, ShowInInspector] private bool _isInitialized = false;
        [BoxGroup("State", true, true, 5), ReadOnly, ShowInInspector] private Scene _currentScene;
        public Scene CurrentScene { get => _currentScene; private set => _currentScene = value; }
        #endregion

        #region INITIALIZATION
        private void SetupThis()
        {
            _isInitialized = true;
            //store the first scene, without triggering the event
            _currentScene = SceneManager.GetActiveScene();
            HelperConsole.DisplayMessage($"{name} complete the setup", J_DebugConstants.Debug_SceneManager);
        }
        #endregion

        #region COMMANDS
        /// <summary>
        /// loads a specific scene from its name
        /// </summary>
        /// <param name="sceneName">the name of the scene to load</param>
        public void LoadScene(string sceneName)
        {
            if (!_isInitialized) SetupThis();
            Timing.RunCoroutine(LoadingTheScene(sceneName), Segment.Update, JCoroutineTags.COROUTINE_SceneChangerTag);
        }

        /// <summary>
        /// used to cancel the load of a given scene
        /// </summary>
        public void CancelLoadScene() { Timing.KillCoroutines(JCoroutineTags.COROUTINE_SceneChangerTag); }
        #endregion

        #region SCENE PROCESSING
        private IEnumerator<float> LoadingTheScene(string sceneName)
        {
            // The Application loads the Scene in the background as the current Scene runs.
            // This is particularly good for creating loading screens.
            var asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            SceneManager.activeSceneChanged += SceneChanged;

            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                //wait one frame and send the progress event
                yield return Timing.WaitForOneFrame;
                if (OnLoadProgress != null) OnLoadProgress(asyncLoad.progress);
            }
        }

        //this is sent when the new scene is changed
        private void SceneChanged(Scene oldScene, Scene newScene)
        {
            HelperConsole.DisplayMessage($"{name} changed from scene -{oldScene.name}- to scene -{newScene.name}-",
                                         J_DebugConstants.Debug_SceneManager);
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= SceneChanged;
            CurrentScene                                                =  newScene;
            if (OnSceneChange != null) OnSceneChange(oldScene, newScene);
        }
        #endregion

        #region SUBSCRIBERS
        public void Subscribe(JSceneChange actionToAdd) { OnSceneChange      += actionToAdd; }
        public void UnSubscribe(JSceneChange actionToRemove) { OnSceneChange -= actionToRemove; }

        public void SubscribeToLoadProgress(JFloatDelegate actionToAdd) { OnLoadProgress      += actionToAdd; }
        public void UnSubscribeToLoadProgress(JFloatDelegate actionToRemove) { OnLoadProgress -= actionToRemove; }
        #endregion
    }
}
