using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Data.Common;
using System.Configuration;
using System.Globalization;
using DataTable = Tenor.Data.DataTable;
using DbType = System.Data.DbType;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Collections.Specialized;
using Tenor.Data.Dialects;


namespace Tenor.BLL
{

    /// <summary>
    /// This is the base entity class. All of your class must inherit directly or indirectly from this
    /// class.
    /// Children classes gain functions to read and save data to a DataBase.
    /// </summary>
    /// <remarks>
    /// You can use the MyGeneration Template on file BLLBased.zeus to create your classes.
    /// </remarks>
    /// <seealso cref="zeus.htm"/> 
    [Serializable()]
    public abstract partial class BLLBase : object
    {
        internal Transaction transaction;

        #region "Ctors"
        private readonly string cacheKey;

        /// <summary>
        /// Initializes a new instance of the BLLBase class.
        /// </summary>
        public BLLBase()
        {
            cacheKey = Tenor.Configuration.TenorModule.IdPrefix + this.GetType().FullName;
        }

        /// <summary>
        /// Initializes a new instance of the BLLBase class. 
        /// </summary>
        /// <param name="lazyLoadingDisabled">On binding, if true, tries to load all the collections.</param>
        public BLLBase(bool lazyLoadingDisabled) : this()
        {
            _IsLazyDisabled = lazyLoadingDisabled;
        }
        #endregion

        #region " Search "


        /// <summary>
        /// Search data on the database based on definitions.
        /// </summary>
        /// <param name="searchOptions">An instance of SearchOptions with search definitions.</param>
        /// <returns>An array of entities.</returns>
        public static BLLBase[] Search(SearchOptions searchOptions)
        {
            return Search(searchOptions, null);
        }

        /// <summary>
        /// Count data on the database based on definitions.
        /// </summary>
        /// <param name="searchOptions">An instance of SearchOptions with search definitions.</param>
        /// <returns>The number os records found.</returns>
        /// <remarks></remarks>
        public static int Count(SearchOptions searchOptions)
        {
            return Count(searchOptions, null);
        }

        /// <summary>
        /// Search data on the database based on definitions.
        /// </summary>
        /// <param name="searchOptions">The search definitions.</param>
        /// <param name="connection">The Connection.</param>
        /// <returns>An array of entities.</returns>
        public static BLLBase[] Search(SearchOptions searchOptions, ConnectionStringSettings connection)
        {
            Tenor.Data.DataTable rs = SearchWithDataTable(searchOptions, connection);
            return BindRows(rs, searchOptions._BaseClass, searchOptions.LazyLoading);
        }

        /// <summary>
        /// Converts a datatable into a set of instances of baseClass.
        /// </summary>
        /// <param name="table">A System.Data.DataTable with raw database data.</param>
        /// <param name="lazyLoading">Indicates whether to enable the lazyLoading on created instances.</param>
        /// <param name="baseClass">Defines which type to load.</param>
        /// <exception cref="System.ArgumentNullException">Occurs when table or baseClass parameters are null.</exception>
        /// <returns>An array of entities.</returns>
        public static BLLBase[] BindRows(DataTable table, Type baseClass, bool lazyLoading)
        {
            if (table == null)
                throw new ArgumentNullException("table");
            if (baseClass == null)
                throw new ArgumentNullException("baseClass");
            if (!baseClass.IsSubclassOf(typeof(BLLBase)))
                throw new Tenor.BLL.InvalidTypeException(baseClass, "baseClass");

            BLLBase[] instances = (BLLBase[])(Array.CreateInstance(baseClass, table.Rows.Count));
            for (int i = 0; i <= instances.Length - 1; i++)
            {
                instances[i] = (BLLBase)(Activator.CreateInstance(baseClass));
                instances[i].Bind(lazyLoading, null, table.Rows[i]);
            }
            return instances;
        }


        /// <summary>
        /// Count data on the database based on definitions.
        /// </summary>
        /// <param name="searchOptions">An instance of SearchOptions with search definitions.</param>
        /// <param name="connection">The Connection.</param>
        /// <returns>The number os records found.</returns>
        /// <remarks></remarks>
        public static int Count(SearchOptions searchOptions, ConnectionStringSettings connection)
        {
            Tenor.Data.DataTable rs = SearchWithDataTable(searchOptions, connection, true);
            return System.Convert.ToInt32(rs.Rows[0][0]);
        }


