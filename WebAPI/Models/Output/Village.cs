namespace WebAPI.Models.Output
{
    public readonly struct Village
    {
        public required int VillageId { get; init; }

        //public required string AllianceName { get; init; }
        //public required string PlayerName { get; init; }
        public required string VillageName { get; init; }

        public required int X { get; init; }
        public required int Y { get; init; }
        public required int Population { get; init; }
        public required bool IsCapital { get; init; }
        public required int Tribe { get; init; }
    }
}