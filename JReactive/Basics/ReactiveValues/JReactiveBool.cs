using UnityEngine;
using System;

namespace JReact
{
    [CreateAssetMenu(menuName = "Reactive/Basics/Reactive Bool")]
    public class JReactiveBool : J_ReactiveElement<bool>
    { 
        public static bool operator true(JReactiveBool element) { return element.CurrentValue; }
        public static bool operator false(JReactiveBool element) { return !element.CurrentValue; }
    }
}