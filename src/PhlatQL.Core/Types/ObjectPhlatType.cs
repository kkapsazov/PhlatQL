using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PhlatQL.Core.Resolvers;

namespace PhlatQL.Core.Types
{
    public class ObjectPhlatType<TSourceType> : IResolveFieldContextT<TSourceType>, IPhlatType
    {
        private readonly Dictionary<string, IFieldResolver> fields = new();

        public void Field(Expression<Func<TSourceType, object>> expression)
        {
            string name = expression.NameOf().ToLowerFirst();
            this.fields.Add(name, new ExpressionFieldResolver<TSourceType, object>(expression));
        }

        public void Field<TResponseType>(Expression<Func<IResolveFieldContextT<TSourceType>, object>> expression) where TResponseType : IResolveFieldContext
        {
            string name = expression.NameOf().ToLowerFirst();
            this.fields.Add(name, new FuncFieldResolver<TSourceType, TResponseType>(expression));
        }

        public Dictionary<string, object> Build()
        {
            Dictionary<string, object> dict = new();

            foreach ((string key, IFieldResolver value) in this.fields)
            {
                dict.Add(key, value.Resolve(this));
            }

            return dict;
        }

        public Dictionary<string, object> Build(TSourceType context)
        {
            if (context is null)
            {
                return new Dictionary<string, object>();
            }

            this.Source = context;
            return this.Build();
        }

        public List<Dictionary<string, object>> Build(IEnumerable<TSourceType> context)
        {
            List<Dictionary<string, object>> list = new();
            if (context is null || !context.Any())
            {
                return list;
            }

            foreach (TSourceType ctx in context)
            {
                list.Add(this.Build(ctx));
            }

            return list;
        }

        public TSourceType Source { get; set; }

        object IResolveFieldContext.Source
        {
            get => this.Source;
            set => this.Source = (TSourceType)value;
        }
    }
}
