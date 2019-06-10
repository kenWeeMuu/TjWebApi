/******************************************
 * AUTHOR:          Rector
 * CREATEDON:       2018-09-26
 * OFFICIAL_SITE:    码友网(https://codedefault.com)--专注.NET/.NET Core
 * 版权所有，请勿删除
 ******************************************/

using System.Linq;

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

 
    }
}