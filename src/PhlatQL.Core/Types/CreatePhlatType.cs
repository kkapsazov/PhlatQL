using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Serialization;
using PhlatQL.Core.Validation;

namespace PhlatQL.Core.Types
{
    public abstract class CreatePhlatType<T> : JsonPatchDocument<T>, IPhlatType where T : class
    {
        private readonly Dictionary<string, (LambdaExpression, List<ValidationRule>)> validationFields = new();

        protected CreatePhlatType(List<Operation<T>> operations, IContractResolver contractResolver) : base(operations, contractResolver)
        {
        }

        protected void Field<TReturnType>(Expression<Func<T, TReturnType>> expression, List<ValidationRule> rules)
        {
            string name = expression.NameOf().ToLowerFirst();
            this.validationFields.Add(name, (expression, rules));
        }

        public ErrorResponse Validate(T context)
        {
            this.Configure();

            ErrorResponse errorsResponse = new();
            foreach (KeyValuePair<string, (LambdaExpression, List<ValidationRule>)> kv in this.validationFields)
            {
                string field = kv.Key;
                (LambdaExpression expression, List<ValidationRule> validationRules) = kv.Value;

                MutationPhlatType<T>.AddValidationErrors(field, context, expression, validationRules, errorsResponse);
            }

            return errorsResponse;
        }

        protected abstract void Configure();
    }
}
