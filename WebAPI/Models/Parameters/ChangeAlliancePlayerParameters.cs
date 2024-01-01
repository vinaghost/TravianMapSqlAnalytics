namespace WebAPI.Models.Parameters
{
    public class ChangeAlliancePlayerParameters : PlayerParameters, IChangeAllianceFilterParameter
    {
        public DateOnly Date { get; set; } = DefaultParameters.Date;

        public int MinChangeAlliance { get; set; } = DefaultParameters.MinChangeAlliance;

        public int MaxChangeAlliance { get; set; } = DefaultParameters.MaxChangeAlliance;
    }
}