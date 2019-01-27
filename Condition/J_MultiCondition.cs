using Sirenix.OdinInspector;
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
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private JReactiveBool[] _trueConditions;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private JReactiveBool[] _falseConditions;
        #endregion

        #region INITIALIZATION METHODS
        protected override void InitializeCheck()
        {
            ConditionsCheck(false);
            _trueConditions.SubscribeToAll(ConditionsCheck);
            _falseConditions.SubscribeToAll(ConditionsCheck);
        }

        protected override void DeInitializeCheck()
        {
            _trueConditions.UnSubscribeToAll(ConditionsCheck);
            _falseConditions.UnSubscribeToAll(ConditionsCheck);
        }
        #endregion

        private void ConditionsCheck(bool item)
        {
            if (_operator == OperatorType.And) AndOperator();
            else OrOperator();
        }

        #region AND
        //checks AND conditions
        private void AndOperator()
        {
            CurrentValue = RequiresAllCondition(_trueConditions, true) &&
                           RequiresAllCondition(_falseConditions, false);
        }

        private bool RequiresAllCondition(JReactiveBool[] collectionToCheck, bool expectedValue)
        {
            //check all true
            for (int i = 0; i < collectionToCheck.Length; i++)
                if (collectionToCheck[i].CurrentValue != expectedValue)
                    return false;

            return true;
        }
        #endregion

        #region OR
        private void OrOperator()
        {
            CurrentValue = RequiresOneCondition(_trueConditions, true) ||
                           RequiresOneCondition(_falseConditions, false);
        }

        //check all the values
        private bool RequiresOneCondition(JReactiveBool[] collectionToCheck, bool expectedValue)
        {
            //check all true
            for (int i = 0; i < collectionToCheck.Length; i++)
                if (collectionToCheck[i].CurrentValue == expectedValue)
                    return true;

            return false;
        }
        #endregion
    }
}
