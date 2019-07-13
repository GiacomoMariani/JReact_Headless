using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.UiView
{
    public class J_RectToScreenPosition : MonoBehaviour
    {
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_ReactiveVector2 _screenPoint;

        [BoxGroup("Setup", true, true, 0), SerializeField] private Camera _camera;
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private RectTransform _rect;
        private RectTransform _ThisRect
        {
            get
            {
                if (_rect == null) _rect = GetComponent<RectTransform>();
                return _rect;
            }
        }

        private void Awake() => UpdateValue();

        private void UpdateValue() => _screenPoint.Current = _ThisRect.ToScreenPosition(_camera);
    }
}
