using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System;
using TY.SPIMS.Utilities;
using System.Collections.Generic;

namespace TY.SPIMS.Client.Helper
{
    public static class HelperExtensions
    {
        public static decimal ToDecimal(this string numberToParse)
        {
            decimal result = 0;
            if (!string.IsNullOrWhiteSpace(numberToParse))
                result = decimal.Parse(numberToParse);

            return result;
        }

        public static decimal ToDecimal(this object numberToParse)
        {
            return numberToParse.ToString().ToDecimal();
        }

        public static int ToInt(this object numberToParse)
        {
            return numberToParse.ToString().ToInt();
        }

        public static DateTime? ToDate(this object dateToParse)
        {
            DateTime? result = null;

            if (dateToParse != null)
            {
                DateTime newDate = new DateTime();
                if (DateTime.TryParse(dateToParse.ToString(), out newDate))
                    result = newDate;
            }

            return result;
        }

        public static int ToInt(this string numberToParse)
        {
            int result = 0;
            if(!string.IsNullOrWhiteSpace(numberToParse))
            {
                if (int.TryParse(numberToParse, out result))
                { }
            }

            return result;
        }

        public static decimal GetValue(this decimal? number)
        {
            return number.HasValue ? number.Value : 0;
        }

        public static DateTime GetValue(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value : DateTime.Today;
        }

        public static SortableBindingList<T> ToSortableBindingList<T>(this List<T> list) 
        {
            return new SortableBindingList<T>(list);
        }

        private static readonly MethodInfo OrderByMethod = typeof(Queryable).GetMethods()
            .Where(method => method.Name == "OrderBy")
            .Where(method => method.GetParameters().Length == 2)
            .Single();

        private static readonly MethodInfo OrderByDescendingMethod = typeof(Queryable).GetMethods()
            .Where(method => method.Name == "OrderByDescending")
            .Where(method => method.GetParameters().Length == 2)
            .Single();

        public static IQueryable<TSource> OrderByProperty<TSource>(this IQueryable<TSource> source, string propertyName)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TSource), "posting");
            Expression orderByProperty = Expression.Property(parameter, propertyName);

            LambdaExpression lambda = Expression.Lambda(orderByProperty, new[] { parameter });
            
            MethodInfo genericMethod = OrderByMethod.MakeGenericMethod(new[] { typeof(TSource), orderByProperty.Type });
            object ret = genericMethod.Invoke(null, new object[] { source, lambda });
            return (IQueryable<TSource>)ret;
        }

        public static IQueryable<TSource> OrderByPropertyDescending<TSource>(this IQueryable<TSource> source, string propertyName)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TSource), "posting");
            Expression orderByProperty = Expression.Property(parameter, propertyName);

            LambdaExpression lambda = Expression.Lambda(orderByProperty, new[] { parameter });

            MethodInfo genericMethod = OrderByDescendingMethod.MakeGenericMethod(new[] { typeof(TSource), orderByProperty.Type });
            object ret = genericMethod.Invoke(null, new object[] { source, lambda });
            return (IQueryable<TSource>)ret;
        }    
    }
}
