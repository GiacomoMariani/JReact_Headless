using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace JReact.StateControls
{
    /// <summary>
    /// uses a click event to send an action
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class J_ColliderAction : MonoBehaviour, IPointerClickHandler
    {
        [BoxGroup("Setup", true, true, 0), SerializeField] private JUnityEvent _unityEventToSend;

        protected virtual void SendCommand() { _unityEventToSend.Invoke(); }

        public void OnPointerClick(PointerEventData eventData) { SendCommand(); }
    }
}
