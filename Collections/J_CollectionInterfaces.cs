using System;
using System.Collections.Generic;

namespace JReact.Collections
{
    public interface iReactiveIndexCollection<out T> : iReactiveCollection<T>
    {
        int Length { get; }
        T this[int index] { get; }
    }

    public interface iReactiveEnumerable<out T> : iReactiveCollection<T>
    {
        IEnumerator<T> GetEnumerator();
    }

    public interface iReactiveCollection<out T>
    {
        void SubscribeToAdd(Action<T>      action);
        void SubscribeToRemove(Action<T>   action);
        void UnSubscribeToAdd(Action<T>    action);
        void UnSubscribeToRemove(Action<T> action);
    }
}
