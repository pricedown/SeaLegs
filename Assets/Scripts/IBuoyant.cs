namespace SeaLegs
{
    public interface IBuoyant
    {
        float TotalBuoyancy { get; }
        float SubmersionRatio { get; }
        bool IsFloating { get; }
    }
}