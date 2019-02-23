using UnityEngine;
using System;

namespace JReact
{
    [CreateAssetMenu(menuName = "Reactive/Basics/Reactive Bool")]
    public class J_ReactiveBool : J_ReactiveItem<bool>
    {
        public static bool operator true(J_ReactiveBool item) { return item.CurrentValue; }
        public static bool operator false(J_ReactiveBool item) { return !item.CurrentValue; }
    }
}
