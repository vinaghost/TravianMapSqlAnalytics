namespace MapSqlAspNetCoreMVC.Models.Input
{
    public class HomeInput : IPagingInput
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}