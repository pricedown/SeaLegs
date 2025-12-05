using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Serialization;

public class Floater : MonoBehaviour
{
    private static readonly Dictionary<Rigidbody, int> RigidbodyFloaterCounts = new();

    [FormerlySerializedAs("_rb")] [SerializeField] private Rigidbody rb;
    public float submersionDepth = 1;
    public float buoyancyCoefficient = 1;
    public float linearDampingCoefficient = 1;
    public float angularDampingCoefficient = 5;
    public WaterSurface waterSurface;

    private WaterSearchParameters _searchParams;
    private WaterSearchResult _waterSurfacePoint;

    private void Awake()
    {
        if (rb == null)
            Debug.LogError($"Rb of {gameObject.name} is null");
        
        waterSurface = FindFirstObjectByType<WaterSurface>();
    }

    private void FixedUpdate()
    {
        var waterHeight = GetWaterHeightAt(transform.position);
        var submersionRatio = 0f;
        if (IsSubmerged(waterHeight)) submersionRatio = GetSubmersionRatio(waterHeight);
        // Apply gravity
        rb.AddForceAtPosition(Physics.gravity / RigidbodyFloaterCounts[rb], transform.position, ForceMode.Acceleration);
        if (submersionRatio > 0f)
        {
            // Buoyant force
            rb.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * submersionRatio, 0f),
                transform.position, ForceMode.Acceleration);
            // Damping linear and angular velocities
            var dt = Time.fixedDeltaTime;
            rb.AddForce(-rb.linearVelocity * (linearDampingCoefficient * submersionRatio * dt),
                ForceMode.VelocityChange);
            rb.AddTorque(-rb.angularVelocity * (angularDampingCoefficient * submersionRatio * dt),
                ForceMode.VelocityChange);
        }
    }

    private void OnEnable()
    {
        RigidbodyFloaterCounts.TryAdd(rb, 0);
        RigidbodyFloaterCounts[rb]++;
    }

    private void OnDisable()
    {
        RigidbodyFloaterCounts[rb]--;
        if (RigidbodyFloaterCounts[rb] == 0)
            RigidbodyFloaterCounts.Remove(rb);
    }

    private void OnDrawGizmos()
    {
        var waterHeight = GetWaterHeightAt(transform.position);
        Gizmos.color = IsSubmerged(waterHeight) ? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position, 0.3f);
    }

    private float GetWaterHeightAt(Vector3 position)
    {
        _searchParams.startPositionWS = position;
        waterSurface.ProjectPointOnWaterSurface(_searchParams, out _waterSurfacePoint);
        return _waterSurfacePoint.projectedPositionWS.y;
    }

    private bool IsSubmerged(float waterHeight)
    {
        return transform.position.y < waterHeight;
    }

    private float GetSubmersionRatio(float waterHeight)
    {
        return Mathf.Clamp01((waterHeight - transform.position.y) / submersionDepth * buoyancyCoefficient);
    }
}