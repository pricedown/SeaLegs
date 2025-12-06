using System;
using UnityEngine;

namespace SeaLegs
{
    [Serializable]
    public class FloaterPoint
    {
        public Transform transform;
        
        public float WaterHeight { get; private set; }
        public bool IsSubmerged { get; private set; }
        public float SubmersionRatio { get; private set; }

        public void UpdateState(IWater water, float submersionDepth, float buoyancyCoefficient)
        {
            WaterHeight = water.GetWaveDisplacementAt(transform.position).y;
            IsSubmerged = transform.position.y < WaterHeight;

            if (IsSubmerged)
            {
                var depth = WaterHeight - transform.position.y;
                SubmersionRatio = Mathf.Clamp01(depth / submersionDepth * buoyancyCoefficient);
            }
            else
            {
                SubmersionRatio = 0f;
            }
        }
    }
}