namespace Core.Features.Shared.Parameters
{
    public abstract record WebParameters
    {
        public bool IsUserInput { get; set; } = false;
    }
}