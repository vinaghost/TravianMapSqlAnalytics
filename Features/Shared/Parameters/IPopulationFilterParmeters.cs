namespace Features.Shared.Parameters
{
    public interface IPopulationFilterParmeters
    {
        IList<int>? Ids { get; }

        int Days { get; }
    }
}