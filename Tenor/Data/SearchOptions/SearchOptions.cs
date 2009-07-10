using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Configuration;


namespace Tenor.Data
{
    /// <summary>
    /// Represents a set of search definitions.
    /// </summary>
    /// <remarks></remarks>
    public class SearchOptions
    {


        internal Type _BaseClass;

        /// <summary>
        /// Creates an instance of SearchOptions based on a base class.
        /// </summary>
        /// <param name="BaseClass">Type of class whether the search will be performed.</param>
        /// <remarks></remarks>
        public SearchOptions(Type baseClass)
        {
            if (baseClass == null)
                throw new ArgumentNullException("baseClass");
            if (!baseClass.IsSubclassOf(typeof(BLL.BLLBase)))
                throw new Tenor.BLL.InvalidTypeException(baseClass, "baseClass");

            _BaseClass = baseClass;
        }



        private bool _LazyLoading = true;
        /// <summary>
        /// Determines whether the LazyLoading is enabled.
        /// </summary>
        /// <value>A Boolean.</value>
        /// <remarks>The default is True.</remarks>
        public virtual bool LazyLoading
        {
            get
            {
                return _LazyLoading;
            }
            set
            {
                _LazyLoading = value;
            }
        }

        private bool _Distinct = false;
        /// <summary>
        /// Determines whether the Distinct Query is enabled.
        /// </summary>
        /// <value>A Boolean.</value>
        /// <returns></returns>
        /// <remarks>The default is False.</remarks>
        public virtual bool Distinct
        {
            get
            {
                return _Distinct;
            }
            set
            {
                _Distinct = value;
            }
        }



        private int _Top = 0;
        /// <summary>
        /// Determines whether the TOP or Limit function is enabled.
        /// </summary>
        /// <value>A Integer.</value>
        /// <returns></returns>
        /// <remarks>The default is 0 (no top).</remarks>
        public virtual int Top
        {
            get
            {
                return _Top;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                _Top = value;
            }
        }


        private ConditionCollection _Conditions;
        public ConditionCollection Conditions
        {
            get
            {
                if (_Conditions == null)
                {
                    _Conditions = new ConditionCollection();
                }
                return _Conditions;
            }
            set
            {
                if (value == null)
                {
                    throw (new ArgumentNullException("value"));
                }
                _Conditions = value;
            }
        }


        private SortingCollection _Sorting;
        public SortingCollection Sorting
        {
            get
            {
                if (_Sorting == null)
                {
                    _Sorting = new SortingCollection();
                }
                return _Sorting;
            }
            set
            {
                if (value == null)
                {
                    throw (new ArgumentNullException("value"));
                }
                _Sorting = value;
            }
        }


        /// <summary>
        /// Executes the query defined on this instance.
        /// </summary>
        public BLL.BLLBase[] Execute()
        {
            return BLL.BLLBase.Search(this);
        }


        /// <summary>
        /// Executes the query defined on this instance.
        /// </summary>
        public BLL.BLLBase[] Execute(ConnectionStringSettings Connection)
        {
            return BLL.BLLBase.Search(this, Connection);
        }


        /// <summary>
        /// Executes the query defined on this instance and returns the number of returned rows.
        /// </summary>
        public int ExecuteCount()
        {
            return BLL.BLLBase.Count(this);
        }


        /// <summary>
        /// Executes the query defined on this instance and returns the number of returned rows.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public int ExecuteCount(ConnectionStringSettings Connection)
        {
            return BLL.BLLBase.Count(this, Connection);
        }


        public override string ToString()
        {
            return this.GetType().Name + ": " + _BaseClass.Name;
        }
    }
}