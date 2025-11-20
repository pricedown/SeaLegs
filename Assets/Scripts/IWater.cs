using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public interface IWater
{
    public abstract float GetWaterHeightAt(Vector3 position);
}
