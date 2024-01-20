using Core.Features.Shared.Parameters;

namespace Core.Features.GetServer
{
    public class ServerParameters : IPaginationParameters
    {
        public int PageNumber { get; set; } = DefaultParameters.PageNumber;

        public int PageSize { get; set; } = DefaultParameters.PageSize;

        public string Key => $"{PageNumber}_{PageSize}";
    }
}