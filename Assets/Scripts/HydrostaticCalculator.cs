using System.Collections.Generic;
using UnityEngine;

namespace SeaLegs
{
    // Result of the calculation
    public struct HydrostaticResult
    {
        public int SubmergedTriangleCount;

        // F = -p * g * h * A * n
        // Where:
        // p = water density (kg/m^3)
        // g = accel due to gravity (m/s^2)
        // h = depth of triangle center below surface (m)
        // A = triangle area (m^2)
        // n = triangle's outward-facing normal
        public Vector3 Force;

        // t = r x F
        public Vector3 Torque;
    }


    public class HydrostaticCalculator
    {
        private readonly List<HullTriangle> _submergedTriangles = new(64);

        public HydrostaticResult Calculate(
            HullTriangle[] hullTriangles,
            IWater water,
            Vector3 centerOfMass,
            float waterDensity)
        {
            _submergedTriangles.Clear();

            foreach (var tri in hullTriangles)
            {
                var h0 = water.GetHeightAboveWater(tri.v0);
                var h1 = water.GetHeightAboveWater(tri.v1);
                var h2 = water.GetHeightAboveWater(tri.v2);

                TriangleCutter.Cut(tri.v0, h0, tri.v1, h1, tri.v2, h2, _submergedTriangles);
            }

            var totalForce = Vector3.zero;
            var totalTorque = Vector3.zero;
            var gravity = Mathf.Abs(Physics.gravity.y);

            foreach (var tri in _submergedTriangles)
            {
                var center = tri.Center;
                var normal = tri.UnitNormal;
                var area = tri.Area;

                var depth = -water.GetHeightAboveWater(center);
                if (depth <= 0)
                    // Skip if center not submerged
                    continue;

                var pressure = waterDensity * gravity * depth;
                var force = -pressure * area * normal;

                totalForce += force;

                var r = center - centerOfMass;
                totalTorque += Vector3.Cross(r, force);
            }

            return new HydrostaticResult
            {
                Force = totalForce,
                Torque = totalTorque,
                SubmergedTriangleCount = _submergedTriangles.Count
            };
        }
    }
}