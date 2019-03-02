using System.Collections.Generic;
using DG.Tweening;
using JReact.Sound;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace JReact.StateControl.Weather
{
    /// <summary>
    /// darkens the sky on specicic states
    /// </summary>
    public class J_Mono_DarkenSky : J_Mono_StateElement
    {
        #region FIELDS AND PROPERTIES
        private const string COROUTINE_ThunderTag = "COROUTINE_ThunderTag";

        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private Image _darkSky;
        private Image DarkSky
        {
            get
            {
                if (_darkSky == null) _darkSky = GetComponent<Image>();
                return _darkSky;
            }
        }

        // --------------- SKY SETUP --------------- //
        [BoxGroup("Setup - Sky", true, true, 0), SerializeField, Range(0.00f, 0.25f)]
        private float _clearAlpha;
        [BoxGroup("Setup - Sky", true, true, 0), SerializeField, Range(0.25f, 0.99f)]
        private float _darkAlpha = .5f;
        [BoxGroup("Setup - Sky", true, true, 0), SerializeField] private float _secondsToGetDark = 1.0f;

        // --------------- THUNDER SETUP --------------- //
        [BoxGroup("Setup - Thunder", true, true, 0), SerializeField] private Vector2 _thunderIntervals = new Vector2(5f, 30f);
        [BoxGroup("Setup - Thunder", true, true, 0), SerializeField] private float _thunderChance = .5f;
        [BoxGroup("Setup - Thunder", true, true, 0), SerializeField] private float _thunderLength = 4f;
        [BoxGroup("Setup - Thunder", true, true, 0), SerializeField] private string _thunderSound;
        [BoxGroup("Setup - Thunder", true, true, 0), SerializeField] private AnimationCurve _thunderCurve;
        [BoxGroup("Setup - Thunder", true, true, 0), SerializeField] private SoundControl _soundControl;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Tween _thunderTween;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _instanceId;
        #endregion

        protected override void InitThis()
        {
            base.InitThis();
            _instanceId = GetInstanceID();
        }

        protected override void SanityChecks()
        {
            base.SanityChecks();
            Assert.IsNotNull(DarkSky,       $"{gameObject.name} requires a _darkSky");
            Assert.IsNotNull(_soundControl, $"{gameObject.name} requires a _soundControl");
            Assert.IsTrue(DarkSky.color.a <= 0.01,
                          $"{gameObject.name}- the alpha of the image {DarkSky.gameObject.name} should start at 0");
        }

        #region COMMANDS
        /// <summary>
        /// darkens the sky
        /// </summary>
        private void DarkenSky()
        {
            DarkSky.DOFade(_darkAlpha, _secondsToGetDark);
            if (_thunderChance > 0f)
                Timing.RunCoroutine(ThunderLoop().CancelWith(gameObject), Segment.SlowUpdate, _instanceId, COROUTINE_ThunderTag);
        }

        /// <summary>
        /// clears the sky
        /// </summary>
        private void ClearSky()
        {
            Timing.KillCoroutines(_instanceId, COROUTINE_ThunderTag);
            _thunderTween?.Kill();
            DarkSky.DOFade(_clearAlpha, _secondsToGetDark);
        }
        #endregion

        #region THUNDERS
        //keep thundering until is raining
        private IEnumerator<float> ThunderLoop()
        {
            while (true)
            {
                TryThundering();
                yield return Timing.WaitForSeconds(_thunderIntervals.GetRandomValue());
            }
        }

        private void TryThundering()
        {
            if (_thunderChance.ChanceHit()) PlayThunder();
        }

        [BoxGroup("Test", true, true, 100), Button(ButtonSizes.Medium)]
        private void PlayThunder()
        {
            _thunderTween = null;
            _soundControl.PlaySound(_thunderSound);
            _thunderTween = DarkSky.DOFade(_darkAlpha, _secondsToGetDark).SetEase<Tween>(_thunderCurve);
        }
        #endregion

        #region ABSTRACT IMPLEMENTATION
        protected override void EnterState() { DarkenSky(); }
        protected override void ExitState() { ClearSky(); }
        #endregion
    }
}
