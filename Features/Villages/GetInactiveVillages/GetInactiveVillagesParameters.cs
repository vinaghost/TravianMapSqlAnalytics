using Features.Villages.GetVillages;

namespace Features.Villages.GetInactiveVillages
{
    public record GetInactiveVillagesParameters : GetVillagesParameters
    {
        public int InactiveDays { get; init; } = 3;
    }

    public static class GetInactiveVillagesParametersExtension
    {
        public static string Key(this GetInactiveVillagesParameters parameters)
        {
            return $"{parameters.KeyParent()}_{parameters.InactiveDays}";
        }
    }
}