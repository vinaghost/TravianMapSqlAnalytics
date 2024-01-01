namespace WebAPI.Models.Parameters
{
    public interface IChangeAllianceFilterParameter : IChangeParameters
    {
        int MinChangeAlliance { get; }
        int MaxChangeAlliance { get; }
    }
}