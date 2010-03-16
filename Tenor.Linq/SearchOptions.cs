using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tenor.BLL;
using System.Collections;
using System.Linq.Expressions;

namespace Tenor.Linq
{
    /// <summary>
    /// Represents a set of search definitions. You can use Linq to make your queries.
    /// </summary>
    public class SearchOptions<T> : IOrderedQueryable, IOrderedQueryable<T>, IEnumerable, IQueryable, System.Linq.IQueryable<T> //where T : BLLBase
    {

        public static IQueryable<T> CreateQuery()
        {
            return new SearchOptions<T>();
        }

        #region Enumerators

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)this.Provider.Execute(this.Expression)).GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>)this.Provider.Execute(this.Expression)).GetEnumerator();
        }

        #endregion



        internal SearchOptions() : this(null, null)
        {
        }

        internal SearchOptions(Expression expression)
            : this(null, expression)
        {
        }

        public SearchOptions(IQueryProvider provider, Expression expression)
        {

            if (provider == null)
                this.provider = new TenorQueryProvider();
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

        private bool _LazyLoading = true;
        /// <summary>
        /// Determines whether the LazyLoading is enabled.
        /// </summary>
        /// <value>A Boolean.</value>
        /// <remarks>The default is True.</remarks>
        public virtual bool LazyLoading
        {
            get { return _LazyLoading; }
            set { _LazyLoading = value; }
        }
    }
}
