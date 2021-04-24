namespace PhlatQL.Core.Resolvers
{
    public interface IFieldResolver
    {
        object Resolve(IResolveFieldContext context);
    }
}
