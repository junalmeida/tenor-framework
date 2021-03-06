using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;


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
            if (!baseClass.IsSubclassOf(typeof(EntityBase)))
                throw new InvalidTypeException(baseClass, "baseClass");

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
        public EntityBase[] Execute()
        {
            return Execute(null);
        }

        /// <summary>
        /// Executes the query defined on this instance.
        /// </summary>
        public EntityBase[] Execute(ConnectionStringSettings connection)
        {
            if (Top > 0 && eagerLoading.Count > 0)
                throw new NotSupportedException("Cannot use eager loading with Top/Limit.");
            if (Projections.Count > 0)
                throw new NotSupportedException("Cannot use projections without a Linq Query.");


            Tenor.Data.DataTable rs = SearchWithDataTable(connection);
            return EntityBase.BindRows(rs, this);
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
        public EntityBase[] ExecutePaged(int page, int pageSize)
        {
            return ExecutePaged(page, pageSize, null);
        }
                
        /// <summary>
        /// Executes the query defined on this instance respecting page and page size arguments.
        /// </summary>
        /// <param name="page">Desired page number (zero-based)</param>
        /// <param name="pageSize">Page size</param>
        public EntityBase[] ExecutePaged(int page, int pageSize, ConnectionStringSettings connection)
        {
            int totalCount = this.ExecuteCount();

            int pageCount = (int)System.Math.Ceiling((double)totalCount / (double)pageSize);

            if (pageCount > 0 && page >= pageCount) page = pageCount - 1;

            int skip = (page * pageSize);
            int take = pageSize;

            return ExecuteSkipTake(skip, take, connection);
        }

        /// <summary>
        /// Executes the query defined on this instance respecting page and page size arguments.
        /// </summary>
        /// <param name="skip">Number of rows to skip</param>
        /// <param name="take">Number of rows to take</param>
        public EntityBase[] ExecuteSkipTake(int skip, int take)
        {
            return ExecuteSkipTake(skip, take, null);
        }

        /// <summary>
        /// Executes the query defined on this instance respecting skip and take arguments.
        /// </summary>
        /// <param name="skip">Number of rows to skip</param>
        /// <param name="take">Number of rows to take</param>
        public EntityBase[] ExecuteSkipTake(int skip, int take, ConnectionStringSettings connection)
        {
            Tenor.Data.DataTable rs = SearchWithDataTable(connection, false, skip, take);
            return EntityBase.BindRows(rs, this);
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

        private Tenor.Data.DataTable SearchWithDataTable(ConnectionStringSettings connection, bool justCount, int? skip, int? take)
        {
            TenorParameter[] parameters = null;
            if (connection == null)
            {
                TableInfo table = TableInfo.CreateTableInfo(this.baseType);
                if (table == null)
                    throw new Tenor.Data.MissingTableMetaDataException(this.baseType);
                connection = table.GetConnection();
            }

            string sql = EntityBase.GetSearchSql(this, justCount, skip, take, connection, out parameters);

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