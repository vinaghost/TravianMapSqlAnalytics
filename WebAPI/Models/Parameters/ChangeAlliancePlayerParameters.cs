namespace WebAPI.Models.Parameters
{
    public class ChangeAlliancePlayerParameters : PlayerParameters
    {
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    }
}