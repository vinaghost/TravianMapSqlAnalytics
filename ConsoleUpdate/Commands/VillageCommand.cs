using ConsoleUpdate.Models;

namespace ConsoleUpdate.Commands
{
    public record VillageCommand(string ServerUrl, List<VillageRaw> VillageRaws) : ServerCommand(ServerUrl);
}