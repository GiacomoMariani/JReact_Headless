using UnityEngine;
using System;

namespace JReact
{
    [CreateAssetMenu(menuName = "Reactive/Basics/Reactive Bool")]
    public class J_ReactiveBool : J_ReactiveElement<bool>
    { 
        public static bool operator true(J_ReactiveBool element) { return element.CurrentValue; }
        public static bool operator false(J_ReactiveBool element) { return !element.CurrentValue; }
    }
}