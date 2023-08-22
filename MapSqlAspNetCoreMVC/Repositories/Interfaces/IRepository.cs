namespace MapSqlAspNetCoreMVC.Repositories.Interfaces
{
    public interface IRepository<TInput, TOutput>
    {
        Task<TOutput> Get(TInput input);
    }
}