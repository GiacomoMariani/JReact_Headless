﻿using JReact.UiView;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.StateControls.LevelSystem
{
    /// <summary>
    /// shows the current level on a text
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class J_UiView_LevelText : J_UiView_TextElement
    {
        [BoxGroup("Setup", true, true, 0), SerializeField, Required, AssetsOnly] private J_LevelProgression _levelControl;

        protected override void SanityChecks()
        {
            base.SanityChecks();
            Assert.IsNotNull(_levelControl, $"{name} requires a levelControl");
        }

        private void UpdateText(J_LevelState previous, J_LevelState current) { SetText(current.ToString()); }

        #region LISTENERS
        private void OnEnable()
        {
            UpdateText(null, _levelControl.CurrentLevelInfo);
            _levelControl.Subscribe(UpdateText);
        }

        private void OnDisable() { _levelControl.UnSubscribe(UpdateText); }
        #endregion
    }
}
