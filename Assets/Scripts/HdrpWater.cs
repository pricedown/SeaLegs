using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SeaLegs
{
    public class HdrpWater : WaterBase, IWater
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

        public override Vector3 GetWaveDisplacementAt(Vector3 position)
        {
            _searchParams.startPositionWS = position;
            waterSurface.ProjectPointOnWaterSurface(_searchParams, out _searchResult);
            return (Vector3)_searchResult.projectedPositionWS -  new Vector3(0, waterSurface.transform.position.y, 0);
        }
    }
}