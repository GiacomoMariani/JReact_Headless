﻿using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Conditions
{
    /// <summary>
    /// checks multiple conditions
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Conditions/Multi", fileName = "MULTI_Condition")]
    public class J_MultiCondition : J_ReactiveCondition
    {
        #region FIELDS AND PROPERTIES
        [BoxGroup("Setup", true, true, 0), SerializeField] private OperatorType _operator = OperatorType.And;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_ReactiveBool[] _trueConditions;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_ReactiveBool[] _falseConditions;
        #endregion

        #region INITIALIZATION METHODS
        protected override void StartCheckingCondition()
        {
            ConditionsCheck(false);
            _trueConditions.SubscribeToAll(ConditionsCheck);
            _falseConditions.SubscribeToAll(ConditionsCheck);
        }

        protected override void StopCheckingCondition()
        {
            _trueConditions.UnSubscribeToAll(ConditionsCheck);
            _falseConditions.UnSubscribeToAll(ConditionsCheck);
        }
        #endregion

        protected override void UpdateCondition()
        {
            base.UpdateCondition();
            ConditionsCheck(false);
        }

        private void ConditionsCheck(bool item)
        {
            if (_operator == OperatorType.And) AndOperator();
            else OrOperator();
        }

        #region AND
        //checks AND conditions
        private void AndOperator()
        {
            Current = RequiresAllCondition(_trueConditions,  true) &&
                           RequiresAllCondition(_falseConditions, false);
        }

        private bool RequiresAllCondition(J_ReactiveBool[] collectionToCheck, bool expectedValue)
        {
            //check all true
            for (int i = 0; i < collectionToCheck.Length; i++)
            {
                if (collectionToCheck[i].Current != expectedValue)
                    return false;
            }

            return true;
        }
        #endregion

        #region OR
        private void OrOperator()
        {
            Current = RequiresOneCondition(_trueConditions,  true) ||
                           RequiresOneCondition(_falseConditions, false);
        }

        //check all the values
        private bool RequiresOneCondition(J_ReactiveBool[] collectionToCheck, bool expectedValue)
        {
            //check all true
            for (int i = 0; i < collectionToCheck.Length; i++)
            {
                if (collectionToCheck[i].Current == expectedValue)
                    return true;
            }

            return false;
        }
        #endregion
    }
}
