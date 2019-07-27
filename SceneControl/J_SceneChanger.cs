using System;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JReact.SceneControls
{
    /// <summary>
    /// this class is used to change the scene
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Scenes/Scene Changer", fileName = "SceneChanger")]
    public sealed class J_SceneChanger : ScriptableObject, jObservable<(Scene previous, Scene current)>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        private event Action<(Scene previous, Scene current)> OnSceneChange;
        private event Action<float> OnLoadProgress;

        //to make sure we save one first scene
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _isInitialized;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Scene _currentScene;
        public Scene CurrentScene { get => _currentScene; private set => _currentScene = value; }

        // --------------- INITIALIZATION --------------- //
        private void SetupThis()
        {
            _isInitialized = true;
            //store the first scene, without triggering the event
            _currentScene = SceneManager.GetActiveScene();
            JLog.Log($"{name} complete the setup", JLogTags.SceneManager, this);
        }

        // --------------- COMMANDS --------------- //
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

        // --------------- SCENE PROCESSING --------------- //
        private IEnumerator<float> LoadingTheScene(string sceneName)
        {
            // The Application loads the Scene in the background as the current Scene runs.
            // This is particularly good for creating loading screens.
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            SceneManager.activeSceneChanged += SceneChanged;

            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                //wait one frame and send the progress event
                yield return Timing.WaitForOneFrame;
                OnLoadProgress?.Invoke(asyncLoad.progress);
            }
        }

        //this is sent when the new scene is changed
        private void SceneChanged(Scene oldScene, Scene newScene)
        {
            JLog.Log($"{name} scene change from -{oldScene.name}- to -{newScene.name}-", JLogTags.SceneManager, this);

            SceneManager.activeSceneChanged -= SceneChanged;
            CurrentScene                    =  newScene;
            OnSceneChange?.Invoke((oldScene, newScene));
        }

        // --------------- SUBSCRIBERS --------------- //
        public void Subscribe(Action<(Scene previous, Scene current)>   actionToAdd)    { OnSceneChange += actionToAdd; }
        public void UnSubscribe(Action<(Scene previous, Scene current)> actionToRemove) { OnSceneChange -= actionToRemove; }

        public void SubscribeToLoadProgress(Action<float>   actionToAdd)    { OnLoadProgress += actionToAdd; }
        public void UnSubscribeToLoadProgress(Action<float> actionToRemove) { OnLoadProgress -= actionToRemove; }

#if UNITY_EDITOR
        // --------------- DEBUG --------------- //
        [BoxGroup("Debug", true, true, 0), SerializeField] private string _sceneToLoad;
        [BoxGroup("Debug", true, true, 0), Button(ButtonSizes.Medium)] private void DebugLoadScene() => LoadScene(_sceneToLoad);
#endif
    }
}