        private static Tenor.Data.DataTable SearchWithDataTable(SearchOptions SearchOptions, ConnectionStringSettings Connection)
        {
            return SearchWithDataTable(SearchOptions, Connection, false);
        }

        private static Tenor.Data.DataTable SearchWithDataTable(SearchOptions searchOptions, ConnectionStringSettings connection, bool justCount)
        {

            TenorParameter[] parameters = null;
            if (connection == null)
            {
                TableInfo table = TableInfo.CreateTableInfo(searchOptions._BaseClass);
                connection = table.GetConnection();
            }

            string sql = GetSearchSql(searchOptions, justCount, connection, out parameters);
            Tenor.Data.DataTable rs = new Tenor.Data.DataTable(sql, parameters, connection);
            DataSet ds = new DataSet();
            ds.Tables.Add(rs);
            ds.EnforceConstraints = false;
            rs.Bind();
            return rs;
        }



        #endregion

        #region " Save "

        /// <summary>
        /// Saves this entity data on the persistence layer.
        /// </summary>
        /// <remarks></remarks>
        public virtual void Save()
        {
            bool isUpdate = true;
            foreach (FieldInfo field in GetFields(this.GetType(), true))
            {
                object valor = field.PropertyValue(this);
                double valorD = 0;

                if (valor == null)
                {
                    isUpdate = false;
                }
                else if (double.TryParse(valor.ToString(), out valorD))
                {
                    isUpdate = isUpdate && (valorD > 0);
                }
                if (!isUpdate)
                {
                    break;
                }
            }
            Save(isUpdate, null);
        }
        /// <summary>
        /// Saves this entity data on the persistence layer.
        /// </summary>
        /// <param name="isUpdate">Force an Update instead of an Insert statement.</param>
        /// <remarks></remarks>
        public virtual void Save(bool isUpdate)
        {
            Save(isUpdate, null);
        }
        [Obsolete]
        internal static string GetParamName(string paramPrefix, FieldInfo field)
        {
            return paramPrefix + field.DataFieldName.Replace(" ", "_");
        }


        /// <summary>
        /// Saves this entity data on the persistence layer.
        /// </summary>
        /// <param name="isUpdate">Force an Update instead of an Insert statement.</param>
        /// <remarks>This method calls the Validate method and only continue if True is returned. You can override the Validate method to create entity validation logic.</remarks>
        public virtual void Save(bool isUpdate, ConnectionStringSettings connection)
        {
            if (Validate())
            {
                TableInfo table = TableInfo.CreateTableInfo(this.GetType());
                
                if (connection == null)
                    connection = table.GetConnection();
                
                TenorParameter[] parameters = null;
                FieldInfo autoKeyField = null;

                GeneralDialect dialect = null;
                string query = GetSaveSql(isUpdate, connection, null, out autoKeyField, out parameters, out dialect);

                string secondQuery = string.Empty;

                if (!isUpdate && autoKeyField != null && !dialect.GetIdentityOnSameCommand
                    && !string.IsNullOrEmpty(dialect.IdentityAfterQuery))
                    secondQuery = string.Format(dialect.IdentityAfterQuery, autoKeyField.InsertSQL);

                object result;
                if (this.transaction != null && this.transaction.transaction != null)
                {
                    result = Helper.ExecuteQuery(query, parameters, transaction.transaction, dialect);

                    if (!string.IsNullOrEmpty(secondQuery))
                        result = Helper.ExecuteQuery(secondQuery, null, transaction.transaction, dialect);
                }
                else
                {
                    result = Helper.UpdateData(query, parameters, connection, secondQuery);
                }

                if (!isUpdate && autoKeyField != null)
                {
                    autoKeyField.SetPropertyValue(this, Convert.ChangeType(result, autoKeyField.FieldType));
                }
            }
        }

        /// <summary>
        /// Saves this entity data on the persistence layer only if the entity does not exists.
        /// </summary>
        /// <param name="conditionalProperty">The property name to check using its value.</param>
        public virtual bool SaveConditional(string conditionalProperty)
        {
            return SaveConditional(new string[] { conditionalProperty });
        }

