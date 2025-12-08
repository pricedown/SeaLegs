using UnityEngine;
using UnityEngine.Serialization;

namespace SeaLegs
{
    /// <summary>
    ///     Hull-based buoyancy using hydrostatic pressure on submerged triangles.
    ///     Alternative to the floater-point based BuoyancyBody.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class HullBuoyancyBody : MonoBehaviour, IBuoyant
    {
        private static readonly int warnTriCount = 100;

        [Header("References")] [SerializeField]
        private Rigidbody rb;

        [SerializeField] private WaterBase _water;

        [FormerlySerializedAs("_hullBuoyancySettings")] [SerializeField]
        private HullBuoyancySettings settings;

        [Header("Hull")] [SerializeField] private MeshFilter hullMeshFilter;

        [Header("Debug")] [SerializeField] private bool logForces;

        [Tooltip("Try this if direction of force is wrong")] [SerializeField]
        private bool flipNormals;

        private HydrostaticCalculator _calculator;
        private HullTriangle[] _localHullTriangles;
        private HullTriangle[] _worldHullTriangles;

        private void Awake()
        {
            if (settings == null)
                Debug.LogError($"No BuoyancySettings on {gameObject.name}");

            // Disable gravity so we can apply our own
            rb.useGravity = false;

            // Extract triangles from hull mesh
            BuildHullTriangles();

            _calculator = new HydrostaticCalculator();
        }

        private void Start()
        {
            if (_water == null)
                Debug.LogError($"No water set on {gameObject.name}");
        }

        private void FixedUpdate()
        {
            if (_water == null || _localHullTriangles == null || _localHullTriangles.Length == 0)
                return;

            TransformHullToWorld();

            // Calculate buoyancy
            var result = _calculator.Calculate(
                _worldHullTriangles,
                _water,
                rb.worldCenterOfMass,
                settings.waterDensity
            );


            rb.AddForce(Physics.gravity, ForceMode.Acceleration);

            var scaledForce = result.Force * settings.forceScale;
            var scaledTorque = result.Torque * settings.forceScale;

            _lastForce = scaledForce;
            _lastTorque = scaledTorque;
            
            rb.AddForce(scaledForce, ForceMode.Force);
            rb.AddTorque(scaledTorque, ForceMode.Force);

            if (logForces) Debug.Log($"Buoyancy: {scaledForce.y:F0}N, SubTris: {result.SubmergedTriangleCount}");

            var submersion = (float)result.SubmergedTriangleCount / Mathf.Max(1, _worldHullTriangles.Length);
            ApplyDamping(submersion);

            SubmersionRatio = submersion;
            TotalBuoyancy = scaledForce.y;
        }

        // IBuoyant
        public float TotalBuoyancy { get; private set; }
        public float SubmersionRatio { get; private set; }
        public bool IsFloating => SubmersionRatio > 0f;

        // Transform hull to world spaces
        private void TransformHullToWorld()
        {
            var localToWorld = hullMeshFilter.transform.localToWorldMatrix;
            for (var i = 0; i < _localHullTriangles.Length; i++)
                _worldHullTriangles[i] = new HullTriangle(
                    localToWorld.MultiplyPoint3x4(_localHullTriangles[i].v0),
                    localToWorld.MultiplyPoint3x4(_localHullTriangles[i].v1),
                    localToWorld.MultiplyPoint3x4(_localHullTriangles[i].v2)
                );
        }

        private void BuildHullTriangles()
        {
            if (hullMeshFilter == null)
            {
                Debug.LogError($"No hull mesh on {gameObject.name}");
                _localHullTriangles = new HullTriangle[0];
                _worldHullTriangles = new HullTriangle[0];
                return;
            }

            var mesh = hullMeshFilter.sharedMesh;
            var vertices = mesh.vertices;
            var triangles = mesh.triangles;
            var triCount = triangles.Length / 3;

            if (triCount > warnTriCount)
                Debug.LogWarning($"{gameObject.name}: Hull mesh has {triCount} triangles - use a simplified mesh");

            _localHullTriangles = new HullTriangle[triCount];
            _worldHullTriangles = new HullTriangle[triCount];

            for (var i = 0; i < triCount; i++)
            {
                var triIndex = i * 3;
                if (flipNormals)
                    // Swap V1 and V2 to flip normal direction
                    _localHullTriangles[i] = new HullTriangle(
                        vertices[triangles[triIndex]],
                        vertices[triangles[triIndex + 2]],
                        vertices[triangles[triIndex + 1]]
                    );
                else
                    _localHullTriangles[i] = new HullTriangle(
                        vertices[triangles[triIndex]],
                        vertices[triangles[triIndex + 1]],
                        vertices[triangles[triIndex + 2]]
                    );
            }

            Debug.Log($"Built hull with {triCount} triangles");
        }

        private void ApplyDamping(float submersionRatio)
        {
            var dt = Time.fixedDeltaTime;

            var linearDampForce = -rb.linearVelocity * (settings.linearDamping * submersionRatio * dt);
            rb.AddForce(linearDampForce, ForceMode.VelocityChange);

            var angularDampTorque = -rb.angularVelocity * (settings.angularDamping * submersionRatio * dt);
            rb.AddTorque(angularDampTorque, ForceMode.VelocityChange);
        }


        #region Debug

        private Vector3 _lastForce;
        private Vector3 _lastTorque;

        private void OnDrawGizmos()
        {
            if (settings == null) return;

            // Draw force arrow
            if (rb != null && _lastForce.sqrMagnitude > 0.01f)
            {
                Gizmos.color = Color.green;
                var forceDir = _lastForce.normalized;
                var forceScale = Mathf.Log10(_lastForce.magnitude + 1) * 0.5f;
                Gizmos.DrawRay(rb.worldCenterOfMass, forceDir * forceScale);
            }

            if (!settings.showSubmergedTriangles) return;
            if (_calculator == null) return;

            Gizmos.color = settings.submergedColor;
            foreach (var tri in _calculator.SubmergedTriangles)
            {
                Gizmos.DrawLine(tri.v0, tri.v1);
                Gizmos.DrawLine(tri.v1, tri.v2);
                Gizmos.DrawLine(tri.v2, tri.v0);
            }
        }

        #endregion
    }
}