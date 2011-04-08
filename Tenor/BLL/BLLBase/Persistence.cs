/*
 * Licensed under the MIT License:
 * http://www.opensource.org/licenses/mit-license.php
 * 
 * Copyright (c) 2009 Marcos Almeida Jr, Rachel Carvalho and Vinicius Barbosa.
 *
 * See the file license.txt for copying permission.
 */
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Text;
using Tenor.Data;
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
                throw (new Tenor.Data.MissingPrimaryKeyException(this.GetType()));

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
            Bind(lazyLoading, null, filterMembers, null);
        }



        /// <summary>
        /// Binds this entity instance to database data using current data of <paramref name="filterMembers">Filter Members</paramref>.
        /// </summary>
        /// <param name="lazyLoading">Defines weather lazy loading is enabled.</param>
        /// <param name="filterMembers">Property members used to filter data.</param>
        /// <param name="dataRow">A System.Data.DataRow to bind this instance. When this parameter is not set, Tenor return data from the database.</param>
        protected virtual void Bind(bool lazyLoading, string baseFieldAlias, string[] filterMembers, System.Data.DataRow dataRow)
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


                System.Data.DataTable dt = so.SearchWithDataTable(connection);

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
                    string alias = baseFieldAlias + f.DataFieldName;
                    f.SetPropertyValue(this, dataRow[alias]);
                }
            }
            foreach (SpecialFieldInfo f in spfields)
            {
                string alias = baseFieldAlias + f.Alias;
                f.SetPropertyValue(this, dataRow[alias]);
            }


            dataRow = null;

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
        /// <param name="pagingStart">First row number that should be returned when paging</param>
        /// <param name="pagingEnd">Last row number that should be returned when paging</param>
        /// <param name="parameters">Outputs the generated parameters.</param>
        /// <returns>A SQL query.</returns>
        public static string GetSearchSql(SearchOptions searchOptions, bool justCount, int? pagingStart, int? pagingEnd, ConnectionStringSettings connection, out TenorParameter[] parameters)
        {
            GeneralDialect dialect = DialectFactory.CreateDialect(connection);

            if (searchOptions == null)
            {
                throw (new ArgumentNullException("searchOptions", "You must specify a SearchOptions instance."));
            }

            TableInfo table = TableInfo.CreateTableInfo(searchOptions.baseType);

            if (connection == null)
            {
                connection = table.GetConnection();
            }

            //Read Projections
            List<Projection> projections = new List<Projection>();
            foreach (Projection p in searchOptions.Projections)
                projections.Add(p);

            //Read Joins

            List<Join> joins = new List<Join>();
            joins.AddRange(GetPlainJoins(searchOptions, dialect));



            //Get necessary fields to create the select statement.
            StringBuilder sqlFields = new StringBuilder();
            List<FieldInfo> fieldInfos = new List<FieldInfo>();
            List<SpecialFieldInfo> spFields = new List<SpecialFieldInfo>();


            FieldInfo[] allFields;
            if (justCount)
                allFields = BLLBase.GetFields(searchOptions.baseType, true); //lets count using distinct subquery
            else if (searchOptions.Projections.Count > 0)
            {
                allFields = ReadProjections<FieldInfo>(projections, null, BLLBase.GetFields(searchOptions.baseType));
            }
            else
                allFields = BLLBase.GetFields(searchOptions.baseType); //lets get all fields

            foreach (FieldInfo f in allFields)
            {
                if (f.PrimaryKey || (!f.LazyLoading && !justCount)) //primary keys and eager fields must be loaded
                    fieldInfos.Add(f);

                // when paging, at least one sorting criterion is needed
                if (pagingStart.HasValue && pagingEnd.HasValue && searchOptions.Sorting.Count == 0 && f.PrimaryKey)
                    searchOptions.Sorting.Add(f.RelatedProperty.Name);
            }

            if (!justCount) //we don't need it on counting
            {
                SpecialFieldInfo[] fields = BLLBase.GetSpecialFields(searchOptions.baseType);
                if (searchOptions.Projections.Count > 0)
                {
                    fields = ReadProjections<SpecialFieldInfo>(projections, null, fields);
                }

                spFields.AddRange(fields);
            }

            sqlFields.Append(dialect.CreateSelectSql(table.RelatedTable, null, fieldInfos.ToArray(), spFields.ToArray()));


            //adding values from eager loading types
            foreach (ForeignKeyInfo fkInfo in searchOptions.eagerLoading.Keys)
            {
                fieldInfos = new List<FieldInfo>();
                spFields = new List<SpecialFieldInfo>();

                //select all fields, or only the projection.
                FieldInfo[] allEagerFields = BLLBase.GetFields(fkInfo.ElementType);
                if (searchOptions.Projections.Count > 0)
                    allEagerFields = ReadProjections<FieldInfo>(projections, fkInfo.RelatedProperty.Name, allEagerFields);

                foreach (FieldInfo f in allEagerFields)
                {
                    if (f.PrimaryKey || !f.LazyLoading)
                        fieldInfos.Add(f);
                }
                //spfields
                SpecialFieldInfo[] allSpFields = BLLBase.GetSpecialFields(fkInfo.ElementType);
                if (searchOptions.Projections.Count > 0)
                    allSpFields = ReadProjections<SpecialFieldInfo>(projections, fkInfo.RelatedProperty.Name, allSpFields);

                spFields.AddRange(allSpFields);
                //joins: joins was made on GetPlainJoins.

                sqlFields.Append(", ");
                sqlFields.Append(dialect.CreateSelectSql(fkInfo.ElementType, searchOptions.eagerLoading[fkInfo], fieldInfos.ToArray(), spFields.ToArray()));
            }


            //Sorting (not necessary for count queries)
            string sqlSort = string.Empty;

            if (!justCount)
            {
                string appendToSelect = null;

                sqlSort = dialect.CreateSortSql(searchOptions.Sorting, table.RelatedTable, joins.ToArray(), searchOptions.Distinct, pagingStart.HasValue && pagingEnd.HasValue, out appendToSelect);

                if (!string.IsNullOrEmpty(appendToSelect))
                    sqlFields.Append(appendToSelect);
            }

            //Check if we found all projections:
            if (projections.Count > 0)
                throw new InvalidProjectionException(projections[0]);


            //Creates the where part
            string sqlWhere = dialect.CreateWhereSql(searchOptions.Conditions, searchOptions.baseType, joins.ToArray(), out parameters);

            // Creates the join parts
            string sqlJoins = dialect.CreateJoinsSql(joins.ToArray());


            // Creates the entire sql string
            string sql = dialect.CreateFullSql(searchOptions.baseType, searchOptions.Distinct, justCount, searchOptions.Top, pagingStart, pagingEnd, sqlFields.ToString(), sqlJoins, sqlSort, sqlWhere);


            Tenor.Diagnostics.Debug.DebugSQL("GetSearchSql()", sql, parameters, connection);
#if DEBUG
            LastSearches.Push(sql);
#endif

            return sql;
        }

