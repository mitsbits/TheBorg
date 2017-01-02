using Borg.Infra.Relational;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Borg
{
    public static class RelationalExtensions
    {
        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string orderByProperty, bool desc) where TEntity : class
        {
            var command = desc ? "OrderByDescending" : "OrderBy";

            var type = typeof(TEntity);

            var property = type.GetProperty(orderByProperty);

            var parameter = Expression.Parameter(type, "p");

            var propertyAccess = Expression.MakeMemberAccess(parameter, property);

            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },

                                   source.Expression, Expression.Quote(orderByExpression));

            return source.Provider.CreateQuery<TEntity>(resultExpression);
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "OrderBy");
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "OrderByDescending");
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "ThenBy");
        }

        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "ThenByDescending");
        }

        private static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            string[] props = property.Split('.');
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                PropertyInfo pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            object result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), type)
                    .Invoke(null, new object[] { source, lambda });
            return (IOrderedQueryable<T>)result;
        }

        public static OrderByInfo<T> GetSorter<T>(this Expression<Func<T, dynamic>> property, bool descending = false) where T : class
        {
            return new OrderByInfo<T>() { Ascending = !descending, Property = property };
        }

        public static IOrderedQueryable<T> Apply<T>(this IQueryable<T> source, params OrderByInfo<T>[] orderBys)
            where T : class
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (orderBys == null || !orderBys.Any()) return (IOrderedQueryable<T>)source;

            IOrderedQueryable<T> orderedQueryable = null;
            var orderByInfos = orderBys;
            for (var i = 0; i < orderByInfos.Count(); i++)
            {
                var info = orderByInfos[i];
                string name = info.TruePropertyName;
                if (i == 0)
                {
                    orderedQueryable = info.Ascending ? source.OrderBy(name) : source.OrderByDescending(name);
                }
                else
                {
                    if (orderedQueryable != null)
                    {
                        orderedQueryable = info.Ascending ? orderedQueryable.ThenBy(name) : orderedQueryable.ThenByDescending(name);
                    }
                }
            }
            return orderedQueryable;
        }
    }
}