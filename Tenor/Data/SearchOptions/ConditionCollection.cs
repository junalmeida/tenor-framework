using System.Diagnostics;
using System;
using System.Collections;
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
            return Add(propertyName, value, CompareOperator.Equal);
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
            return Add(propertyName, value, compareOperator, (Type)null);
        }



        public ConditionCollection Add(string propertyName, object value, Type castType)
        {
            return Add(propertyName, value, Tenor.Data.CompareOperator.Equal, castType);
        }


        /// <summary>
        /// Adds a compare filter casting the database value.
        /// </summary>
        /// <param name="propertyName">The property name of the base class.</param>
        /// <param name="value">Some value.</param>
        /// <param name="compareOperator">The <see cref="CompareOperator"/>.</param>
        /// <returns>This instance.</returns>
        public ConditionCollection Add(string propertyName, object value, CompareOperator compareOperator, Type castType)
        {
            return Add(propertyName, value, compareOperator, castType, null);
        }      
        
        
        
        public ConditionCollection Add(string propertyName, object value, string joinAlias)
        {
            return Add(propertyName, value, Tenor.Data.CompareOperator.Equal, joinAlias);
        }


        public ConditionCollection Add(string propertyName, object value, Type castType, string joinAlias)
        {
            return Add(propertyName, value, Tenor.Data.CompareOperator.Equal, castType, joinAlias);
        }

        public ConditionCollection Add(string propertyName, object value, CompareOperator compareOperator, string joinAlias)
        {
            return Add(propertyName, value, compareOperator, null, joinAlias);
        }


        public ConditionCollection Add(string propertyName, object value, CompareOperator compareOperator, Type castType, string joinAlias)
        {
            return Add(new SearchCondition(joinAlias, propertyName, value, compareOperator, castType));
        }




        /// <summary>
        /// Adds a Search Condition.
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <remarks></remarks>
        public ConditionCollection Add(SearchCondition searchCondition)
        {
            if (searchCondition == null)
            {
                throw (new ArgumentNullException("searchCondition"));
            }
            else if (Count > 0 && this[Count - 1].GetType() != typeof(Data.LogicalOperator))
            {
                throw (new ArgumentException("Cannot insert a SearchCondition when the last item is not an operator.", "searchCondition"));
            }

            base.Add(searchCondition);
            return this;
        }


        /// <summary>
        /// Adds a condition group.
        /// </summary>
        /// <param name="SearchConditions"></param>
        /// <remarks></remarks>
        public ConditionCollection Add(ConditionCollection searchConditions)
        {
            if (searchConditions == null || searchConditions.Count == 0)
            {
                throw (new ArgumentException("Cannot insert Null or empty collection."));
            }
            else if (this.Items.Count > 0 && (this[this.Items.Count - 1].GetType() == typeof(SearchCondition) || this[this.Items.Count - 1].GetType() == typeof(ConditionCollection)))
            {
                throw (new ArgumentException("Cannot insert a SearchCondition when the last item is not an operator.", "searchConditions", null));
            }
            else if (!(searchConditions[searchConditions.Count - 1].GetType() == typeof(SearchCondition) || searchConditions[searchConditions.Count - 1].GetType() == typeof(ConditionCollection)))
            {
                throw (new ArgumentException("Invalid ConditionCollecion. Collections cannot end with an operator.", "searchConditions", null));
            }
            base.Add(searchConditions);

            return this;
        }


        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new void Add(object item)
        {
            base.Add(item);
        }

        #endregion

        #region " AND "
        /// <summary>
        /// Adds an equality comparison.
        /// </summary>
        public ConditionCollection And(string propertyName, object value)
        {
            return And(propertyName, value, CompareOperator.Equal);
        }

        /// <summary>
        /// Adds a comparison.
        /// </summary>
        public ConditionCollection And(string propertyName, object value, CompareOperator compareOperator)
        {
            return And(propertyName, value, compareOperator, (Type)null);
        }

        public ConditionCollection And(string propertyName, object value, Type castType)
        {
            return And(propertyName, value, CompareOperator.Equal, castType);
        }

        public ConditionCollection And(string propertyName, object value, CompareOperator compareOperator, Type castType)
        {
            return And(propertyName, value, compareOperator, castType, null);
        }





        public ConditionCollection And(string propertyName, object value, string joinAlias)
        {
            return And(propertyName, value, CompareOperator.Equal, joinAlias);
        }

        public ConditionCollection And(string propertyName, object value, CompareOperator compareOperator, string joinAlias)
        {
            return And(propertyName, value, compareOperator, null, joinAlias);
        }

        public ConditionCollection And(string propertyName, object value, Type castType, string joinAlias)
        {
            return And(propertyName, value, CompareOperator.Equal, castType, joinAlias);
        }
        public ConditionCollection And(string propertyName, object value, CompareOperator compareOperator, Type castType, string joinAlias)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(propertyName, value, compareOperator, castType, joinAlias);
        }


        public ConditionCollection And(SearchCondition SearchCondition)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(SearchCondition);
        }

        public ConditionCollection And(ConditionCollection SearchConditions)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(SearchConditions);
        }

        #endregion

        #region " Or "

        public ConditionCollection Or(string propertyName, object value)
        {
            return Or(propertyName, value, CompareOperator.Equal);
        }

        public ConditionCollection Or(string propertyName, object value, CompareOperator compareOperator)
        {
            return Or(propertyName, value, compareOperator, (Type)null);
        }

        public ConditionCollection Or(string propertyName, object value, Type castType)
        {
            return Or(propertyName, value, CompareOperator.Equal, castType);
        }

        public ConditionCollection Or(string propertyName, object value, CompareOperator compareOperator, Type castType)
        {
            return Or(propertyName, value, compareOperator, castType, null);
        }


        public ConditionCollection Or(string propertyName, object value, string joinAlias)
        {
            return Or(propertyName, value, CompareOperator.Equal, joinAlias);
        }

        public ConditionCollection Or(string propertyName, object value, CompareOperator compareOperator, string joinAlias)
        {
            return Or(propertyName, value, compareOperator, null, joinAlias);
        }

        public ConditionCollection @Or(string propertyName, object value, Type castType, string joinAlias)
        {
            return Or(propertyName, value, CompareOperator.Equal, castType, joinAlias);   
        }

        public ConditionCollection Or(string propertyName, object value, CompareOperator compareOperator, Type castType, string joinAlias)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(new SearchCondition(joinAlias, propertyName, value, compareOperator, castType));
        }



        public ConditionCollection Or(SearchCondition SearchCondition)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(SearchCondition);
        }

        public ConditionCollection Or(ConditionCollection SearchConditions)
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
            Join join = new Join(joinAlias);
            join.ParentAlias = parentAlias;
            join.PropertyName = propertyName;
            join.JoinMode = joinMode;
            includes.Add(join);
        }
        #endregion


        //public new object this[int index]
        //{
        //    get
        //    {
        //        return base[index];
        //    }
        //}

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
