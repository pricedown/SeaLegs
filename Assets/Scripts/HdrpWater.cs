using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SeaLegs
{
    public class HdrpWater : MonoBehaviour, IWater
    {
        [SerializeField] private WaterSurface waterSurface;

        private WaterSearchParameters _searchParams;
        private WaterSearchResult _searchResult;

        private void Awake()
        {
            if (waterSurface == null)
                waterSurface = FindFirstObjectByType<WaterSurface>();

            if (waterSurface == null)
                Debug.LogError("No watersurface found");
        }

        public Vector3 GetWaveDisplacementAt(Vector3 position)
        {
            _searchParams.startPositionWS = position;
            waterSurface.ProjectPointOnWaterSurface(_searchParams, out _searchResult);
            return _searchResult.projectedPositionWS;
        }
    }
}