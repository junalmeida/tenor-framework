using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Configuration;
using Tenor.BLL;


namespace Tenor.Data
{
    /// <summary>
    /// Represents a set of search definitions.
    /// </summary>
    public class SearchOptions : ISearchOptions
    {

        #region Search Data 
        internal Type baseType;
        //public Type Class
        //{
        //    get
        //    {
        //        return baseType;
        //    }
        //}

        /// <summary>
        /// Creates an instance of SearchOptions based on a base class.
        /// </summary>
        /// <param name="baseClass">Type of class whether the search will be performed.</param>
        public SearchOptions(Type baseClass)
        {
            if (baseClass == null)
                throw new ArgumentNullException("baseClass");
            if (!baseClass.IsSubclassOf(typeof(BLL.BLLBase)))
                throw new Tenor.BLL.InvalidTypeException(baseClass, "baseClass");

            baseType = baseClass;
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

        private ProjectionCollection _projections;
        public ProjectionCollection Projections
        {
            get
            {
                if (_projections == null)
                    _projections = new ProjectionCollection();
                return _projections;
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






        public override string ToString()
        {
            return this.GetType().Name + ": " + baseType.Name;
        }



        internal Dictionary<ForeignKeyInfo, string> eagerLoading = new Dictionary<ForeignKeyInfo, string>();

        /// <summary>
        /// Includes the property on the eager loading list.
        /// </summary>
        /// <param name="foreignPropertyName">The name of the property on the base class of this SearchOptions.</param>
        public void LoadAlso(string foreignPropertyName)
        {
            if (string.IsNullOrEmpty(foreignPropertyName))
                throw new ArgumentNullException("foreignPropertyName");

            ForeignKeyInfo fkInfo = ForeignKeyInfo.Create(baseType.GetProperty(foreignPropertyName));
            if (fkInfo == null)
                throw new MissingForeignKeyException(baseType, foreignPropertyName);

            //TODO: Find a better alias name.
            string alias = foreignPropertyName;

            eagerLoading.Add(fkInfo, alias);
        }
        #endregion

        #region Execute

        /// <summary>
        /// Executes the query defined on this instance.
        /// </summary>
        public BLLBase[] Execute()
        {
            return Execute(null);
        }

        /// <summary>
        /// Executes the query defined on this instance.
        /// </summary>
        public BLLBase[] Execute(ConnectionStringSettings connection)
        {
            if (Top > 0 && eagerLoading.Count > 0)
                throw new NotSupportedException("Cannot use eager loading with Top/Limit.");
            if (Projections.Count > 0)
                throw new NotSupportedException("Cannot use projections without a Linq Query.");


            Tenor.Data.DataTable rs = SearchWithDataTable(connection);
            return Tenor.BLL.BLLBase.BindRows(rs, this);
        }

        #endregion

        #region Execute Matrix

        /// <summary>
        /// Executes the query defined on this instance and returns a matrix of results.
        /// </summary>
        public object[][] ExecuteMatrix()
        {
            return ExecuteMatrix(null);
        }

        /// <summary>
        /// Executes the query defined on this instance and returns a matrix.
        /// </summary>
        public object[][] ExecuteMatrix(ConnectionStringSettings connection)
        {
            if (Top > 0 && eagerLoading.Count > 0)
                throw new NotSupportedException("Cannot use eager loading with Top/Limit.");

            Tenor.Data.DataTable rs = SearchWithDataTable(connection);

            List<object[]> result = new List<object[]>();

            foreach (DataRow row in rs.Rows)
            {
                List<object> colums = new List<object>();
                for (int i = 0; i < rs.Columns.Count; i++)
                {
                    if (row[i] is DBNull)
                        colums.Add(null);
                    else
                        colums.Add(row[i]);
                }
                result.Add(colums.ToArray());
            }

            return result.ToArray();
        }

        #endregion

        #region Execute With Paging

        /// <summary>
        /// Executes the query defined on this instance respecting page and page size arguments.
        /// </summary>
        /// <param name="page">Desired page number (zero-based)</param>
        /// <param name="pageSize">Page size</param>
        public BLLBase[] ExecutePaged(int page, int pageSize)
        {
            return ExecutePaged(page, pageSize, null);
        }


        /// <summary>
        /// Executes the query defined on this instance respecting page and page size arguments.
        /// </summary>
        /// <param name="page">Desired page number (zero-based)</param>
        /// <param name="pageSize">Page size</param>
        public BLLBase[] ExecutePaged(int page, int pageSize, ConnectionStringSettings connection)
        {
            int totalCount = this.ExecuteCount(connection);

            int pageCount = (int)System.Math.Ceiling((double)totalCount / (double)pageSize);

            if (pageCount > 0 && page >= pageCount) page = pageCount - 1;

            int pagingStart = (page * pageSize) + 1;
            int pagingEnd = (page + 1) * pageSize;

            Tenor.Data.DataTable rs = SearchWithDataTable(connection, false, pagingStart, pagingEnd);
            return Tenor.BLL.BLLBase.BindRows(rs, this);
        }

        #endregion

        #region Execute With Count

         /// <summary>
        /// Executes the query defined on this instance and returns the number of returned rows.
        /// </summary>
        public int ExecuteCount()
        {
            return ExecuteCount(null);
        }

        /// <summary>
        /// Executes the query defined on this instance and returns the number of returned rows.
        /// </summary>
        public int ExecuteCount(ConnectionStringSettings connection)
        {
            if (this.eagerLoading.Count > 0)
                throw new NotSupportedException("Cannot use eager loading with aggregation.");
            Tenor.Data.DataTable rs = SearchWithDataTable(connection, true);
            return System.Convert.ToInt32(rs.Rows[0][0]);
        }
        #endregion

        #region Search Datatable

        internal Tenor.Data.DataTable SearchWithDataTable(ConnectionStringSettings connection)
        {
            return SearchWithDataTable(connection, false);
        }

        private Tenor.Data.DataTable SearchWithDataTable(ConnectionStringSettings connection, bool justCount)
        {
            return SearchWithDataTable(connection, justCount, null, null);
        }

        private Tenor.Data.DataTable SearchWithDataTable(ConnectionStringSettings connection, bool justCount, int? pagingStart, int? pagingEnd)
        {
            TenorParameter[] parameters = null;
            if (connection == null)
            {
                TableInfo table = TableInfo.CreateTableInfo(this.baseType);
                connection = table.GetConnection();
            }

            string sql = Tenor.BLL.BLLBase.GetSearchSql(this, justCount, pagingStart, pagingEnd, connection, out parameters);

            Tenor.Data.DataTable rs = new Tenor.Data.DataTable(sql, parameters, connection);
            DataSet ds = new DataSet();
            ds.Tables.Add(rs);
            ds.EnforceConstraints = false;
            rs.Bind();

            return rs;
        }

        #endregion

    }
}