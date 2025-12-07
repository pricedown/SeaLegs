using UnityEngine;

namespace SeaLegs
{
    /// <summary>
    /// A triangle on the boat hull with computed properties
    /// - three vertices
    /// - cached center,
    /// - cached normal
    /// - unitnormal,
    /// - area
    /// Foundation for Kerner's approach to buoyancy
    /// https://www.gamedeveloper.com/programming/water-interaction-model-for-boats-in-video-games
    /// </summary>
    public struct HullTriangle
    {
        public Vector3 v0, v1, v2;
        public Vector3 Center => (v0 + v1 + v2) / 3f;
        public Vector3 Normal => Vector3.Cross(v1 - v0, v2 - v0);
        public Vector3 UnitNormal => Normal.normalized;
        public float Area => Normal.magnitude * 0.5f; // triangle maf : )

        public HullTriangle(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
        }
    }
}