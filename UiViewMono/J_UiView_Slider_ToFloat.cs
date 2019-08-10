using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace JReact.UiView
{
    [RequireComponent(typeof(Slider))]
    public class J_UiView_Slider_ToFloat : MonoBehaviour
    {
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_ReactiveFloat _floatValue;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Slider _slider;
        private Slider ThisSlider
        {
            get
            {
                if (_slider == null) _slider = GetComponent<Slider>();
                return _slider;
            }
        }

        private void UpdateValue(float sliderValue) => _floatValue.Current = sliderValue;

        private void OnEnable()
        {
            ThisSlider.value = _floatValue.Current;
            _slider.onValueChanged.AddListener(UpdateValue);
        }

        private void OnDisable() { _slider.onValueChanged.RemoveListener(UpdateValue); }
    }
}
