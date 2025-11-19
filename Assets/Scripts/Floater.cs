// https://www.youtube.com/watch?v=vzqoLJmpUqU

using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Floater : MonoBehaviour
{
    public Rigidbody rb;
    public float submersionDepth = 1;
    public float buoyancyCoefficient = 1;
    public int floaterCount = 1;

    public float linearDampingCoefficient = 1;
    public float angularDampingCoefficient = 5;
    public WaterSurface waterSurface;

    private WaterSearchParameters _searchParams;
    private WaterSearchResult _waterSurfacePoint;

    private void Awake()
    {
        waterSurface = FindFirstObjectByType<WaterSurface>();
    }

    private void FixedUpdate()
    {
        rb.AddForceAtPosition(Physics.gravity / floaterCount,  transform.position, ForceMode.Acceleration);
        
        _searchParams.startPositionWS =  transform.position;
        waterSurface.ProjectPointOnWaterSurface(_searchParams, out _waterSurfacePoint);

        if (transform.position.y <  _waterSurfacePoint.projectedPositionWS.y)
        {
            float submersionRatio = Mathf.Clamp01(((_waterSurfacePoint.projectedPositionWS.y - transform.position.y) / submersionDepth) * buoyancyCoefficient) ;
            // Buoyant force
            rb.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * submersionRatio, 0f),transform.position, ForceMode.Acceleration);
            
            // Damping linear and angular velocities 
            rb.AddForce(-rb.linearVelocity * (submersionRatio * linearDampingCoefficient * Time.fixedDeltaTime), ForceMode.VelocityChange);
            rb.AddTorque(-rb.angularVelocity * (submersionRatio * angularDampingCoefficient * Time.fixedDeltaTime), ForceMode.VelocityChange);
        }
    }
}