        /// <summary>
        /// Saves this entity data on the persistence layer only if the entity does not exists.
        /// </summary>
        /// <param name="conditionalProperties">An array of properties to check using its values.</param>
        public virtual bool SaveConditional(string[] conditionalProperties)
        {
            return SaveConditional(conditionalProperties, false);
        }

        /// <summary>
        /// Saves this entity data on the persistence layer only if the entity does not exists when canUpdate is false. Otherwise, creates a new record or updates an existing one.
        /// </summary>
        /// <param name="conditionalProperty">The property name to check using its value.</param>
        /// <param name="isUpdate">If true, specifies that if a record existis, it will be updated. Otherwise, if a record exists, nothing will be done.</param>
        public virtual bool SaveConditional(string conditionalProperty, bool canUpdate)
        {
            return SaveConditional(new string[] { conditionalProperty }, canUpdate);
        }

        /// <summary>
        /// Saves this entity data on the persistence layer only if the entity does not exists when canUpdate is false. Otherwise, creates a new record or updates an existing one.
        /// </summary>
        /// <param name="conditionalProperties">An array of properties to check using its values.</param>
        /// <param name="isUpdate">If true, specifies that if a record existis, it will be updated. Otherwise, if a record exists, nothing will be done.</param>
        public virtual bool SaveConditional(string[] conditionalProperties, bool canUpdate)
        {
            return SaveConditional(conditionalProperties, canUpdate, null);
        }

