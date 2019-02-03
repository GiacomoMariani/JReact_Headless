using JReact;
using UnityEngine;

namespace JReact.UnityService
{
    public class Unity_AxisCatcher : iInputAxisGetter
    {
        public float GetAxis(string axisId) { return Input.GetAxis(axisId); }
        public float GetAxisRaw(string axisId) { return Input.GetAxisRaw(axisId); }
    }
}
