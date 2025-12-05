using UnityEngine;

namespace SeaLegs
{
    public interface IWater
    {
        public Vector3 GetWaveDisplacementAt(Vector3 position);
    }
}
