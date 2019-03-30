using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    /// <summary>
    /// this is a base event we can reference on unity editor
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Events/Event")]
    public class J_SO_Event : ScriptableObject, iObservable
    {
        private event JAction OnEnter;

        [ButtonGroup("Commands", 200), Button("Activate", ButtonSizes.Medium)] public void RaiseEvent() { OnEnter?.Invoke(); }

        public void Subscribe(JAction actionToSubscribe) { OnEnter   += actionToSubscribe; }
        public void UnSubscribe(JAction actionToSubscribe) { OnEnter -= actionToSubscribe; }
    }
}
