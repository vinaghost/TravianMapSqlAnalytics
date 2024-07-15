namespace WebMVC.ViewModels
{
    public class BaseViewModel<TParameters, TData>
    {
        public required TParameters Parameters { get; set; }
        public required TData? Data { get; set; }
    }
}