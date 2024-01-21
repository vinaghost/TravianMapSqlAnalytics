namespace Core.Features.Shared.Parameters
{
    public interface IDistanceFilterParameters
    {
        public int TargetX { get; }
        public int TargetY { get; }

        public int MinDistance { get; }
        public int MaxDistance { get; }
    }
}