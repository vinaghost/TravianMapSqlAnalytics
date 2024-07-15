namespace Features.GetAllianceData
{
    public record AllianceDataDto(AllianceDto Alliance, IList<PlayerDto> Players);
}