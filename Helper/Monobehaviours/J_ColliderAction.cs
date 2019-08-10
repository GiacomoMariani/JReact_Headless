using JReact.Conditions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JReact
{
    /// <summary>
    /// uses a click event to send an action
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public sealed class J_ColliderAction : MonoBehaviour, IPointerClickHandler
    {
        [BoxGroup("Setup", true, true), SerializeField] private J_ReactiveCondition[] _conditions;
        [BoxGroup("Setup", true, true), SerializeField] private JUnityEvent _unityEventToSend;

        private void SendCommand()
        {
            if (_conditions.AndOperator()) _unityEventToSend.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData) => SendCommand();
    }
}
