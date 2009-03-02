using System.Diagnostics;
using System;
using System.Collections;
using Microsoft.VisualBasic;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Tenor.Data
{
    /// <summary>
    /// This class provides a way to make searches on entities.
    /// </summary>
    public class ConditionCollection : Collection<object>
    {


        #region " Constructors "
        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        public ConditionCollection()
        {
        }

        /// <summary>
        /// Creates an instance with one equality filter. 
        /// </summary>
        /// <param name="property">The property name of the base class.</param>
        /// <param name="value">Some value.</param>
        public ConditionCollection(string property, object value)
        {
            Add(property, value);
        }

        /// <summary>
        /// Creates and instance with one filter.
        /// </summary>
        /// <param name="property">The property name of the base class.</param>
        /// <param name="value">Some value.</param>
        /// <param name="compareOperator">The <see cref="CompareOperator"/> used to compare values.</param>
        public ConditionCollection(string property, object value, CompareOperator compareOperator)
        {
            Add(property, value, compareOperator);
        }

        ///// <summary>
        ///// Creates an instance with one filter.
        ///// </summary>
        ///// <param name="Table"></param>
        ///// <param name="Property"></param>
        ///// <param name="value"></param>
        //public ConditionCollection(Type Table, string propertyName, object value)
        //{
        //    Add(Table, propertyName, value);
        //}

        //public ConditionCollection(Type Table, string propertyName, object value, Tenor.Data.CompareOperator CompareOperator)
        //{
        //    Add(Table, propertyName, value, CompareOperator);
        //}
        #endregion

        #region " Add "
        /// <summary>
        /// Adds a logical operator between conditions or condition groups.
        /// </summary>
        /// <param name="logicalOperator">The <see cref="LogicalOperator"/>.</param>
        /// <remarks></remarks>
        public void Add(LogicalOperator logicalOperator)
        {
            if (this.Items.Count == 0)
            {
                throw (new ArgumentException("Cannot insert an operator when the collection is empty.", "logicalOperator", null));
            }
            else if (this[this.Items.Count - 1].GetType() == typeof(LogicalOperator))
            {
                throw (new ArgumentException("Cannot insert an operator when the last item is another operator.", "logicalOperator", null));
            }
            base.Add(logicalOperator);
        }

        /// <summary>
        /// Adds an equality filter.
        /// </summary>
        /// <param name="propertyName">The property name of the base class.</param>
        /// <param name="value">Some value.</param>
        /// <returns>This instance.</returns>
        public ConditionCollection Add(string propertyName, object value)
        {
            Add(new SearchCondition(null, propertyName, value, CompareOperator.Equal));
            return this;
        }

        /// <summary>
        /// Adds a compare filter.
        /// </summary>
        /// <param name="propertyName">The property name of the base class.</param>
        /// <param name="value">Some value.</param>
        /// <param name="compareOperator">The <see cref="CompareOperator"/>.</param>
        /// <returns>This instance.</returns>
        public ConditionCollection Add(string propertyName, object value, CompareOperator compareOperator)
        {
            Add(new SearchCondition(null, propertyName, value, compareOperator));
            return this;
        }

        /// <summary>
        /// Adds a compare filter casting the database value.
        /// </summary>
        /// <param name="propertyName">The property name of the base class.</param>
        /// <param name="value">Some value.</param>
        /// <param name="compareOperator">The <see cref="CompareOperator"/>.</param>
        /// <returns>This instance.</returns>
        public ConditionCollection Add(string propertyName, object value, Tenor.Data.CompareOperator compareOperator, Type castType)
        {
            Add(new SearchCondition(null, propertyName, value, compareOperator, castType));
            return this;
        }

        public ConditionCollection Add(string propertyName, object value, Type castType)
        {
            Add(new SearchCondition(null, propertyName, value, Tenor.Data.CompareOperator.Equal, castType));
            return this;
        }

        public ConditionCollection Add(string joinAlias, string propertyName, object value)
        {
            Add(new SearchCondition(joinAlias, propertyName, value, Tenor.Data.CompareOperator.Equal));
            return this;
        }


        public ConditionCollection Add(string joinAlias, string propertyName, object value, Tenor.Data.CompareOperator compareOperator)
        {
            Add(new SearchCondition(joinAlias, propertyName, value, compareOperator));
            return this;
        }



        public ConditionCollection Add(string joinAlias, string propertyName, object value, Tenor.Data.CompareOperator compareOperator, Type castType)
        {
            Add(new SearchCondition(joinAlias, propertyName, value, compareOperator, castType));
            return this;
        }



        public ConditionCollection Add(string joinAlias, string propertyName, object value, Type castType)
        {
            Add(new SearchCondition(joinAlias, propertyName, value, Tenor.Data.CompareOperator.Equal, castType));
            return this;
        }

        /// <summary>
        /// Adiciona uma condição
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <remarks></remarks>
        public ConditionCollection Add(SearchCondition SearchCondition)
        {
            if (SearchCondition == null)
            {
                throw (new ArgumentException("Cannot insert a Null object."));
            }
            else if (Count > 0 && this[Count - 1].GetType() != typeof(Data.LogicalOperator))
            {
                throw (new ArgumentException("Cannot insert a SearchCondition when the last item is not an operator.", "SearchCondition", null));
            }

            base.Add(SearchCondition);
            return this;
        }


        /// <summary>
        /// Adiciona um grupo de condições.
        /// </summary>
        /// <param name="SearchConditions"></param>
        /// <remarks></remarks>
        public ConditionCollection Add(ConditionCollection SearchConditions)
        {
            if (SearchConditions == null || SearchConditions.Count == 0)
            {
                throw (new ArgumentException("Cannot insert Null or empty collection."));
            }
            else if (this.Items.Count > 0 && (this[this.Items.Count - 1].GetType() == typeof(SearchCondition) || this[this.Items.Count - 1].GetType() == typeof(ConditionCollection)))
            {
                throw (new ArgumentException("Cannot insert a SearchCondition when the last item is not an operator.", "SearchConditions", null));
            }
            else if (!(SearchConditions[SearchConditions.Count - 1].GetType() == typeof(SearchCondition) || SearchConditions[SearchConditions.Count - 1].GetType() == typeof(ConditionCollection)))
            {
                throw (new ArgumentException("Invalid ConditionCollecion. Collections cannot end with an operator.", "SearchConditions", null));
            }
            base.Add(SearchConditions);

            return this;
        }

        #endregion

        #region " AND "
        /// <summary>
        /// Adiciona uma condição de igualdade entre propriedade e valor.
        /// </summary>
        /// <param name="Property"></param>
        /// <param name="value"></param>
        /// <remarks></remarks>
        public ConditionCollection @And(string propertyName, object value)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(propertyName, value);
        }

        /// <summary>
        /// Adiciona uma condição comparativa entre propriedade e valor.
        /// </summary>
        /// <param name="Property"></param>
        /// <param name="value"></param>
        /// <param name="CompareOperator"></param>
        /// <remarks></remarks>
        public ConditionCollection @And(string propertyName, object value, Tenor.Data.CompareOperator CompareOperator)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(propertyName, value, CompareOperator);

        }

        /// <summary>
        /// Adiciona uma condição comparativa entre propriedade e valor
        /// </summary>
        /// <param name="Property"></param>
        /// <param name="value"></param>
        /// <param name="CompareOperator"></param>
        /// <param name="castType"></param>
        /// <remarks></remarks>
        public ConditionCollection @And(string propertyName, object value, Tenor.Data.CompareOperator compareOperator, Type castType)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(propertyName, value, compareOperator, castType);
        }


        public ConditionCollection @And(string propertyName, object value, Type castType)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(propertyName, value, castType);
        }


        public ConditionCollection @And(string joinAlias, string propertyName, object value)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(joinAlias, propertyName, value);
        }

        public ConditionCollection @And(string joinAlias, string propertyName, object value, Tenor.Data.CompareOperator CompareOperator)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(joinAlias, propertyName, value, CompareOperator);
        }

        public ConditionCollection @And(string joinAlias, string propertyName, object value, Tenor.Data.CompareOperator compareOperator, Type castType)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(joinAlias, propertyName, value, compareOperator, castType);
        }

        public ConditionCollection @And(string joinAlias, string propertyName, object value, Type castType)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(joinAlias, propertyName, value, castType);
        }

        public ConditionCollection @And(SearchCondition SearchCondition)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(SearchCondition);
        }

        public ConditionCollection @And(ConditionCollection SearchConditions)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(SearchConditions);
        }

        #endregion

        #region " Or "


        /// <summary>
        /// Adiciona uma condição de igualdade entre propriedade e valor.
        /// </summary>
        /// <param name="Property"></param>
        /// <param name="value"></param>
        /// <remarks></remarks>
        public ConditionCollection @Or(string propertyName, object value)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(propertyName, value);
        }

        /// <summary>
        /// Adiciona uma condição comparativa entre propriedade e valor.
        /// </summary>
        /// <param name="Property"></param>
        /// <param name="value"></param>
        /// <param name="CompareOperator"></param>
        /// <remarks></remarks>
        public ConditionCollection @Or(string propertyName, object value, Tenor.Data.CompareOperator CompareOperator)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(propertyName, value, CompareOperator);
        }

        /// <summary>
        /// Adiciona uma condição comparativa entre propriedade e valor
        /// </summary>
        /// <param name="Property"></param>
        /// <param name="value"></param>
        /// <param name="CompareOperator"></param>
        /// <param name="castType"></param>
        /// <remarks></remarks>
        public ConditionCollection @Or(string propertyName, object value, Tenor.Data.CompareOperator compareOperator, Type castType)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(propertyName, value, compareOperator, castType);
        }

        public ConditionCollection @Or(string propertyName, object value, Type castType)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(propertyName, value, castType);
        }

        public ConditionCollection @Or(string joinAlias, string propertyName, object value)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(joinAlias, propertyName, value);
        }

        public ConditionCollection @Or(string joinAlias, string propertyName, object value, Tenor.Data.CompareOperator CompareOperator)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(joinAlias, propertyName, value, CompareOperator);
        }

        public ConditionCollection @Or(string joinAlias, string propertyName, object value, Tenor.Data.CompareOperator compareOperator, Type castType)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(joinAlias, propertyName, value, compareOperator, castType);
        }

        public ConditionCollection @Or(string joinAlias, string propertyName, object value, Type castType)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(joinAlias, propertyName, value, castType);
        }


        public ConditionCollection @Or(SearchCondition SearchCondition)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(SearchCondition);
        }

        public ConditionCollection @Or(ConditionCollection SearchConditions)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(SearchConditions);
        }
        #endregion

        #region " Contains e IndexOf "
        public bool Contains(ConditionCollection SearchConditions)
        {
            return base.Contains(SearchConditions);
        }

        public bool Contains(SearchCondition SearchCondition)
        {
            return base.Contains(SearchCondition);
        }

        public bool Contains(Data.LogicalOperator LogicalOperator)
        {
            return base.Contains(LogicalOperator);
        }

        public int IndexOf(ConditionCollection SearchConditions)
        {
            return base.IndexOf(SearchConditions);
        }

        public int IndexOf(SearchCondition SearchCondition)
        {
            return base.IndexOf(SearchCondition);
        }

        public int IndexOf(Data.LogicalOperator LogicalOperator)
        {
            return base.IndexOf(LogicalOperator);
        }

        #endregion

        #region " Include Table "
        internal List<Join> includes = new List<Join>();

        /// <summary>
        /// Creates a join.
        /// </summary>
        /// <remarks></remarks>
        public void Include(string propertyName, string joinAlias)
        {
            Include(null, propertyName, joinAlias, JoinMode.InnerJoin);
        }

        /// <summary>
        /// Creates a join.
        /// </summary>
        /// <remarks></remarks>
        public void Include(string parentAlias, string propertyName, string joinAlias, JoinMode joinMode)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw (new ArgumentNullException("propertyName"));
            }
            if (string.IsNullOrEmpty(joinAlias))
            {
                throw (new ArgumentNullException("joinAlias"));
            }
            Join join = new Join();
            join.parentAlias = parentAlias;
            join.joinAlias = joinAlias;
            join.propertyName = propertyName;
            join.joinMode = joinMode;
            includes.Add(join);
        }
        #endregion


        public new object this[int index]
        {
            get
            {
                return base[index];
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public void Insert()
        {
            throw new NotSupportedException();
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public void Remove()
        {
            throw new NotSupportedException();
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public void RemoveAt()
        {
            throw new NotSupportedException();
        }
    }
}
