using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace JReact
{
    public class JEvent<T>
    {
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Length => _actions.Count;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private List<Action<T>> _actions = new List<Action<T>>();

        public void Call(T firstParameter)
        {
            for (int i = 0; i < Length; i++)
                _actions[i].Invoke(firstParameter);
        }

        public void Add(Action<T> action) => _actions.Add(action);

        public void Remove(Action<T> action)
        {
            if (_actions.Contains(action)) _actions.Remove(action);
        }
    }
}
