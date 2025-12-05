using SeaLegs;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class HdrpWater : MonoBehaviour, IWater
{
    public WaterSurface waterSurface;

    private WaterSearchParameters _searchParams;

    Vector3 IWater.GetWaveDisplacementAt(Vector3 position)
    {
        WaterSearchResult _waterSurfacePoint;
        _searchParams.startPositionWS = position;
        waterSurface.ProjectPointOnWaterSurface(_searchParams, out _waterSurfacePoint);
        return new Vector3(0, _waterSurfacePoint.projectedPositionWS.y, 0);
    }
}
