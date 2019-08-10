using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Conditions
{
    /// <summary>
    /// checks if a given stackable pass a comparison
    /// </summary>
    /// <typeparam name="T">a stackable</typeparam>
    public abstract class J_ConditionCount<T> : J_ReactiveCondition
        where T : iStackable
    {
        //desired request
        [BoxGroup("Setup", true, true), SerializeField] private ComparisonType _comparison;
        [BoxGroup("Setup", true, true), SerializeField] private int _amount;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected abstract T _stackable { get; }

        protected override void StartCheckingCondition()
        {
            AmountChanged(_stackable.Current);
            _stackable.Subscribe(AmountChanged);
        }

        protected override void StopCheckingCondition() { _stackable.UnSubscribe(AmountChanged); }

        protected override void UpdateCondition()
        {
            base.UpdateCondition();
            AmountChanged(_stackable.Current);
        }

        protected virtual void AmountChanged(int currentAmount)
        {
            switch (_comparison)
            {
                case ComparisonType.Less:
                    Current = currentAmount < _amount;
                    break;

                case ComparisonType.Equal:
                    Current = currentAmount == _amount;
                    break;

                case ComparisonType.More:
                    Current = currentAmount > _amount;
                    break;

                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}
