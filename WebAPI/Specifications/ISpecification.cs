namespace WebAPI.Specifications
{
    public interface ISpecification<T>
    {
        IQueryable<T> Apply(IQueryable<T> query);
    }

    public interface ISpecification<TIn, TOut>
    {
        IQueryable<TOut> Apply(IQueryable<TIn> query);
    }
}