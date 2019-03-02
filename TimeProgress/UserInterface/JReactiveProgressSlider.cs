using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace JReact.TimeProgress
{
    /// <summary>
    /// connects a slidder with a progress event
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class JReactiveProgressSlider : J_Mono_ProgressView
    {
        #region FIELDS AND PROPERTIES
        private Slider _thisSlider;
        private Slider ThisSlider
        {
            get
            {
                if (_thisSlider == null) _thisSlider = GetComponent<Slider>();
                return _thisSlider;
            }
        }
        #endregion

        protected override void SanityChecks()
        {
            base.SanityChecks();
            //setting the base values
            Assert.IsTrue(Math.Abs(ThisSlider.minValue) < JConstants.GeneralFloatTolerance,
                          "Slider min value must be 0");

            Assert.IsTrue(Math.Abs(ThisSlider.maxValue - 1.0f) < JConstants.GeneralFloatTolerance,
                          "Slider max value must be 1");
        }

        #region ABSTRACT IMPLEMENTATION
        //setup the slider at enable
        protected override void ViewEnabled(J_Progress progress)
        {
            base.ViewEnabled(progress);
            if (progress.IsRunning) ThisSlider.value = progress.ProgressPercentage;
            gameObject.SetActive(progress.IsRunning);
        }

        protected override void ProgressStart(J_Progress progress)
        {
            ThisSlider.value = progress.ProgressPercentage;
            gameObject.SetActive(true);
        }

        protected override void ProgressUpdate(J_Progress progress) { ThisSlider.value = progress.ProgressPercentage; }

        protected override void ProgressComplete(J_Progress progress)
        {
            gameObject.SetActive(false);
            ThisSlider.value = 0f;
        }
        #endregion
    }
}
