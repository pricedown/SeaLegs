using UnityEngine;

namespace SeaLegs
{
    public abstract class WaterBase : MonoBehaviour, IWater
    {
        public abstract Vector3 GetWaveDisplacementAt(Vector3 position);
        public abstract float GetHeightAboveWater(Vector3 point);
    }
}