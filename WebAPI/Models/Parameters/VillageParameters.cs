namespace WebAPI.Models.Parameters
{
    public class VillageParameters : QueryStringParameters
    {
        public int MinPopulation { get; set; } = 0;
        public int MaxPopulation { get; set; } = 10000;
        public List<int> Players { get; set; } = [];
        public List<int> Alliances { get; set; } = [];
    }
}