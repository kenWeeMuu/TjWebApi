using System;
using System.Linq;
using System.Linq.Expressions;

namespace WebApi.Extensions
{
    public static class QueryExtensions
    {
        public  static IQueryable<T> Contains<T>(this IQueryable<T> source, string propertyName,string value) {
            ParameterExpression pe = Expression.Parameter(typeof(T), "c");
            var ee1 = Expression.Property(pe, propertyName);
            var ee2 = Expression.Constant(value);
            var body = Expression.Call(ee1, "Contains", null, new Expression[] { ee2 });
            var expression = Expression.Lambda<Func<T, bool>>(body, pe);


            return source.Where(expression);
        }
    }
}
