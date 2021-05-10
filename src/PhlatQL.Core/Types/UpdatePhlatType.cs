using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Serialization;
using PhlatQL.Core.Validation;

namespace PhlatQL.Core.Types
{
    public abstract class UpdatePhlatType<T> : JsonPatchDocument<T>, IPhlatType where T : class
    {
        private readonly Dictionary<string, (LambdaExpression, List<ValidationRule>)> validationFields = new();

        protected UpdatePhlatType(List<Operation<T>> operations, IContractResolver contractResolver) : base(operations, contractResolver)
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
            List<string> fields = this.ToFields();

            ErrorResponse errorsResponse = new();
            foreach (string field in fields)
            {
                if (!this.validationFields.ContainsKey(field))
                {
                    continue;
                }

                (LambdaExpression expression, List<ValidationRule> validationRules) = this.validationFields[field];

                MutationPhlatType<T>.AddValidationErrors(field, context, expression, validationRules, errorsResponse);
            }

            return errorsResponse;
        }

        protected abstract void Configure();
    }
}
