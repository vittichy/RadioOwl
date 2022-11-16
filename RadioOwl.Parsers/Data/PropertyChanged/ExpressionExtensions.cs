using System;
using System.Linq.Expressions;

namespace RadioOwl.Parsers.Data.PropertyChanged
{
    public static class ExpressionExtensions
    {
        public static string PropertyName<TProperty>(this Expression<Func<TProperty>> projection)
        {
            var memberExpression = (MemberExpression)projection.Body;

            return memberExpression.Member.Name;
        }
    }

}
