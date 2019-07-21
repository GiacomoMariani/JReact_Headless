namespace JReact.Conditions
{
    public static class J_ConditionsExtensions
    {
        public static bool AndOperator(this J_ReactiveCondition[] conditions, bool ignoreNulls = false)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                if(conditions[i] == null && ignoreNulls) continue;
                if (!conditions[i].Current) return false;
            }

            return true;
        }
        
        public static bool OrOperator(this J_ReactiveCondition[] conditions, bool ignoreNulls = false)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                if(conditions[i] == null && ignoreNulls) continue;
                if (conditions[i].Current) return true;
            }

            return false;
        }
    }
}
