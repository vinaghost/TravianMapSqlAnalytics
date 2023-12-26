namespace WebAPI.Models.Parameters
{
    public class ChangePopulationVillageParameters : VillageParameters
    {
        public int MinChangePopulation { get; set; } = 0;
        public int MaxChangePopulation { get; set; } = 10000;

        private const int MAX_DAYS = 7;
        private int _days = 3;

        public int Days
        {
            get => _days;
            set => _days = (value > MAX_DAYS) ? MAX_DAYS : value;
        }
    }
}