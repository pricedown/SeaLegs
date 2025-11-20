using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Floater : MonoBehaviour
{
    private static Dictionary<Rigidbody, int> _floaterCounts = new Dictionary<Rigidbody, int>();

    [SerializeField] private Rigidbody _rb;
    public float submersionDepth = 1;
    public float buoyancyCoefficient = 1;

    public float linearDampingCoefficient = 1;
    public float angularDampingCoefficient = 5;
    public WaterSurface waterSurface;

    private WaterSearchParameters _searchParams;
    private WaterSearchResult _waterSurfacePoint;

    private void Awake()
    {
        waterSurface = FindFirstObjectByType<WaterSurface>();
    }

    private void OnEnable()
    {
        _floaterCounts.TryAdd(_rb, 0);
        _floaterCounts[_rb]++;
    }

    private void OnDisable()
    {
        _floaterCounts[_rb]--;
        if (_floaterCounts[_rb] == 0)
            _floaterCounts.Remove(_rb);
    }

    private bool IsSubmerged()
    {
        return transform.position.y < GetWaterHeightAt(transform.position);
    }

    private float GetWaterHeightAt(Vector3 position)
    {
        _searchParams.startPositionWS = position;
        waterSurface.ProjectPointOnWaterSurface(_searchParams, out _waterSurfacePoint);
        return _waterSurfacePoint.projectedPositionWS.y;
    }
    
    private float GetSubmersionRatio()
    {
        return Mathf.Clamp01(((GetWaterHeightAt(transform.position) - transform.position.y) / submersionDepth) * buoyancyCoefficient);
    }

    private void FixedUpdate()
    {
        // Apply gravity
        _rb.AddForceAtPosition(Physics.gravity / _floaterCounts[_rb],  transform.position, ForceMode.Acceleration);
        
        if (IsSubmerged())
        {
            // Buoyant force
            _rb.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * GetSubmersionRatio(), 0f),transform.position, ForceMode.Acceleration);
            
            // Damping linear and angular velocities 
            _rb.AddForce(-_rb.linearVelocity * (GetSubmersionRatio() * linearDampingCoefficient * Time.fixedDeltaTime), ForceMode.VelocityChange);
            _rb.AddTorque(-_rb.angularVelocity * (GetSubmersionRatio() * angularDampingCoefficient * Time.fixedDeltaTime), ForceMode.VelocityChange);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = IsSubmerged() ? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position, 0.3f);
    }
}