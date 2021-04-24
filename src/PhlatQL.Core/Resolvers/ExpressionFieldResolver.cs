using System;
using System.Linq.Expressions;

namespace PhlatQL.Core.Resolvers
{
    public class ExpressionFieldResolver<TSourceType, TProperty> : IFieldResolver
    {
        private readonly Func<TSourceType, TProperty> property;

        public ExpressionFieldResolver(Expression<Func<TSourceType, TProperty>> property)
        {
            this.property = property.Compile();
        }

        public object Resolve(IResolveFieldContext context)
        {
            return this.property((TSourceType)context.Source);
        }
    }
}
