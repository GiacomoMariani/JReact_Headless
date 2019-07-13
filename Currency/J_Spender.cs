using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Currency
{
    public abstract class J_Spender : ScriptableObject
    {
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_Fillable _resource;

        public bool TrySpending(int amount)
        {
            if(_resource.HasEnough(amount))
            {
                _resource.Remove(amount);
                return true;
            }

            //if not we start an action based on the spender and return false
            NoSpendFeedback(_resource.HowMuchToReach(amount));
            return false;
        }

        protected abstract void NoSpendFeedback(int missingResources);
    }
}

