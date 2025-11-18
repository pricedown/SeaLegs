using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeaLegs
{
    [RequireComponent(typeof(Rigidbody))]
    public class FloatingObject : MonoBehaviour
    {
        public List<Transform> floatPoints = new List<Transform>();
        public float floatStrength = 1f;
        public float objectHeight = 1f;
        public float objectVolume = 1f;
        public float waterDensity = 1000;
        public float dampeningFactor = 0.9f;
        
        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            float waveHeight = Waves.Instance.GetHeight(transform.position.x);
            float distanceAboveWater = transform.position.y - waveHeight;
            if (distanceAboveWater < 0)
            {
                Vector3 buoyancy = Vector3.up * (floatStrength * Mathf.Abs(distanceAboveWater) * Physics.gravity.magnitude * objectVolume * waterDensity);

                _rb.AddForceAtPosition(buoyancy, transform.position, ForceMode.Force);
                _rb.AddForceAtPosition(-_rb.linearVelocity * (dampeningFactor * objectVolume), transform.position, ForceMode.Force);
            }
        }
    }
}
