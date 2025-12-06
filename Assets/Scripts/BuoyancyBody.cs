using System.Collections.Generic;
using UnityEngine;

namespace SeaLegs
{
    [RequireComponent(typeof(Rigidbody))]
    public class BuoyancyBody : MonoBehaviour, IBuoyant
    {
        [Header("References")] 
        [SerializeField] private Rigidbody rb;
        [SerializeField] private BuoyancySettings settings;

        [Header("Floater Points")] 
        [SerializeField] private List<FloaterPoint> floaterPoints = new();
        private IWater _water;

        // IBuoyant
        public float TotalBuoyancy { get; private set; }
        public float SubmersionRatio { get; private set; }
        public bool IsFloating => SubmersionRatio > 0f;
        
        private void Awake()
        {
            if (rb == null) 
                rb = GetComponent<Rigidbody>();
            
            if (settings == null) 
                Debug.LogError($"No BuoyancySettings on {gameObject.name}");
            
            rb.useGravity = false;
        }

        private void Start()
        {
            // Find the water if unset
            if (_water == null)
            {
                var waterBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
                foreach (var behaviour in waterBehaviours)
                    if (behaviour is IWater water)
                        _water = water;
            }
            
            if (_water == null)
                Debug.LogError($"No water found");
        }

        private void FixedUpdate()
        {
            if (_water == null) return;

            var dt = Time.fixedDeltaTime;
            var totalSubmersion = 0f;

            // Update state of all floaters & float them
            foreach (var floater in floaterPoints)
            {
                floater.UpdateState(_water, settings.submersionDepth, settings.buoyancyCoefficient);
                ApplyForcesAtFloater(floater, dt);
                totalSubmersion += floater.SubmersionRatio;
            }

            SubmersionRatio = totalSubmersion / floaterPoints.Count;
            TotalBuoyancy = totalSubmersion;
        }

        private void ApplyForcesAtFloater(FloaterPoint floater, float dt)
        {
            var position = floater.transform.position;

            var gravityPerFloater = Physics.gravity.y / floaterPoints.Count;
            
            // Apply gravity
            rb.AddForceAtPosition(
                new Vector3(0f, gravityPerFloater, 0f),
                position,
                ForceMode.Acceleration
            );

            if (!floater.IsSubmerged) 
                return;

            var submersion = floater.SubmersionRatio;

            var buoyantForce = Mathf.Abs(gravityPerFloater) * submersion;
            rb.AddForceAtPosition(
                new Vector3(0f, buoyantForce, 0f),
                position,
                ForceMode.Acceleration
            );

            ApplyDamping(submersion, dt);
        }

        private void ApplyDamping(float submersionRatio, float dt)
        {
            var linearDampForce = -rb.linearVelocity * (settings.linearDamping * submersionRatio * dt);
            rb.AddForce(linearDampForce, ForceMode.VelocityChange);

            var angularDampTorque = -rb.angularVelocity * (settings.angularDamping * submersionRatio * dt);
            rb.AddTorque(angularDampTorque, ForceMode.VelocityChange);
        }

        private void AutoFindFloaterPoints()
        {
            floaterPoints.Clear();
            foreach (Transform child in transform)
                if (child.name.Contains("Floater") || child.CompareTag("Floater"))
                    floaterPoints.Add(new FloaterPoint { transform = child });

            Debug.Log($"Found {floaterPoints.Count} floater points");
        }

        #region Debug

        private void OnDrawGizmos()
        {
            // Draw spheres on each floater!
            
            if (settings == null || _water == null) return;

            foreach (var floater in floaterPoints)
            {
                if (floater.transform == null) continue;

                var waterHeight = _water.GetWaveDisplacementAt(floater.transform.position).y;
                var isSubmerged = floater.transform.position.y < waterHeight;

                Gizmos.color = isSubmerged ? settings.submergedColor : settings.aboveWaterColor;
                Gizmos.DrawSphere(floater.transform.position, settings.gizmoRadius);

                Gizmos.color = Color.cyan;
                var waterPoint = new Vector3(floater.transform.position.x, waterHeight, floater.transform.position.z);
                Gizmos.DrawLine(floater.transform.position, waterPoint);
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Draw connections between floaters
            
            if (floaterPoints.Count < 2) return;

            Gizmos.color = Color.yellow;
            for (var i = 0; i < floaterPoints.Count; i++)
            {
                var current = floaterPoints[i];
                var next = floaterPoints[(i + 1) % floaterPoints.Count];

                if (current.transform != null && next.transform != null)
                    Gizmos.DrawLine(current.transform.position, next.transform.position);
            }
        }

        #endregion
    }
}