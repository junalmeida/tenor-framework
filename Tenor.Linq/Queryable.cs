using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Tenor.Linq
{
    public static class Queryable
    {
        public static IQueryable<TSource> LoadAlso<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (keySelector == null)
                throw new ArgumentNullException("keySelector");
            return (IQueryable<TSource>)source.Provider.CreateQuery<TSource>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource), typeof(TKey) }), new Expression[] { source.Expression, Expression.Quote(keySelector) }));
        }

        public static IQueryable<TSource> LoadAlso<TSource>(this IQueryable<TSource> source, string keySelector)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (keySelector == null)
                throw new ArgumentNullException("keySelector");
            return (IQueryable<TSource>)source.Provider.CreateQuery<TSource>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }), new Expression[] { source.Expression, Expression.Constant(keySelector, typeof(string)) }));
        }



    }
}
