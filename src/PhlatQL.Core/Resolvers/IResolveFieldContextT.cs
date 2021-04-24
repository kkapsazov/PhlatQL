namespace PhlatQL.Core.Resolvers
{
    public interface IResolveFieldContextT<TSource> : IResolveFieldContext
    {
        TSource Source { get; }
    }
}
