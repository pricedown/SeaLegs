using UnityEngine;

namespace SeaLegs
{
    [CreateAssetMenu(fileName = "BuoyancySettings", menuName = "SeaLegs/Buoyancy Settings")]
    public class BuoyancySettings : ScriptableObject
    {
        [Header("Buoyancy")] 
        [Tooltip("How deep an object sinks before full buoyancy")] public float submersionDepth = 1f;
        public float buoyancyCoefficient = 1f;

        [Header("Damping")] 
        public float linearDamping = 1f;
        public float angularDamping = 5f;

        [Header("Debug")] 
        public float gizmoRadius = 0.3f;
        public Color submergedColor = Color.green;
        public Color aboveWaterColor = Color.red;
    }
}