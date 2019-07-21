using System;
using UnityEngine;

namespace JReact
{
    [CreateAssetMenu(menuName = "Reactive/Basics/Reactive Action")]
    public sealed class J_ReactiveAction : J_ReactiveItem<Action>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        public void Process() => Current?.Invoke();
    }
}
