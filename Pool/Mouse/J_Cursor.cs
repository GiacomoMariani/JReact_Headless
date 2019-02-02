using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Pool.SpecialEffect
{
    /// <summary>
    /// this class is used to change the mouse pointer
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Special Effects/Cursor")]
    public class J_Cursor : ScriptableObject, iResettable
    {
        #region FIELDS AND VALUES
        private event JGenericDelegate<GameObject> OnPointerChange;

        // --------------- SETUP --------------- //
        [BoxGroup("Setup - Cursor", true, true, 0), SerializeField, AssetsOnly, Required]
        private J_TransformGenerator _transformParent;
        [BoxGroup("Setup - Cursor", true, true, 0), SerializeField, AssetsOnly, Required]
        private GameObject _defaultCursor;
        [BoxGroup("Setup - Cursor", true, true, 0), SerializeField] private GameObject[] _customCursors;

        [BoxGroup("Setup - Effect", true, true, 5), SerializeField, AssetsOnly]
        private J_Pool_SpecialEffects _effect;
        [BoxGroup("Setup - Effect", true, true, 5), SerializeField, Range(0.5f, 15f)]
        private float _interval = 1f;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 15), ReadOnly, ShowInInspector] private Camera _mainCamera;
        [FoldoutGroup("State", false, 15), ReadOnly, ShowInInspector] private bool _activeCursor = false;
        [FoldoutGroup("State", false, 15), ReadOnly, ShowInInspector] private GameObject _currentCursor;
        public GameObject CurrentCursor
        {
            get => _currentCursor;
            private set
            {
                CurrentCursor.SetActive(false);
                _currentCursor = value;
                CurrentCursor.SetActive(true);
            }
        }

        [FoldoutGroup("State - Cursor", false, 20), ReadOnly, ShowInInspector] private GameObject _cursorView;
        [FoldoutGroup("State - Cursor", false, 20), ReadOnly, ShowInInspector] private GameObject[] _customCursorViews;

        [FoldoutGroup("State - Effects", false, 25), ReadOnly, ShowInInspector] private bool _activeEffect = false;
        #endregion

        #region STARTUP
        /// <summary>
        /// sets the cursor with a prefab
        /// </summary>
        /// <param name="mainCamera">we the main camera</param>
        public void StartCursor(Camera mainCamera)
        {
            EnableCursor(mainCamera);
            if (_effect != null)
                EnableEffect(mainCamera);
        }
        #endregion

        #region MOUSE CURSOR
        private void EnableCursor(Camera mainCamera)
        {
            _activeCursor = true;
            SetupCursorViews();
            SetDefaultCursor();
            Cursor.visible = false;
            Timing.RunCoroutine(FollowMouse(mainCamera), Segment.LateUpdate, JCoroutineTags.COROUTINE_MouseFollow);
        }

        private void SetupCursorViews()
        {
            Assert.IsNotNull(_defaultCursor, $"{name}- requires a default cursor");

            // --------------- DEFAULT CURSOR --------------- //
            _currentCursor = _cursorView = Instantiate(_defaultCursor, _transformParent.ThisTransform);

            // --------------- CUSTOM CURSOR VIEWS  --------------- //    
            _customCursorViews = new GameObject[_customCursors.Length];
            for (int i = 0; i < _customCursors.Length; i++)
            {
                _customCursorViews[i] = Instantiate(_customCursors[i], _transformParent.ThisTransform);
                _customCursorViews[i].gameObject.SetActive(false);
            }
        }

        private IEnumerator<float> FollowMouse(Camera mainCamera)
        {
            while (true)
            {
                _currentCursor.transform.position = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                yield return Timing.WaitForOneFrame;
            }
        }
        #endregion

        #region MOUSE EFFECTS
        private void EnableEffect(Camera mainCamera)
        {
            _activeEffect = true;
            SetMouseEffect(_effect, _interval);
            Timing.RunCoroutine(InstantiateEffect(mainCamera), Segment.LateUpdate, JCoroutineTags.COROUTINE_MouseEffect);
        }

        private IEnumerator<float> InstantiateEffect(Camera mainCamera)
        {
            while (true)
            {
                _effect.TriggerEffectOnPosition(mainCamera.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
                yield return Timing.WaitForSeconds(_interval);
            }
        }
        #endregion

        #region COMMANDS - CURSOR
        /// <summary>
        /// resets the cursor to its default
        /// </summary>
        public void SetDefaultCursor() { SetCursor(_cursorView); }

        /// <summary>
        /// enables a custom cursor
        /// </summary>
        /// <param name="indexOfCursor">the index of the custom cursor</param>
        public void SetCustomCursor(int indexOfCursor)
        {
            Assert.IsTrue(indexOfCursor < _customCursorViews.Length,
                          $"{name} is trying to set the pointer at index {indexOfCursor}, but there are only {_customCursors.Length} pointers");

            Assert.IsNotNull(_customCursorViews[indexOfCursor], $"{name}- the view requested at {indexOfCursor}, is null");
            SetCursor(_customCursorViews[indexOfCursor]);
        }

        //sets a given cursor
        private void SetCursor(GameObject pointer)
        {
            CurrentCursor = pointer;
            OnPointerChange?.Invoke(CurrentCursor);
        }

        /// <summary>
        /// disables the cursor
        /// </summary>
        public void DisableMouseCursor()
        {
            //remove the cursor
            if (!_activeCursor) return;
            if (CurrentCursor != null) CurrentCursor.gameObject.SetActive(false);
            Timing.KillCoroutines(JCoroutineTags.COROUTINE_MouseEffect);
            //reset the system cursor
            Cursor.visible = true;
            _activeCursor  = false;
        }
        #endregion

        #region COMMANDS - EFFECT
        /// <summary>
        /// applies an effect to spawn on mouse
        /// </summary>
        /// <param name="specialEffect">the effect to spawn</param>
        /// <param name="interval">the interval for the effect</param>
        public void SetMouseEffect(J_Pool_SpecialEffects specialEffect, float interval, Camera cameraToSet = null)
        {
            if (_activeEffect) return;
            if (cameraToSet == null) cameraToSet = _mainCamera;
            Assert.IsNotNull(cameraToSet, $"{name}-No main camera set. Probably this is not started yet.");
            EnableEffect(cameraToSet);
        }

        /// <summary>
        /// stops the effect
        /// </summary>
        public void DisableMouseEffect()
        {
            if (!_activeEffect) return;
            Timing.KillCoroutines(JCoroutineTags.COROUTINE_MouseEffect);
            _activeEffect = false;
        }
        #endregion

        #region DISABLE AND RESET
        private void OnDisable() { ResetThis(); }
        public void ResetThis()
        {
            DisableMouseCursor();
            DisableMouseEffect();
        }
        #endregion
        #region SUBSCRIBERS
        public void Subscribe(JGenericDelegate<GameObject> actionToAdd) { OnPointerChange      += actionToAdd; }
        public void UnSubscribe(JGenericDelegate<GameObject> actionToRemove) { OnPointerChange -= actionToRemove; }
        #endregion
    }
}
