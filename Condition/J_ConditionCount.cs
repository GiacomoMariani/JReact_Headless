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
        [BoxGroup("Setup", true, true, 0), SerializeField] private ComparisonType _comparison;
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _amount;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected abstract T _stackable { get; }

        protected override void StartCheckingCondition()
        {
            AmountChanged(_stackable.CurrentValue);
            _stackable.Subscribe(AmountChanged);
        }

        protected override void StopCheckingCondition() { _stackable.UnSubscribe(AmountChanged); }

        protected override void UpdateCondition()
        {
            base.UpdateCondition();
            AmountChanged(_stackable.CurrentValue);
        }

        protected virtual void AmountChanged(int currentAmount)
        {
            switch (_comparison)
            {
                case ComparisonType.Less:
                    CurrentValue = currentAmount < _amount;
                    break;
                case ComparisonType.Equal:
                    CurrentValue = currentAmount == _amount;
                    break;
                case ComparisonType.More:
                    CurrentValue = currentAmount > _amount;
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}
