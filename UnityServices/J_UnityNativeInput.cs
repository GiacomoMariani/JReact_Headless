using UnityEngine;

namespace JReact.UnityService
{
    public class Unity_AxisCatcher : iInputAxisGetter
    {
        public float GetAxis(string axisId) => Input.GetAxis(axisId);
        public float GetAxisRaw(string axisId) => Input.GetAxisRaw(axisId);
    }
}
