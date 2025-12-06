using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeaLegs
{
    [Obsolete("Composite floater is obsolete, please use BuoyancyBody instead")]
    public class CompositeFloater : MonoBehaviour
    {
        [SerializeField] public List<Transform> floaters = new();
        [SerializeField] protected Rigidbody _rb;
        [SerializeField] protected IWater water;

        [Header("Floater Settings")]
        public float submersionDepth = 1;
        public float buoyancyCoefficient = 1;
        public float linearDampingCoefficient = 1;
        public float angularDampingCoefficient = 5;

        [Header("Debugging")]
        [SerializeField] protected float gizmoSize = 0.3f;

        void FixedUpdate()
        {
            // Update all floaters
            foreach (Transform t in floaters)
                UpdateFloater(t.position, Time.fixedDeltaTime);
        }

        // Update one floater
        protected virtual void UpdateFloater(Vector3 floaterPosition, float dt)
        {
            float waterHeight = water.GetWaveDisplacementAt(floaterPosition).y;
            var submersionRatio = 0f;

            if (!IsSubmerged(floaterPosition, waterHeight))
                return;

            submersionRatio = GetSubmersionRatio(floaterPosition, waterHeight);

            // Gravity
            _rb.AddForceAtPosition(Physics.gravity / floaters.Count, floaterPosition, ForceMode.Acceleration);

            // Buoyant force
            _rb.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * submersionRatio, 0f),
                floaterPosition, ForceMode.Acceleration);

            // Damp velocities
            _rb.AddForce(-_rb.linearVelocity * (linearDampingCoefficient * submersionRatio * dt),
                ForceMode.VelocityChange);
            _rb.AddTorque(-_rb.angularVelocity * (angularDampingCoefficient * submersionRatio * dt),
                ForceMode.VelocityChange);
        }
    
        void Awake()
        {
            _rb.useGravity = false;
            Debug.LogWarning($"{gameObject.name}: Composite floater is obsolete, please use BuoyancyBody instead");
        }

        bool IsSubmerged(Vector3 position, float waterHeight)
        {
            return transform.position.y < waterHeight;
        }

        float GetSubmersionRatio(Vector3 position, float waterHeight)
        {
            return Mathf.Clamp01((waterHeight - position.y) / submersionDepth * buoyancyCoefficient);
        }

        void OnDrawGizmos()
        {
            foreach (Transform f in floaters)
            {
                Gizmos.color = IsSubmerged(f.position, water.GetWaveDisplacementAt(f.position).y) ? Color.green : Color.red;
                Gizmos.DrawSphere(f.position, gizmoSize);
            }
        }

    }
}
