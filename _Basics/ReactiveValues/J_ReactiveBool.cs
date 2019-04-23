using UnityEngine;

namespace JReact
{
    [CreateAssetMenu(menuName = "Reactive/Basics/Reactive Bool")]
    public class J_ReactiveBool : J_ReactiveItem<bool>
    {
        public static bool operator true(J_ReactiveBool item) => item.Current;
        public static bool operator false(J_ReactiveBool item) => !item.Current;
    }
}
