/* Copyright (c) 2009 Marcos Almeida Jr, Rachel Carvalho and Vinicius Barbosa.
 *
 * See the file license.txt for copying permission.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Tenor.Data;
using System.Configuration;
using System.Data.Common;
using Tenor.Data.Dialects;

namespace Tenor.BLL
{
    public abstract partial class BLLBase
    {


        /// <summary>
        /// Binds this entity instance to database data using primary keys current data.
        /// </summary>
        public void Bind()
        {
            Bind(true);
        }

        
        /// <summary>
        /// Binds this entity instance to database data using primary keys current data.
        /// </summary>
        /// <param name="lazyLoading">Defines weather lazy loading is enabled.</param>
        /// <exception cref="MissingFieldsException">Ocurrs when no property with a FieldAttribute is defined.</exception>
        /// <exception cref="MissingPrimaryKeyException">Occurs when no FieldAttribute with primary key is defined.</exception>
        protected void Bind(bool lazyLoading)
        {
            FieldInfo[] fields = GetFields(this.GetType(), true);


            if (fields.Length == 0)
                throw (new MissingPrimaryKeyException(this.GetType()));

            List<string> propertyNames = new List<string>();
            foreach (FieldInfo field in fields)
            {
                propertyNames.Add(field.RelatedProperty.Name);
            }

            Bind(lazyLoading, propertyNames.ToArray());
        }

        /// <summary>
        /// Binds this entity instance to database data using current data of <paramref name="filterMembers">Filter Members</paramref>.
        /// </summary>
        /// <param name="lazyLoading">Defines weather lazy loading is enabled.</param>
        /// <param name="filterMembers">Property members used to filter data.</param>
        /// <remarks></remarks>
        protected void Bind(bool lazyLoading, string[] filterMembers)
        {
            Bind(lazyLoading, filterMembers, null);
        }



        /// <summary>
        /// Binds this entity instance to database data using current data of <paramref name="filterMembers">Filter Members</paramref>.
        /// </summary>
        /// <param name="lazyLoading">Defines weather lazy loading is enabled.</param>
        /// <param name="filterMembers">Property members used to filter data.</param>
        /// <param name="dataRow">A System.Data.DataRow to bind this instance. When this parameter is not set, Tenor return data from the database.</param>
        protected virtual void Bind(bool lazyLoading, string[] filterMembers, System.Data.DataRow dataRow)
        {
            bool fromSearch = (dataRow != null);
            //dataRow is null when Bind is called from LoadForeing or directly from user code.
            //dataRow is not null when Bind is called from Search (see Search.cs)

            if (!fromSearch && ClassMetadata.Cacheable && LoadFromCache())
            {
                //LoadFromCache returns true, the Bind was done.
                //If not cacheable, never LoadFromCache (see Cache.cs)
                return;
            }


            TableInfo table = TableInfo.CreateTableInfo(this.GetType());
            ConnectionStringSettings connection = table.GetConnection();
            if (!fromSearch)
            {

                //Retrieve data

                SearchOptions so = new SearchOptions(this.GetType());



                // SELECT and FILTERS

                List<FieldInfo> filters = new List<FieldInfo>();
                foreach (string s in filterMembers)
                {
                    FieldInfo field = FieldInfo.Create(this.GetType().GetProperty(s));
                    if (field == null)
                        throw new Tenor.Data.MissingFieldException(this.GetType(), s);
                    filters.Add(field);
                }


                foreach (FieldInfo field in filters)
                {
                    if (so.Conditions.Count > 0)
                    {
                        so.Conditions.Add(Tenor.Data.LogicalOperator.And);
                    }
                    so.Conditions.Add(field.RelatedProperty.Name, field.PropertyValue(this));
                }


                System.Data.DataTable dt = SearchWithDataTable(so, connection);

                if (dt.Rows.Count == 0)
                {
                    dt.Dispose();
                    throw (new RecordNotFoundException());
                }
                else if (dt.Rows.Count > 1)
                {
                    throw new ManyRecordsFoundException();
                }

                dataRow = dt.Rows[0];

            }




            FieldInfo[] fields = GetFields(this.GetType());
            SpecialFieldInfo[] spfields = GetSpecialFields(this.GetType());

            foreach (FieldInfo f in fields)
            {
                if (f.PrimaryKey || !f.LazyLoading)
                {
                    f.SetPropertyValue(this, dataRow[f.DataFieldName]);
                }
            }
            foreach (SpecialFieldInfo f in spfields)
            {
                f.SetPropertyValue(this, dataRow[f.Alias]);
            }


            dataRow = null;

            
            //TODO: Try to bind lazy properties at the same database round, and
            //we need to define a way to choose which relations to load during bind.

            /*
            if (!lazyLoading)
            {
            ForeignKeyInfo[] foreignkeys = GetForeignKeys(this.GetType());
                foreach (ForeignKeyInfo f in foreignkeys)
                {
                    this.LoadForeign(f.RelatedProperty.Name, true, null, connection);
                }
            }
             */


            if (ClassMetadata.Cacheable)
            {
                SaveToCache();
            }
            //Setting the lazy status.
            this.EnableLazyLoading(lazyLoading);
        }




        /// <summary>
        /// Creates an update query of this entity data.
        /// </summary>
        /// <param name="isUpdate">Determines whether to update an existing record or create a new record.</param>
        /// <param name="parameters">Outputs an array of TenorParameter with required parameters.</param>
        /// <param name="autoKeyField">Outputs the autonumber FieldInfo.</param>
        /// <param name="specialValues">The special values can contains sql sentences/sequences/etc</param>
        /// <param name="connection">The Connection.</param>
        /// <param name="dialect"></param>
        /// <returns>Return a SQL query string.</returns>
        internal string GetSaveSql(bool isUpdate, ConnectionStringSettings connection, System.Collections.Specialized.NameValueCollection specialValues, out FieldInfo autoKeyField, out TenorParameter[] parameters, out GeneralDialect dialect)
        {
            Dictionary<FieldInfo, object> data = new Dictionary<FieldInfo, object>();

            TableInfo table = TableInfo.CreateTableInfo(this.GetType());
            if (connection == null)
                connection = table.GetConnection();

            dialect = DialectFactory.CreateDialect(connection);

            autoKeyField = null;
            ConditionCollection conditions = new ConditionCollection();

            List<FieldInfo> fieldInfos = new List<FieldInfo>(GetFields(this.GetType()));
            for (int i = fieldInfos.Count - 1; i >= 0; i--)
            {
                if (isUpdate && fieldInfos[i].PrimaryKey)
                {
                    if (conditions.Count != 0)
                        conditions.Add(LogicalOperator.And);
                    conditions.Add(fieldInfos[i].RelatedProperty.Name, fieldInfos[i].PropertyValue(this));

                    if (fieldInfos[i].AutoNumber)
                    {
                        autoKeyField = fieldInfos[i];
                        data.Add(fieldInfos[i], null);
                    }
                    else
                        fieldInfos.RemoveAt(i);
                }
                else if (fieldInfos[i].AutoNumber)
                {
                    autoKeyField = fieldInfos[i];
                    data.Add(fieldInfos[i], null);
                }
                else if (fieldInfos[i].LazyLoading && !propertyData.ContainsKey(fieldInfos[i].RelatedProperty.Name))
                {
                    fieldInfos.RemoveAt(i);
                }
                else
                {
                    data.Add(fieldInfos[i], fieldInfos[i].PropertyValue(this));
                }
            }

            string sql = dialect.CreateSaveSql(this.GetType(), data, specialValues, conditions, out parameters) + dialect.LineEnding;

            if (dialect.GetIdentityOnSameCommand && !isUpdate && autoKeyField != null)
            {
                string queryFormat = @"{0}
{1}
{2}";

                string before = string.Empty;
                if (!string.IsNullOrEmpty(dialect.IdentityBeforeQuery))
                {
                    before = dialect.IdentityBeforeQuery + dialect.LineEnding;
                }

                string after = string.Empty;
                if (!string.IsNullOrEmpty(dialect.IdentityAfterQuery))
                {
                    after = string.Format(dialect.IdentityAfterQuery, autoKeyField.InsertSQL) + dialect.LineEnding;
                }

                sql = string.Format(queryFormat, before, sql, after);
            }

            return sql;
        }


        /// <summary>
        /// Creates the SQL query based on conditions using the current dialect.
        /// Generally, you can call this method for debugging reasons.
        /// </summary>
        /// <param name="searchOptions">The search definitions.</param>
        /// <param name="connection">The Connection.</param>
        /// <param name="justCount">Indicates that this is a COUNT query.</param>
        /// <param name="parameters">Outputs the generated parameters.</param>
        /// <returns>A SQL query.</returns>
        public static string GetSearchSql(SearchOptions searchOptions, bool justCount, ConnectionStringSettings connection, out TenorParameter[] parameters)
        {
            GeneralDialect dialect = DialectFactory.CreateDialect(connection);

            if (searchOptions == null)
            {
                throw (new ArgumentNullException("searchOptions", "You must specify a SearchOptions instance."));
            }

            TableInfo table = TableInfo.CreateTableInfo(searchOptions._BaseClass);

            if (connection == null)
            {
                connection = table.GetConnection();
            }


            //Read Joins

            Join[] joins = GetPlainJoins(searchOptions.Conditions, searchOptions._BaseClass);

            //Get necessary fields to create the select statement.
            List<FieldInfo> fieldInfos = new List<FieldInfo>();
            foreach (FieldInfo f in BLLBase.GetFields(searchOptions._BaseClass))
            {
                if (f.PrimaryKey || !f.LazyLoading)
                    fieldInfos.Add(f);
            }
            SpecialFieldInfo[] spFields = BLLBase.GetSpecialFields(searchOptions._BaseClass);

            string sqlFields = dialect.CreateSelectSql(table.RelatedTable, fieldInfos.ToArray(), spFields);


            //Sorting
            string appendToSelect = null;
            string sqlSort = dialect.CreateSortSql(searchOptions.Sorting, table.RelatedTable, joins, searchOptions.Distinct, out appendToSelect);
            if (!string.IsNullOrEmpty(appendToSelect))
                sqlFields += appendToSelect;

            //Creates the where part
            string sqlWhere = dialect.CreateWhereSql(searchOptions.Conditions, searchOptions._BaseClass, joins, out parameters);

            // Creates the join parts
            string sqlJoins = dialect.CreateJoinsSql(joins);


            // Creates the entire sql string
            string sql = dialect.CreateFullSql(searchOptions._BaseClass, searchOptions.Distinct, justCount, searchOptions.Top, sqlFields, sqlJoins, sqlSort, sqlWhere);


            Tenor.Diagnostics.Debug.DebugSQL("GetSearchSql()", sql, parameters, connection);

            return sql;
        }

        /// <summary>
        /// Persists a list on the database. 
        /// </summary>
        /// <param name="propertyName">The name of a many-to-many property on this class.</param>
        public virtual void SaveList(string propertyName)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");
            System.Reflection.PropertyInfo prop = this.GetType().GetProperty(propertyName);
            if (prop == null)
                throw new ArgumentException(string.Format("The property '{0}' was not found on '{1}'.", propertyName, this.GetType().FullName), "propertyName");
            ForeignKeyInfo fkInfo = ForeignKeyInfo.Create(prop);
            if (fkInfo == null)
                throw new InvalidMappingException(this.GetType());

            if (!fkInfo.IsManyToMany)
                throw new TenorException("Currently, only many-to-many relations are supported");


            TableInfo table = TableInfo.CreateTableInfo(this.GetType());
            if (table == null)
                throw new InvalidMappingException(this.GetType());

            ConnectionStringSettings connection = (tenorTransaction == null ?table.GetConnection() : tenorTransaction.Connection);
            GeneralDialect dialect = DialectFactory.CreateDialect(connection);

            TenorParameter[] parameters;
            DbTransaction t = (tenorTransaction == null ? null : tenorTransaction.dbTransaction);

            if (dialect.GetType() == typeof(Tenor.Data.Dialects.TSql.TSql))
            {
                //oh god! do you have a better idea on where to write this code?
                System.Data.SqlClient.SqlBulkCopy bulk;
                if (t == null)
                    bulk = new System.Data.SqlClient.SqlBulkCopy(tenorTransaction.Connection.ConnectionString);
                else
                    bulk = new System.Data.SqlClient.SqlBulkCopy((System.Data.SqlClient.SqlConnection)t.Connection, System.Data.SqlClient.SqlBulkCopyOptions.Default, (System.Data.SqlClient.SqlTransaction)t);

                bulk.DestinationTableName = dialect.GetPrefixAndTable(fkInfo.ManyToManyTablePrefix, fkInfo.ManyToManyTable);
                System.Data.DataTable data;
                string sql = dialect.CreateSaveList(table, fkInfo, this, out parameters, out data);
                Helper.ExecuteQuery(sql, parameters, t, dialect);
                bulk.WriteToServer(data);
            }
            else
            {
                string sql = dialect.CreateSaveListSql(table, fkInfo, this, out parameters);
                Helper.ExecuteQuery(sql, parameters, t, dialect);
            }
        }
    }
}
