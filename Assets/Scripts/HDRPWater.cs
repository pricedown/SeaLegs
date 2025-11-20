using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class HDRPWater : MonoBehaviour, IWater
{
    public WaterSurface waterSurface;

    private WaterSearchParameters _searchParams;

    float IWater.GetWaterHeightAt(Vector3 position)
    {
        WaterSearchResult _waterSurfacePoint;
        _searchParams.startPositionWS = position;
        waterSurface.ProjectPointOnWaterSurface(_searchParams, out _waterSurfacePoint);
        return _waterSurfacePoint.projectedPositionWS.y;
    }
}
