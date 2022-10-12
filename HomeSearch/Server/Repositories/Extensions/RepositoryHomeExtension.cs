using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using HomeSearch.Server.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
// using System.Linq.Dynamic.Core;

namespace HomeSearch.Server.Repositories.Extensions
{
	public static class HomeSearchExtension
	{

        /// <summary>
        /// Orders the by.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="direction">The direction.</param>
        /// <returns></returns>
        public static IOrderedMongoQueryable<T> OrderBy<T>(this IMongoQueryable<T> source, string propertyName, OrderDirection direction)
        {
            switch (direction)
            {
                case OrderDirection.ASC:
                    return source.OrderBy(ToLambda<T>(propertyName));

                case OrderDirection.DESC:
                    return source.OrderByDescending(ToLambda<T>(propertyName));

                default:
                    return source.OrderBy(ToLambda<T>(propertyName));
            }

        }

        /// <summary>
        /// Orders the by.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="direction">The direction.</param>
        /// <returns></returns>
        public static IOrderedMongoQueryable<T> OrderBy<T>(this IMongoQueryable<T> source, string propertyName, int direction)
        {
            var dir = (OrderDirection)direction;

            switch (dir)
            {
                case OrderDirection.ASC:
                    return source.OrderBy(ToLambda<T>(propertyName));

                case OrderDirection.DESC:
                    return source.OrderByDescending(ToLambda<T>(propertyName));

                default:
                    return source.OrderBy(ToLambda<T>(propertyName));
            }

        }


        /// <summary>
        /// Orders the by.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static IOrderedMongoQueryable<T> OrderBy<T>(this IMongoQueryable<T> source, string propertyName)
        {
            return source.OrderBy(ToLambda<T>(propertyName));
        }

        /// <summary>
        /// Orders the by descending.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static IOrderedMongoQueryable<T> OrderByDescending<T>(this IMongoQueryable<T> source, string propertyName)
        {
            return source.OrderByDescending(ToLambda<T>(propertyName));
        }

        /// <summary>
        /// Wheres the specified property name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="stringToSource">The string to source.</param>
        /// <returns></returns>
        public static IMongoQueryable<T> Where<T>(this IMongoQueryable<T> source, string propertyName, string stringToSource)
        {
            return source.Where(ToLambdaWhere<T>(propertyName, stringToSource));
        }


        /// <summary>
        /// Wheres the text.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <param name="field">The field.</param>
        /// <param name="search">The search.</param>
        /// <returns></returns>
        public static IMongoQueryable<T> WhereText<T>(this IMongoQueryable<T> query, Expression<Func<T, object>> field, string search)
        {

            search = Regex.Escape(search);
            var filter = Builders<T>.Filter.Text(search);
            var member = HelpersLinq.PropertyName<T>(field);

            var regexFilter = string.Format("^{0}.*", search);
            var filterRegex = Builders<T>.Filter.Regex(member, BsonRegularExpression.Create(new Regex(regexFilter, RegexOptions.IgnoreCase)));

            return query.Where(_ => filter.Inject() && filterRegex.Inject());
        }

        /// <summary>
        /// Converts to lambda.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        private static Expression<Func<T, object>> ToLambda<T>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propertyName);
            var propAsObject = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(propAsObject, parameter);
        }

        /// <summary>
        /// Converts to lambdawhere.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="stringToSearch">The string to search.</param>
        /// <returns></returns>
        private static Expression<Func<T, bool>> ToLambdaWhere<T>(string propertyName, string stringToSearch)
        {
            //var parameter = Expression.Parameter(typeof(T));

            // Create a parameter to use for both of the expression bodies.
            var parameter = Expression.Parameter(typeof(T), "x");
            // Invoke each expression with the new parameter, and combine the expression bodies with OR.
            var predicate = Expression.Lambda<Func<T, bool>>(Expression.Call(Expression.PropertyOrField(parameter, propertyName), "Contains", null, Expression.Constant(stringToSearch)), parameter);

            return predicate;
        }


        public static class HelpersLinq
        {
            public static string PropertyName<T>(Expression<Func<T, object>> expression)
            {
                var member = expression.Body as MemberExpression;
                if (member != null && member.Member is PropertyInfo)
                    return member.Member.Name;

                throw new ArgumentException("Expression is not a Property", "expression");
            }
        }

        public enum OrderDirection
        {
            ASC = 1,
            DESC
        }

        public static IQueryable<Home> Search(this IQueryable<Home> homes, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return homes;

            var lowerCaseSearchTerm = searchTerm.Trim().ToLower();

            return homes.Where(p => p.Name.ToLower().Contains(lowerCaseSearchTerm));
        }

        public static IMongoQueryable<Home> Search(this IMongoQueryable<Home> homes, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return homes;

            var lowerCaseSearchTerm = searchTerm.Trim().ToLower();

            return homes.Where(p => p.Name.ToLower().Contains(lowerCaseSearchTerm));
        }

        public static IOrderedMongoQueryable<Home> Sort(this IMongoQueryable<Home> homes, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return homes.OrderBy(e => e.Name);

            var orderParams = orderByQueryString.Trim();
            var propertyInfos = typeof(Home).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var objectProperty = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(orderParams, StringComparison.InvariantCultureIgnoreCase));

            if (string.IsNullOrWhiteSpace(objectProperty.Name))
                return homes.OrderBy(e => e.Name);

            var orderQuery = objectProperty.Name.TrimEnd(',', ' ');
            return homes.OrderBy(orderQuery);
        }

        public static IQueryable<Home> SearchByTags(this IQueryable<Home> homes, string[] searchTerm)
        {
            if (searchTerm == null)
                return homes;

            return homes.Where(p => searchTerm.All(tag => p.Tags.Contains(tag.ToLower())));
        }

    }
}

