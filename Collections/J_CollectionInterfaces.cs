using System;
using UnityEngine;

namespace JReact.Collections
{
    public interface iReactiveCollection<out T>
    {
        int Length { get; }
        T this[int index] { get; }
        void SubscribeToAdd(Action<T> action);
        void SubscribeToRemove(Action<T> action);
        void UnSubscribeToAdd(Action<T> action);
        void UnSubscribeToRemove(Action<T> action);
    }
}
