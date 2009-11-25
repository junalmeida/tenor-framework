using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Tenor.Linq
{
    public class QueryProvider : IQueryProvider
    {

        public QueryProvider()
        {

        }

        IQueryable<T> IQueryProvider.CreateQuery<T>(Expression expression)
        {
            Type elementType = typeof(T);
            return (IQueryable<T>)Activator.CreateInstance(typeof(SearchOptions<>).MakeGenericType(elementType), new object[] { this, expression });
        }



        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Type elementType = expression.Type;

            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(SearchOptions<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (System.Reflection.TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }



        T IQueryProvider.Execute<T>(Expression expression)
        {
            return (T)this.Execute(expression);
        }



        private string GetQueryText(Expression expression)
        {
            return null;
        }

        public object Execute(Expression expression)
        {
            Type type = expression.Type;
            Tenor.Data.SearchOptions so = new Tenor.Data.SearchOptions(type);



            return so.Execute();
        }


    }
}
