using UnityEngine;

namespace SeaLegs
{
    /// <summary>
    ///     Hull-based buoyancy using hydrostatic pressure on submerged triangles.
    ///     Alternative to the floater-point based BuoyancyBody.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class HullBuoyancyBody : MonoBehaviour, IBuoyant
    {
        [Header("References")] [SerializeField]
        private Rigidbody rb;

        [SerializeField] private WaterBase _water;

        [Header("Hull")] 
        [SerializeField] private MeshFilter hullMeshFilter;
        [Range(0.001f, 10f)]
        [SerializeField] private float forceScale = 1f;

        [SerializeField] private float waterDensity = 1025f;

        [Header("Damping")] [SerializeField] private float linearDamping = 1f;
        
        [Header("Debug")]
        [SerializeField] private bool logForces = false;

        [SerializeField] private float angularDamping = 5f;
        private HydrostaticCalculator _calculator;
        private HullTriangle[] _localHullTriangles;
        private HullTriangle[] _worldHullTriangles;

        private void Awake()
        {
            // Disable gravity so we can apply our own
            rb.useGravity = false;

            // Extract triangles from hull mesh
            BuildHullTriangles();

            _calculator = new HydrostaticCalculator();
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
                waterDensity
            );
            


            rb.AddForce(Physics.gravity, ForceMode.Acceleration);
            
            // rb.AddForce(result.Force, ForceMode.Force);
            // rb.AddTorque(result.Torque, ForceMode.Force);
            Vector3 scaledForce = result.Force * forceScale;
            Vector3 scaledTorque = result.Torque * forceScale;
                        
            rb.AddForce(scaledForce, ForceMode.Force);
            rb.AddTorque(scaledTorque, ForceMode.Force);
            
            if (logForces)
            {
                Debug.Log($"Buoyancy: {scaledForce.y:F0}N, SubTris: {result.SubmergedTriangleCount}");
            }
            
            var submersion = (float)result.SubmergedTriangleCount / Mathf.Max(1, _worldHullTriangles.Length);
            ApplyDamping(submersion);

            SubmersionRatio = submersion;
            TotalBuoyancy = scaledForce.y;
        }

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
                return;
            }

            var mesh = hullMeshFilter.sharedMesh;
            var vertices = mesh.vertices;
            var triangles = mesh.triangles;
            var triCount = triangles.Length / 3;

            _localHullTriangles = new HullTriangle[triCount];
            _worldHullTriangles = new HullTriangle[triCount];

            for (var i = 0; i < triCount; i++)
            {
                var triIndex = i * 3;
                _localHullTriangles[i] = new HullTriangle(
                    vertices[triangles[triIndex]],
                    vertices[triangles[triIndex + 1]],
                    vertices[triangles[triIndex + 2]]
                );
            }
        }

        private void ApplyDamping(float submersionRatio)
        {
            var dt = Time.fixedDeltaTime;

            var linearDampForce = -rb.linearVelocity * (linearDamping * submersionRatio * dt);
            rb.AddForce(linearDampForce, ForceMode.VelocityChange);

            var angularDampTorque = -rb.angularVelocity * (angularDamping * submersionRatio * dt);
            rb.AddTorque(angularDampTorque, ForceMode.VelocityChange);
        }
        
        
        #region Debug
        private void OnDrawGizmos()
        {
            if (_calculator == null) return;
            
            Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
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