﻿using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace JReact.UiView
{
    /// <summary>
    /// used to show a button
    /// </summary>
    [RequireComponent(typeof(Button))]
    public abstract class J_UiView_ButtonElement : MonoBehaviour
    {
        //the button related to this element
        private Button _thisButton;
        [BoxGroup("View", true, true, 50), ReadOnly, ShowInInspector] protected Button ThisButton
        {
            get
            {
                if (_thisButton == null) _thisButton = GetComponent<Button>();
                return _thisButton;
            }
        }

        #region INITIALIZATION
        //initialization
        private void Awake()
        {
            InitThis();
            SanityChecks();
        }

        //element used for initialization
        protected virtual void InitThis() {}

        //sanity checks
        protected virtual void SanityChecks() { Assert.IsNotNull(ThisButton, "Requires a Button: " + gameObject); }
        #endregion

        #region PRECHECKS
        //this is used in case we want to apply any condition, as default it is true
        protected virtual bool CanBePressed() => true;

        //try pressing the button, send the command if the button can be pressed
        private void TryPressButton()
        {
            if (CanBePressed()) ButtonCommand();
        }
        #endregion

        #region INTERFACE
        //the main command sent by this button
        protected abstract void ButtonCommand();
        #endregion

        #region LISTENERS
        //start and stop tracking on enable
        protected virtual void OnEnable() { ThisButton.onClick.AddListener(TryPressButton); }
        protected virtual void OnDisable() { ThisButton.onClick.RemoveListener(TryPressButton); }
        #endregion
    }
}
