using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tenor.BLL;
using System.Collections;
using System.Linq.Expressions;

namespace Tenor.Linq
{
    public class SearchOptions<T> : IEnumerable, IQueryable, System.Linq.IQueryable<T> where T : BLLBase
    {

        #region Enumerators

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)this.Provider.Execute<T>(this.Expression)).GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>)this.Provider.Execute<T>(this.Expression)).GetEnumerator();
        }

        #endregion



        public SearchOptions() : this(null)
        {
        }

        public SearchOptions(Expression expression) : this (null,expression)
        {
        }

        public SearchOptions(QueryProvider provider, Expression expression)
        {

            if (provider == null)
                this.provider = new QueryProvider();
            else
                this.provider = provider;

            if (expression == null)
                this.expression = Expression.Constant(this);
            else
                this.expression = expression;
        }

        public Type ElementType
        {
            get { return typeof(T); }
        }

        Expression expression = null;
        public Expression Expression
        {
            get { return expression; }
        }

        IQueryProvider provider = null;
        public IQueryProvider Provider
        {
            get { return provider; }
        }


    }
}
