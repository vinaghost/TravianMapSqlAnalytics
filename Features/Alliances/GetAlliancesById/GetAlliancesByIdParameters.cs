namespace Features.Alliances.GetAlliancesById
{
    public record GetAlliancesByIdParameters(IList<int> Ids);

    public static class GetAlliancesByIdParametersExtension
    {
        public static string Key(this GetAlliancesByIdParameters parameters)
        {
            return string.Join(',', parameters.Ids.Distinct().Order());
        }
    }
}