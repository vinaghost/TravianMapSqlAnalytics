namespace MainCore.Models
{
    public class Alliance
    {
        public Alliance()
        {
        }

        public Alliance(VillageRaw village)
        {
            Id = village.AllyId;
            Name = village.AllyName;
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}