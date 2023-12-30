namespace WebAPI.Models.Parameters
{
    public class ChangeAlliancePlayerParameters : PlayerParameters, IChangeParameters
    {
        public DateOnly Date { get; } = DefaultParameters.Date;
    }
}