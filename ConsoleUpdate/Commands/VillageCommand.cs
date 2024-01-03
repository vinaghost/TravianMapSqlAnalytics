using ConsoleUpdate.Models;

namespace ConsoleUpdate.Commands
{
    public class VillageCommand : ServerCommand
    {
        public List<VillageRaw> VillageRaws { get; }

        public VillageCommand(string serverUrl, List<VillageRaw> villageRaws) : base(serverUrl)
        {
            VillageRaws = villageRaws;
        }
    }
}