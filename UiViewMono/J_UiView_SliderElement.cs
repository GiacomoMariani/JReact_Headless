using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace JReact.UiView
{
    [RequireComponent(typeof(Slider))]
    public class J_UiView_SliderElement : MonoBehaviour
    {
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_ReactiveFloat _sliderValue;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Slider _slider;
        private Slider ThisSlider
        {
            get
            {
                if (_slider == null) _slider = GetComponent<Slider>();
                return _slider;
            }
        }

        private void UpdateValue(float sliderValue) => _sliderValue.Current = sliderValue;

        private void OnEnable()
        {
            _sliderValue.Current = ThisSlider.value;
            _slider.onValueChanged.AddListener(UpdateValue);
        }

        private void OnDisable() { _slider.onValueChanged.RemoveListener(UpdateValue); }
    }
}
