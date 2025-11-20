using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class CompositeFloater : MonoBehaviour
{
    [SerializeField] public List<Transform> floaters = new();
    [SerializeField] protected Rigidbody _rb;
    [SerializeField] protected IWater water;

    [Header("Floater Properties")]
    public float submersionDepth = 1;
    public float buoyancyCoefficient = 1;
    public float linearDampingCoefficient = 1;
    public float angularDampingCoefficient = 5;

    [Header("Debugging")]
    [SerializeField] protected float gizmoSize = 0.3f;

    private void Awake()
    {
        InitRigidbody();
    }

    private void FixedUpdate()
    {
        foreach (Transform t in floaters)
            UpdateFloater(t.position, Time.fixedDeltaTime);
    }

    private void InitRigidbody()
    {
        _rb.useGravity = false;
    }

    protected virtual void UpdateFloater(Vector3 floaterPosition, float dt)
    {
        var waterHeight = water.GetWaterHeightAt(floaterPosition);
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


    private bool IsSubmerged(Vector3 position, float waterHeight)
    {
        return transform.position.y < waterHeight;
    }

    private float GetSubmersionRatio(Vector3 position, float waterHeight)
    {
        return Mathf.Clamp01((waterHeight - position.y) / submersionDepth * buoyancyCoefficient);
    }

    private void OnDrawGizmos()
    {
        foreach (Transform f in floaters)
        {
            Gizmos.color = IsSubmerged(f.position, water.GetWaterHeightAt(f.position)) ? Color.green : Color.red;
            Gizmos.DrawSphere(f.position, gizmoSize);
        }
    }

}
