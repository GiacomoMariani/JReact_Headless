using System.Collections.Generic;
using UnityEngine;

namespace JReact.Z_Experimental
{
    public sealed class J_SelectableDotTrace : MonoBehaviour
    {
        [SerializeField] private float _threshold = .975f;

        private List<iHittable> _hittable = new List<iHittable>();

        public iHittable Hit { get; private set; }

        public void Check(Ray2D ray)
        {
            Hit = null;

            Vector2 rayDirection = ray.direction.normalized;

            float closest = 0f;
            for (int i = 0; i < _hittable.Count; i++)
            {
                iHittable hittable        = _hittable[i];
                Vector2   objectDirection = (hittable.Position - ray.origin).normalized;

                float rayHitPercentage = Vector2.Dot(rayDirection, objectDirection);

                hittable.PercentageHit = rayHitPercentage;
                if (rayHitPercentage > _threshold &&
                    rayHitPercentage > closest)
                {
                    closest = rayHitPercentage;
                    Hit     = _hittable[i];
                }
            }
        }
    }

    public abstract class iHittable : MonoBehaviour
    {
        public Vector2 Position { get; protected set; }
        public float PercentageHit { get; set; }
    }
}
