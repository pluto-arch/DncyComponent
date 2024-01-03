using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Dotnetydd.Specifications.EntityFrameworkCore.Extensions
{
    public static class SearchExtension
    {
        /// <summary>
        ///     Filters <paramref name="source" /> by applying an 'SQL LIKE' operation to it.
        /// </summary>
        /// <typeparam name="T">The type being queried against.</typeparam>
        /// <param name="source">The sequence of <typeparamref name="T" /></param>
        /// <param name="criterias">
        ///     <list type="bullet">
        ///         <item>Selector, the property to apply the SQL LIKE against.</item>
        ///         <item>SearchTerm, the value to use for the SQL LIKE.</item>
        ///     </list>
        /// </param>
        /// <returns></returns>
        public static IQueryable<T> Search<T>(this IQueryable<T> source,
            IEnumerable<(Expression<Func<T, string>> selector, string searchTerm)> criterias)
        {
            Expression expr = null;

            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");

            foreach ((Expression<Func<T, string>> selector, string searchTerm) in criterias)
            {
                if (selector == null || string.IsNullOrEmpty(searchTerm))
                {
                    continue;
                }

                PropertyInfo propertyInfo = typeof(EF).GetProperty(nameof(EF.Functions));

                if (propertyInfo is null)
                {
                    continue;
                }

                MemberExpression functions = Expression.Property(null, propertyInfo);

                MethodInfo like = typeof(DbFunctionsExtensions).GetMethod(nameof(DbFunctionsExtensions.Like),
                    new[] { functions.Type, typeof(string), typeof(string) });

                if (like is null)
                {
                    continue;
                }

                Expression propertySelector =
                    ParameterReplacerVisitor.Replace(selector, selector.Parameters[0], parameter);

                Expression expression = (propertySelector as LambdaExpression)?.Body;

                if (expression is null)
                {
                    continue;
                }

                MethodCallExpression likeExpression =
                    Expression.Call(null, like, functions, expression, Expression.Constant(searchTerm));

#if !NETCOREAPP3_1
                expr = expr == null ? likeExpression : Expression.OrElse(expr, likeExpression);
#else
                expr = expr == null ? likeExpression : (Expression)Expression.OrElse(expr, likeExpression);
#endif

            }

            return expr == null ? source : source.Where(Expression.Lambda<Func<T, bool>>(expr, parameter));
        }
    }
}

