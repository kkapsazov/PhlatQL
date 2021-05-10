using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using PhlatQL.Core.Validation;

namespace PhlatQL.Core.Types
{
    public static class MutationPhlatType<T>
    {
        public static void AddValidationErrors(string field, T context, LambdaExpression expression, List<ValidationRule> validationRules, ErrorResponse errorsResponse)
        {
            Type type = expression.Body.Type;
            Delegate lambda = expression.Compile();
            object? value = lambda.DynamicInvoke(context);
            ConstantExpression valueExp = Expression.Constant(value);

            foreach (ValidationRule rule in validationRules)
            {
                switch (rule.Type)
                {
                    case RuleType.MaxValue:
                    case RuleType.MinValue:
                    case RuleType.StrictMaxValue:
                    case RuleType.StrictMinValue:
                    {
                        ExpressionType expType = rule.Type switch
                        {
                            RuleType.MaxValue => ExpressionType.LessThanOrEqual,
                            RuleType.MinValue => ExpressionType.GreaterThanOrEqual,
                            RuleType.StrictMaxValue => ExpressionType.LessThan,
                            RuleType.StrictMinValue => ExpressionType.GreaterThan
                        };

                        string code = rule.Type switch
                        {
                            RuleType.MaxValue => ActionErrorCodes.FIELD_MAX_VALUE,
                            RuleType.MinValue => ActionErrorCodes.FIELD_MIN_VALUE,
                            RuleType.StrictMaxValue => ActionErrorCodes.FIELD_STRICT_MAX_VALUE,
                            RuleType.StrictMinValue => ActionErrorCodes.FIELD_STRICT_MIN_VALUE
                        };

                        ConstantExpression ruleExp = Expression.Constant(Convert.ChangeType(rule.Value, type));
                        BinaryExpression binary = Expression.MakeBinary(expType, valueExp, ruleExp);
                        bool result = Expression.Lambda<Func<bool>>(binary).Compile().Invoke();
                        if (!result)
                        {
                            errorsResponse.AddFieldError(field, code, new List<string>
                            {
                                field,
                                rule.Value
                            });
                        }

                        break;
                    }
                    case RuleType.MinLength:
                    case RuleType.MaxLength:
                    {
                        if (value is null)
                        {
                            break;
                        }

                        string code = rule.Type switch
                        {
                            RuleType.MinLength => ActionErrorCodes.FIELD_MIN_LENGTH,
                            RuleType.MaxLength => ActionErrorCodes.FIELD_MAX_LENGTH
                        };

                        bool hasError = rule.Type switch
                        {
                            RuleType.MinLength => value.ToString().Length < int.Parse(rule.Value),
                            RuleType.MaxLength => value.ToString().Length > int.Parse(rule.Value)
                        };

                        if (hasError)
                        {
                            errorsResponse.AddFieldError(field, code, new List<string>
                            {
                                field,
                                rule.Value
                            });
                        }

                        break;
                    }
                    case RuleType.Required:
                    {
                        if (value is null)
                        {
                            errorsResponse.AddFieldError(field, ActionErrorCodes.FIELD_REQUIRED, new List<string>
                            {
                                field,
                                rule.Value
                            });
                        }

                        break;
                    }
                    case RuleType.Regex:
                    {
                        if (value is null)
                        {
                            break;
                        }

                        Match match = Regex.Match(value.ToString(), rule.Value);
                        if (!match.Success)
                        {
                            errorsResponse.AddFieldError(field, ActionErrorCodes.FIELD_REGEX, new List<string>
                            {
                                field,
                                rule.Value
                            });
                        }

                        break;
                    }
                }
            }
        }
    }
}
