namespace MapSqlAspNetCoreMVC.Models.Input
{
    public class InactiveFormInput : IPagingInput
    {
        public int Days { get; set; } = 3;
        public int Tribe { get; set; } = 0;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}