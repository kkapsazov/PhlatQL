using System;
using System.Linq.Expressions;

namespace PhlatQL.Core
{
    public static class Extensions
    {
        public static string NameOf<TSourceType, TProperty>(this Expression<Func<TSourceType, TProperty>> expression)
        {
            MemberExpression member = (MemberExpression)expression.Body;
            return member.Member.Name;
        }

        public static string ToLowerFirst(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            return char.ToLower(str[0]) + str.Substring(1);
        }
    }
}
