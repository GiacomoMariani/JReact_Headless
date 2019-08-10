using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    /// <summary>
    /// this is a base event we can reference on unity editor
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Events/Event")]
    public sealed class J_Event : ScriptableObject, jObservable
    {
        private event Action OnEnter;

        [ButtonGroup("Commands", 200), Button("Activate", ButtonSizes.Medium)] public void RaiseEvent() { OnEnter?.Invoke(); }

        public void Subscribe(Action   actionToSubscribe) { OnEnter += actionToSubscribe; }
        public void UnSubscribe(Action actionToSubscribe) { OnEnter -= actionToSubscribe; }
    }
}
