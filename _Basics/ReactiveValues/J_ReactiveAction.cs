using System;
using UnityEngine;
using UnityEngine.Events;

namespace JReact
{
    [CreateAssetMenu(menuName = "Reactive/Basics/Reactive Action")]
    public sealed class J_ReactiveAction : J_ReactiveItem<Action>, iProcessable
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        public UnityAction ThisAction => Process;

        public void Process() => Current?.Invoke();
    }
}
