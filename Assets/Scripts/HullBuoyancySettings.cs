using UnityEngine;

namespace SeaLegs
{
    [CreateAssetMenu(menuName = "SeaLegs/Hull Buoyancy Settings")]
    public class HullBuoyancySettings : ScriptableObject
    {
        [Header("Physics")] public float waterDensity = 1025f;

        public float forceScale = 1f;

        [Header("Damping")] public float linearDamping = 1f;

        public float angularDamping = 5f;

        [Header("Debug")] public bool showSubmergedTriangles = true;

        public bool logForces;
        public Color submergedColor = new(0f, 1f, 0f, 0.3f);
    }
}