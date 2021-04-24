using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PhlatQL.Core.Resolvers
{
    public class FuncFieldResolver<TSourceType, TResponseType> : IFieldResolver
    {
        private readonly Func<IResolveFieldContext, object> resolver;

        public FuncFieldResolver(Expression<Func<IResolveFieldContextT<TSourceType>, object>> expression)
        {
            this.resolver = this.GetResolverFor(expression.Compile());
        }

        private Func<IResolveFieldContext, object> GetResolverFor(Func<IResolveFieldContextT<TSourceType>, object> resolver)
        {
            return context =>
            {
                object resolved = resolver((IResolveFieldContextT<TSourceType>)context);
                if (resolved == null)
                {
                    return null;
                }

                IResolveFieldContext ctx = (IResolveFieldContext)Activator.CreateInstance(typeof(TResponseType));

                if (resolved.GetType().IsGenericType && resolved.GetType().IsAssignableTo(typeof(IEnumerable)))
                {
                    List<object> list = new();
                    foreach (object item in (IEnumerable)resolved)
                    {
                        ctx.Source = item;
                        list.Add(ctx.Build());
                    }

                    return list;
                }

                ctx.Source = resolved;
                return ctx.Build();
            };
        }

        public object Resolve(IResolveFieldContext context)
        {
            return this.resolver(context);
        }

        object IFieldResolver.Resolve(IResolveFieldContext context)
        {
            return this.Resolve(context);
        }
    }
}
