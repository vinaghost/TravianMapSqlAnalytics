namespace WebAPI.Models.Parameters
{
    public interface IDistanceFilterParameter
    {
        public int TargetX { get; }
        public int TargetY { get; }

        public int MinDistance { get; }
        public int MaxDistance { get; }
    }
}