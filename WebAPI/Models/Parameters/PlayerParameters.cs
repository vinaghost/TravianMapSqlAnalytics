namespace WebAPI.Models.Parameters
{
    public class PlayerParameters : QueryStringParameters
    {
        public List<int> Alliances { get; set; } = [];
    }
}