        /// <summary>
        /// Checks if the entity already exists based on the properties passed and inserts if it doesn't. In case of canUpdate is true and the entity already exists, updates it to the current values.
        /// </summary>
        /// <param name="conditionalProperties">Properties to be considered when checking existence of the entity.</param>
        /// <param name="canUpdate">If true, specifies that if a record exists, it will be updated.</param>
        /// <param name="connection">The connection.</param>     
        public virtual bool SaveConditional(string[] conditionalProperties, bool canUpdate, ConnectionStringSettings connection)
        {
            if (Validate())
            {
                if (conditionalProperties == null || conditionalProperties.Length == 0)
                    throw new ArgumentNullException("conditionalProperties");

                TableInfo table = TableInfo.CreateTableInfo(this.GetType());
                if (connection == null)
                    connection = table.GetConnection();


                FieldInfo[] fields = BLLBase.GetFields(this.GetType(), null, conditionalProperties);
                if (fields.Length != conditionalProperties.Length)
                {
                    throw new MissingFieldsException(this.GetType(), true);
                    //throw (new ArgumentException("Cannot find one or more ConditionalFields", "conditionalProperties"));
                }


                FieldInfo[] fieldsPrimary = BLLBase.GetFields(this.GetType(), true);


                FieldInfo autoKeyField = null;

                TenorParameter[] parameters = null;

                GeneralDialect dialect = null;

                string insertQuery = GetSaveSql(false, connection, null, out autoKeyField, out parameters, out dialect);

                //updateQuery doesn't need parameters cause it's already set.\
                TenorParameter[] parameters2 = null;
                string updateQuery = GetSaveSql(true, connection, null, out autoKeyField, out parameters2, out dialect);
                parameters2 = null;

                string query = dialect.CreateConditionalSaveSql(insertQuery, updateQuery, conditionalProperties, fieldsPrimary);

                DataTable result = Helper.QueryData(connection, query.ToString(), parameters);

                if (!canUpdate && System.Convert.ToInt32(result.Rows[0][0]) == -1)
                {
                    return false;
                }
                else if (canUpdate && System.Convert.ToInt32(result.Rows[0][0]) == -1)
                {
                    return false;
                }
                else
                {
                    if (autoKeyField != null)
                    {
                        autoKeyField.SetPropertyValue(this, Convert.ChangeType(result.Rows[0][0], autoKeyField.FieldType));
                    }
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes this instance from the database.
        /// </summary>
        public void Delete()
        {

            ConditionCollection conditions = CreateDeleteConditions(this);


            TableInfo table = TableInfo.CreateTableInfo(this.GetType());
            ConnectionStringSettings connection = table.GetConnection();
            GeneralDialect dialect = DialectFactory.CreateDialect(connection);


            Join[] joins = GetPlainJoins(conditions, table.RelatedTable);
            
            TenorParameter[] parameters = null;
            string query = dialect.CreateDeleteSql(this.GetType(), conditions, joins, out parameters);

            if (this.transaction != null && this.transaction.transaction != null)
                Helper.ExecuteQuery(query, parameters, transaction.transaction, dialect);
            else
                Helper.UpdateData(query, parameters, connection);
        }

        /// <summary>
        /// Deletes these instances from the database.
        /// </summary>
        /// <param name="instances">An array of entities.</param>
        public static void Delete(BLLBase[] instances)
        {
            if (instances == null)
                throw new ArgumentNullException("instances");
            if (instances.Length == 0)
                return;

            Type type = null;
            Transaction transaction = null;

            List<Join> joinList = new List<Join>();


            ConditionCollection conditions = new ConditionCollection();
            foreach (BLLBase item in instances)
            {
                if (item == null)
                    throw new ArgumentNullException("instance");

                if (type == null)
                    type = item.GetType();
                else if (type != item.GetType())
                    throw new TenorException("You can have only items with the same type.");

                if (transaction == null)
                    transaction = item.transaction;
                else if (transaction != item.transaction)
                    throw new TenorException("You can have only items on the same transaction.");


                ConditionCollection cc = CreateDeleteConditions(item);

                if (conditions.Count > 0)
                    conditions.Add(LogicalOperator.Or);
                conditions.Add(cc);
                joinList.AddRange(GetPlainJoins(cc, item.GetType()));
            }

            TableInfo table = TableInfo.CreateTableInfo(type);
            ConnectionStringSettings connection = table.GetConnection();
            GeneralDialect dialect = DialectFactory.CreateDialect(connection);


            TenorParameter[] parameters = null;
            string query = dialect.CreateDeleteSql(type, conditions, joinList.ToArray(), out parameters);

            if (transaction != null && transaction.transaction != null)
                Helper.ExecuteQuery(query, parameters, transaction.transaction, dialect);
            else
                Helper.UpdateData(query, parameters, connection);


        }

        private static ConditionCollection CreateDeleteConditions(BLLBase instance)
        {
            ConditionCollection conditions = new ConditionCollection();

            foreach (FieldInfo i in GetFields(instance.GetType()))
            {
                if (i.PrimaryKey)
                {
                    if (conditions.Count != 0)
                        conditions.Add(LogicalOperator.And);
                    conditions.Add(i.RelatedProperty.Name, i.PropertyValue(instance));
                }
            }
            if (conditions.Count == 0)
            {
                throw (new Tenor.Data.MissingPrimaryKeyException(instance.GetType()));
            }

            return conditions;
        }

        #endregion

        #region " Localizable "
        /// <summary>
        /// When overriden, indicates that this entity can be localized automatically.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public virtual bool Localizable
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region "Find Functions"

        private static void SetFindDefinitions<t>(string PropertyExpression, object Value, Tenor.Data.CompareOperator @Operator) where t : BLLBase
        {
            //TODO: Consider coding an anonymous delegate.
            SetFindDefinitions<t>(new string[] { PropertyExpression }, new object[] { Value }, new Tenor.Data.CompareOperator[] { @Operator }, new Tenor.Data.LogicalOperator[0]);
        }

        private static void SetFindDefinitions<t>(string[] PropertyExpression, object[] Value, Tenor.Data.CompareOperator[] @Operator, Tenor.Data.LogicalOperator[] Logical) where t : BLLBase
        {
            //TODO: Consider coding an anonymous delegate.
            BLL.BLLCollection<t>._findProp = PropertyExpression;
            BLL.BLLCollection<t>._findValue = Value;
            BLL.BLLCollection<t>._findOperator = @Operator;
            BLL.BLLCollection<t>._findLogical = Logical;
        }

        /// <summary>
        /// Search for an object on the collection.
        /// </summary>
        /// <param name="items">An array of entities.</param>
        /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
        /// <param name="value">The value desired.</param>
        /// <returns>The first found object.</returns>
        public static T Find<T>(T[] items, string propertyExpression, object value) where T : BLLBase
        {
            return Find(items, propertyExpression, value, Tenor.Data.CompareOperator.Equal);
        }

        /// <summary>
        /// Search for an object on the collection.
        /// </summary>
        /// <param name="items">An array of entities.</param>
        /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
        /// <param name="value">The value desired.</param>
        /// <param name="compareOperator">The compare operator to use against the property.</param>
        /// <returns>The first found object.</returns>
        public static T Find<T>(T[] items, string propertyExpression, object value, Tenor.Data.CompareOperator compareOperator) where T : BLLBase
        {
            SetFindDefinitions<T>(propertyExpression, value, compareOperator);
            return Array.Find<T>(items, new Predicate<T>(BLLCollection<T>.FindDelegate));
        }


        /// <summary>
        /// Search for objects on the collection.
        /// </summary>
        /// <param name="items">An array of entities.</param>
        /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
        /// <param name="value">The value desired.</param>
        /// <returns>An array of objects.</returns>
        public static T[] FindAll<T>(T[] items, string propertyExpression, object value) where T : BLLBase
        {
            return FindAll(items, propertyExpression, value, Tenor.Data.CompareOperator.Equal);
        }

        /// <summary>
        /// Search for objects on the collection.
        /// </summary>
        /// <param name="items">An array of entities.</param>
        /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
        /// <param name="value">The value desired.</param>
        /// <param name="compareOperator">The compare operator to use against the property.</param>
        /// <returns>An array of objects.</returns>
        public static T[] FindAll<T>(T[] items, string propertyExpression, object value, Tenor.Data.CompareOperator compareOperator) where T : BLLBase
        {
            SetFindDefinitions<T>(propertyExpression, value, compareOperator);
            return Array.FindAll<T>(items, new Predicate<T>(BLLCollection<T>.FindDelegate));
        }

        /// <summary>
        /// Search for objects on the collection.
        /// </summary>
        /// <param name="items">An array of entities.</param>
        /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
        /// <param name="value">The value desired.</param>
        /// <param name="compareOperator">An array of compare operators used against the properties.</param>
        /// <param name="logicalOperators">An array of logical operators used to join the comparisons. Notice that this array must have one item less than the other arrays.</param>
        /// <returns>An array of objects.</returns>
        public static T[] FindAll<T>(T[] items, string[] propertyExpressions, object[] values, Tenor.Data.CompareOperator[] compareOperators, Tenor.Data.LogicalOperator[] logicalOperators) where T : BLLBase
        {
            SetFindDefinitions<T>(propertyExpressions, values, compareOperators, logicalOperators);
            return Array.FindAll<T>(items, new Predicate<T>(BLLCollection<T>.FindDelegate));
        }

        /// <summary>
        /// Search for an object on the collection.
        /// </summary>
        /// <param name="items">An array of entities.</param>
        /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
        /// <param name="value">The value desired.</param>
        /// <returns>The last found object.</returns>
        public static T FindLast<T>(T[] items, string propertyExpression, object value) where T : BLLBase
        {
            return FindLast(items, propertyExpression, value, Tenor.Data.CompareOperator.Equal);
        }

        /// <summary>
        /// Search for an object on the collection.
        /// </summary>
        /// <param name="items">An array of entities.</param>
        /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
        /// <param name="value">The value desired.</param>
        /// <param name="compareOperator">The compare operator to use against the property.</param>
        /// <returns>The last found object.</returns>
        public static T FindLast<T>(T[] items, string propertyExpression, object value, Tenor.Data.CompareOperator compareOperator) where T : BLLBase
        {
            SetFindDefinitions<T>(propertyExpression, value, compareOperator);
            return Array.FindLast<T>(items, new Predicate<T>(BLLCollection<T>.FindDelegate));
        }

        /// <summary>
        /// Search for an object on the collection.
        /// </summary>
        /// <param name="items">An array of entities.</param>
        /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
        /// <param name="value">The value desired.</param>
        /// <returns>The index of the first found object.</returns>
        public static int FindIndex<T>(T[] items, string propertyExpression, object value) where T : BLLBase
        {
            SetFindDefinitions<T>(propertyExpression, value, Tenor.Data.CompareOperator.Equal);
            return Array.FindIndex<T>(items, new Predicate<T>(BLLCollection<T>.FindDelegate));
        }

        /// <summary>
        /// Search for an object on the collection.
        /// </summary>
        /// <param name="items">An array of entities.</param>
        /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
        /// <param name="value">The value desired.</param>
        /// <returns>The index of the last found object.</returns>
        public static int FindLastIndex<T>(T[] items, string propertyExpression, object value) where T : BLLBase
        {
            SetFindDefinitions<T>(propertyExpression, value, Tenor.Data.CompareOperator.Equal);
            return Array.FindLastIndex<T>(items, new Predicate<T>(BLLCollection<T>.FindDelegate));
        }

        #region " Legacy Find Overloads"
        /// <summary>
        /// Search for an object on the collection.
        /// </summary>
        /// <param name="items">An array of entities.</param>
        /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
        /// <param name="value">The value desired.</param>
        /// <returns>The first found object.</returns>
        public static BLLBase Find(BLLBase[] items, string propertyExpression, object value)
        {
            return Find<BLLBase>(items, propertyExpression, value);
        }

        /// <summary>
        /// Search for an object on the collection.
        /// </summary>
        /// <param name="items">An array of entities.</param>
        /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
        /// <param name="value">The value desired.</param>
        /// <param name="compareOperator">The compare operator to use against the property.</param>
        /// <returns>The first found object.</returns>
        public static BLLBase Find(BLLBase[] items, string propertyExpression, object value, Tenor.Data.CompareOperator compareOperator)
        {
            return Find<BLLBase>(items, propertyExpression, value, compareOperator);
        }


        /// <summary>
        /// Search for objects on the collection.
        /// </summary>
        /// <param name="items">An array of entities.</param>
        /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
        /// <param name="value">The value desired.</param>
        /// <returns>An array of objects.</returns>
        public static BLLBase[] FindAll(BLLBase[] items, string propertyExpression, object value)
        {
            return FindAll<BLLBase>(items, propertyExpression, value);
        }

        /// <summary>
        /// Search for objects on the collection.
        /// </summary>
        /// <param name="items">An array of entities.</param>
        /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
        /// <param name="value">The value desired.</param>
        /// <param name="compareOperator">The compare operator to use against the property.</param>
        /// <returns>An array of objects.</returns>
        public static BLLBase[] FindAll(BLLBase[] items, string propertyExpression, object value, Tenor.Data.CompareOperator compareOperator)
        {
            return FindAll<BLLBase>(items, propertyExpression, value, compareOperator);
        }

        /// <summary>
        /// Search for an object on the collection.
        /// </summary>
        /// <param name="items">An array of entities.</param>
        /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
        /// <param name="value">The value desired.</param>
        /// <returns>The last found object.</returns>
        public static BLLBase FindLast(BLLBase[] items, string propertyExpression, object value)
        {
            return FindLast<BLLBase>(items, propertyExpression, value);
        }

        /// <summary>
        /// Search for an object on the collection.
        /// </summary>
        /// <param name="items">An array of entities.</param>
        /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
        /// <param name="value">The value desired.</param>
        /// <param name="compareOperator">The compare operator to use against the property.</param>
        /// <returns>The last found object.</returns>
        public static BLLBase FindLast(BLLBase[] items, string propertyExpression, object value, Tenor.Data.CompareOperator compareOperator)
        {
            return FindLast<BLLBase>(items, propertyExpression, value, compareOperator);
        }

        /// <summary>
        /// Search for an object on the collection.
        /// </summary>
        /// <param name="items">An array of entities.</param>
        /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
        /// <param name="value">The value desired.</param>
        /// <returns>The index of the first found object.</returns>
        public static int FindIndex(BLLBase[] items, string propertyExpression, object value)
        {
            return FindIndex<BLLBase>(items, propertyExpression, value);
        }

        /// <summary>
        /// Search for an object on the collection.
        /// </summary>
        /// <param name="items">An array of entities.</param>
        /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
        /// <param name="value">The value desired.</param>
        /// <returns>The index of the last found object.</returns>
        public static int FindLastIndex(BLLBase[] items, string propertyExpression, object value)
        {
            return FindLastIndex<BLLBase>(items, propertyExpression, value);
        }
        #endregion


        #endregion

        #region "Sort Functions"
        /// <summary>
        /// Sorts the entities on the collection based on a property.
        /// </summary>
        /// <param name="items">An array of entities.</param>
        /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
        public static void Sort(BLLBase[] items, string propertyExpression)
        {
            Array.Sort(items, new BLLBaseComparer(propertyExpression));
        }


        private class BLLBaseComparer : IComparer
        {

            private string _PropertyExpression;

            public BLLBaseComparer(string PropertyExpression)
            {
                //SetCompareDefinitions(PropertyExpression)
                _PropertyExpression = PropertyExpression;
            }

            public int Compare(object x, object y)
            {
                return Util.Compare(((BLLBase)x), ((BLLBase)y), _PropertyExpression);
            }
        }


        #endregion

        #region " Extra "

        /// <summary>
        /// Validates data before saving this instance. 
        /// You can override this to write your validation rules.
        /// </summary>
        protected virtual bool Validate()
        {
            return true;
        }
        #endregion

        #region " Operators "


        public override bool Equals(object obj)
        {
            BLLBase x = this;
            BLLBase y = obj as BLLBase;

            if (x == null && y == null)
            {
                return true;
            }
            else if (x == null || y == null)
            {
                return false;
            }
            else
            {
                Type t = x.GetType();
                if (t != y.GetType())
                {
                    return false;
                }
                FieldInfo[] primaryKeys = GetPrimaryKeys(t);

                bool res = true;

                foreach (FieldInfo p in primaryKeys)
                {
                    res = res && (p.PropertyValue(x).Equals(p.PropertyValue(y)));
                    if (!res)
                    {
                        break;
                    }
                }
                return res;
            }
        }

        public override int GetHashCode()
        {
            int hash = 57;
            foreach (FieldInfo i in GetPrimaryKeys(this.GetType()))
            {
                hash = 27 * hash * i.PropertyValue(this).GetHashCode();
            }
            return hash;
        }

        public static bool operator ==(BLLBase x, BLLBase y)
        {
            return object.Equals(x, y);
        }

        public static bool operator !=(BLLBase x, BLLBase y)
        {
            return !object.Equals(x, y);
        }
        #endregion






        #region Join Utility
        internal static Join[] GetPlainJoins(ConditionCollection conditions, Type baseClass)
        {
            List<Join> list = new List<Join>();
            GetPlainJoins(conditions, list);

            foreach (Join join in list)
            {
                if (join.ForeignKey == null)
                {
                    SetProperty(list, join, baseClass);
                }
            }
            return list.ToArray();
        }

        private static void GetPlainJoins(ConditionCollection conditions, List<Join> list)
        {
            foreach (Join join in conditions.includes)
            {
                if (list.Contains(join))
                    throw new InvalidOperationException("The join '" + join.JoinAlias + "' is already on the list.");
                else
                    list.Add(join);
            }
            foreach (object item in conditions)
            {
                if (item.GetType() == typeof(ConditionCollection))
                {
                    GetPlainJoins((ConditionCollection)item, list);
                }
            }
        }

        /// <summary>
        /// Sets the LocalTableInfo and ForeignTableInfo of all joins recursivelly.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="join"></param>
        /// <param name="baseClass"></param>
        private static void SetProperty(List<Join> list, Join join, Type baseClass)
        {
            if (join.LocalTableInfo == null)
            {
                if (string.IsNullOrEmpty(join.ParentAlias))
                {
                    join.LocalTableInfo = TableInfo.CreateTableInfo(baseClass);
                }
                else
                {
                    int pos = list.IndexOf(new Join(join.ParentAlias));
                    if (pos == -1)
                        throw new InvalidOperationException("Cannot find the parent alias for '" + join.JoinAlias + "'");
                    Join parent = list[pos];
                    if (parent.LocalTableInfo == null)
                        SetProperty(list, parent, baseClass);

                    join.LocalTableInfo = TableInfo.CreateTableInfo(parent.ForeignKey.ElementType);
                }
            }

            join.ForeignKey = ForeignKeyInfo.Create(join.LocalTableInfo.RelatedTable.GetProperty(join.PropertyName));
            if (join.ForeignKey == null)
                throw new InvalidOperationException("Cannot find '" + join.PropertyName + "' on '" + join.LocalTableInfo.RelatedTable.Name + "' class. You must define a ForeignKey on that class.");
            join.ForeignTableInfo = TableInfo.CreateTableInfo(join.ForeignKey.ElementType);
        }

        #endregion


    }
}