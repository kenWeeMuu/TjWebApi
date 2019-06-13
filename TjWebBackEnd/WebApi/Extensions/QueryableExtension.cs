/******************************************
 * AUTHOR:          Rector
 * CREATEDON:       2018-09-26
 * OFFICIAL_SITE:    码友网(https://codedefault.com)--专注.NET/.NET Core
 * 版权所有，请勿删除
 ******************************************/

using System;
using System.Linq;
using System.Linq.Expressions;

namespace WebApi.Extensions
{
    /// <summary>
    ///
    /// </summary>
    public static class QueryableExtension
    {
        /// <summary>
        /// IQueryable分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IQueryable<T> Paged<T>(this IQueryable<T> query, int currentPage = 1, int pageSize = 20) {
            if (currentPage < 1) {
                currentPage = 1;
            }
            query = query.Skip((currentPage - 1) * pageSize).Take(pageSize);
            return query;
        }

        public static IQueryable<T> Contains<T>(this IQueryable<T> source, string propertyName, string value)
        {
            ParameterExpression pe = Expression.Parameter(typeof(T), "c");
            var ee1 = Expression.Property(pe, propertyName);
            var ee2 = Expression.Constant(value);
            var body = Expression.Call(ee1, "Contains", null, new Expression[] { ee2 });
            var expression = Expression.Lambda<Func<T, bool>>(body, pe);


            return source.Where(expression);
        }


    }
}