#if DEBUG
        internal static Stack<string> LastSearches = new Stack<string>();
#endif

        private static T[] ReadProjections<T>(IList<Projection> projections, string joinAlias, T[] allFields) where T : PropInfo
        {
            List<T> allFieldsList = new List<T>();
            for (int i = projections.Count - 1; i >= 0; i--)
            {
                Projection p = projections[i];
                if (p.JoinAlias == joinAlias)
                {
                    bool added = false;
                    foreach (T f in allFields)
                        if (f.RelatedProperty.Name == p.PropertyName)
                        {
                            added = true;
                            allFieldsList.Add(f);
                            break;
                        }
                    if (!added)
                        throw new InvalidProjectionException(p);
                    else
                        projections.RemoveAt(i);
                }
            }
            return allFieldsList.ToArray();
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

            ConnectionStringSettings connection = (tenorTransaction == null ? table.GetConnection() : tenorTransaction.Connection);
            GeneralDialect dialect = DialectFactory.CreateDialect(connection);

            TenorParameter[] parameters;
            DbTransaction t = (tenorTransaction == null ? null : tenorTransaction.dbTransaction);

            //if (dialect.GetType() == typeof(Tenor.Data.Dialects.TSql.TSql))
            //{
            //    //oh god! do you have a better idea on where to write this code?
            //    System.Data.SqlClient.SqlBulkCopy bulk;
            //    if (t == null)
            //        bulk = new System.Data.SqlClient.SqlBulkCopy(tenorTransaction.Connection.ConnectionString);
            //    else
            //        bulk = new System.Data.SqlClient.SqlBulkCopy((System.Data.SqlClient.SqlConnection)t.Connection, System.Data.SqlClient.SqlBulkCopyOptions.Default, (System.Data.SqlClient.SqlTransaction)t);

            //    bulk.DestinationTableName = dialect.GetPrefixAndTable(fkInfo.ManyToManyTablePrefix, fkInfo.ManyToManyTable);
            //    System.Data.DataTable data;
            //    string sql = dialect.CreateSaveList(table, fkInfo, this, out parameters, out data);
            //    foreach (DataColumn col in data.Columns)
            //    {
            //        bulk.ColumnMappings.Add(col.ColumnName, col.ColumnName);
            //    }

            //    Helper.ExecuteQuery(sql, parameters, t, dialect);
            //    bulk.WriteToServer(data);
            //    bulk.Close();
            //}
            //else
            //{
            string sql = dialect.CreateSaveListSql(table, fkInfo, this, out parameters);
            Helper.ExecuteQuery(sql, parameters, t, dialect);
            //}
        }
    }
}
