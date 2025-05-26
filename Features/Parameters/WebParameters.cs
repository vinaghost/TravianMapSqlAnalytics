namespace Features.Parameters
{
    public abstract record WebParameters
    {
        public bool IsUserInput { get; set; } = false;
    }
}