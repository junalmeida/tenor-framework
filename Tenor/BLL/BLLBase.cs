using System.Diagnostics;
using System;
using System.Collections;
using Microsoft.VisualBasic;
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
    /// You can use the MyGeneration Template on file BLLBases.zeus to create your classes.
    /// </remarks>
    /// <seealso cref="../zeus.htm"/> 
    [Serializable()]
    public abstract partial class BLLBase : object
    {
        internal Transaction transaction;

        #region "Ctors"

        private readonly string ChaveCache;

        public BLLBase()
        {
            ChaveCache = Tenor.Configuration.TenorModule.IdPrefix + this.GetType().FullName;
        }

        public BLLBase(bool LazyLoadingDisabled) : this()
        {
            _IsLazyDisabled = LazyLoadingDisabled;
        }

        #endregion

        #region " Search "




        /// <summary>
        /// Faz uma consulta especial e retorna uma coleção de objetos
        /// </summary>
        /// <param name="SearchOptions">Uma instância de SearchOptions com as informações da pesquisa.</param>
        /// <returns>Array de BLLBase</returns>
        /// <remarks></remarks>
        public static BLLBase[] Search(SearchOptions SearchOptions)
        {
            return Search(SearchOptions, null);
        }

        /// <summary>
        /// Faz uma consulta especial e retorna uma coleção de objetos
        /// </summary>
        /// <param name="SearchOptions">Uma instância de SearchOptions com as informações da pesquisa.</param>
        /// <returns>Array de BLLBase</returns>
        /// <remarks></remarks>
        public static int Count(SearchOptions SearchOptions)
        {
            return Count(SearchOptions, null);
        }

        /// <summary>
        /// Retorna o SQL gerado pelas condições.
        /// </summary>
        /// <param name="SearchOptions"></param>
        /// <param name="Connection">By ref</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GetSearchSql(SearchOptions searchOptions, bool justCount, ConnectionStringSettings connection, out TenorParameter[] parameters)
        {
            IDialect dialect = DialectFactory.CreateDialect(connection);

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
            FieldInfo[] fieldInfos = BLLBase.GetFields(searchOptions._BaseClass);
            SpecialFieldInfo[] spFields = BLLBase.GetSpecialFields(searchOptions._BaseClass);

            string sqlFields = dialect.CreateSelectSql(table.RelatedTable, fieldInfos, spFields);


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
        /// Faz uma consulta especial e retorna uma coleção de objetos
        /// </summary>
        /// <param name="SearchOptions">Uma instância de SearchOptions com as informações da pesquisa.</param>
        /// <returns>Array de BLLBase</returns>
        /// <remarks></remarks>
        public static BLLBase[] Search(SearchOptions SearchOptions, ConnectionStringSettings Connection)
        {
            Tenor.Data.DataTable rs = SearchWithDataTable(SearchOptions, Connection);
            return Search(rs, SearchOptions._BaseClass, SearchOptions.LazyLoading, Connection);
        }

        public static BLLBase[] Search(DataTable Table, Type BaseClass, bool LazyLoading, ConnectionStringSettings Connection)
        {
            BLLBase[] instances = (BLLBase[])(Array.CreateInstance(BaseClass, Table.Rows.Count));
            for (int i = 0; i <= instances.Length - 1; i++)
            {
                instances[i] = (BLLBase)(Activator.CreateInstance(BaseClass));
                instances[i].Bind(LazyLoading, null, Table.Rows[i]);
            }
            return instances;
        }


        /// <summary>
        /// Faz uma consulta especial e retorna uma coleção de objetos
        /// </summary>
        /// <param name="SearchOptions">Uma instância de SearchOptions com as informações da pesquisa.</param>
        /// <returns>Array de BLLBase</returns>
        /// <remarks></remarks>
        public static int Count(SearchOptions SearchOptions, ConnectionStringSettings Connection)
        {
            Tenor.Data.DataTable rs = SearchWithDataTable(SearchOptions, Connection, true);
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
        /// 
        /// </summary>
        /// <param name="isUpdate"></param>
        /// <param name="parameters"></param>
        /// <param name="autoKeyField"></param>
        /// <param name="specialValues">The special values can contains sql sentences/sequences/etc</param>
        /// <param name="connection"></param>
        /// <returns></returns>
        internal string GetSaveSql(bool isUpdate, ConnectionStringSettings connection, NameValueCollection specialValues, out FieldInfo autoKeyField, out TenorParameter[] parameters, out IDialect dialect)
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
            string sql = dialect.CreateSaveSql(this.GetType(), data, specialValues, conditions, out parameters);


            return sql;
        }

        /// <summary>
        /// Saves this entity data on the persistence layer.
        /// </summary>
        /// <param name="isUpdate">Force an Update instead of an Insert statement.</param>
        /// <remarks></remarks>
        public virtual void Save(bool isUpdate, ConnectionStringSettings connection)
        {
            if (Validate())
            {
                TableInfo table = TableInfo.CreateTableInfo(this.GetType());
                if (connection == null)
                    connection = table.GetConnection();
                TenorParameter[] parameters = null;
                FieldInfo autoKeyField = null;
                string identityQuery = null;
                bool runOnSameQuery = false;

                IDialect dialect = null;
                string query = GetSaveSql(isUpdate, connection, null, out autoKeyField, out parameters, out dialect);

                int result;
                if (this.transaction != null && this.transaction.transaction != null)
                    result = Helper.ExecuteQuery(query, parameters, transaction.transaction, dialect);
                else
                    result = Helper.ExecuteQuery(query, parameters, connection);

                if (!isUpdate && (autoKeyField != null))
                {
                    autoKeyField.SetPropertyValue(this, Convert.ChangeType(result, autoKeyField.FieldType));
                }
            }
        }

        /// <summary>
        /// Somente salva a instancia se o Campo Condicional não existir.
        /// </summary>
        public virtual bool SaveConditional(string conditionalField)
        {
            return SaveConditional(new string[] { conditionalField });
        }

        /// <summary>
        /// Somente salva a instancia se o Campo Condicional não existir.
        /// </summary>
        public virtual bool SaveConditional(string[] conditionalFields)
        {
            return SaveConditional(conditionalFields, false);
        }

        /// <summary>
        /// Salva ou atualiza a instancia de acordo com isUpdate
        /// </summary>
        public virtual bool SaveConditional(string conditionalField, bool isUpdate)
        {
            return SaveConditional(new string[] { conditionalField }, isUpdate);
        }

                /// <summary>
        /// Quando isUpdate é False, o sistema cria um registro novo, se o mesmo não existir. Se o registro for criado,
        /// retorna True.
        /// Quando isUpdate é True, o sistema cria um registro novo se o mesmo não exister, caso contrário, atualiza.
        /// Se o registro for criado, retorna True.
        /// </summary>
        public virtual bool SaveConditional(string[] conditionalFields, bool isUpdate)
        {
            return SaveConditional(conditionalFields, isUpdate, null);
        }
        /// <summary>
        /// Quando isUpdate é False, o sistema cria um registro novo, se o mesmo não existir. Se o registro for criado,
        /// retorna True.
        /// Quando isUpdate é True, o sistema cria um registro novo se o mesmo não exister, caso contrário, atualiza.
        /// Se o registro for criado, retorna True.
        /// </summary>
        public virtual bool SaveConditional(string[] conditionalFields, bool isUpdate, ConnectionStringSettings connection)
        {
            if (Validate())
            {
                TableInfo table = TableInfo.CreateTableInfo(this.GetType());
                if (connection == null)
                    connection = table.GetConnection();
                

                FieldInfo[] fields = BLLBase.GetFields(this.GetType(), null, conditionalFields);
                if (conditionalFields.Length == 0)
                {
                    throw (new ArgumentException("Cannot find one or more ConditionalFields", "ConditionalFields"));
                }
                else if (fields.Length != conditionalFields.Length)
                {
                    throw (new ArgumentException("Cannot find one or more ConditionalFields", "ConditionalFields"));
                }

                FieldInfo[] fieldsPrimary = BLLBase.GetFields(this.GetType(), true);


                FieldInfo autoKeyField = null;

                TenorParameter[] parameters = null;
                string identityQuery = null;
                bool runOnSameQuery = false;

                IDialect dialect = null;

                string insertQuery = GetSaveSql(false, connection, null, out autoKeyField, out parameters, out dialect);

                //updateQuery doesn't need parameters cause it's already set.\
                TenorParameter[] parameters2 = null;
                string updateQuery = GetSaveSql(true, connection, null, out autoKeyField, out parameters2, out dialect);
                parameters2 = null;

                string query = dialect.CreateConditionalSaveSql(insertQuery, updateQuery, conditionalFields, fieldsPrimary);

                DataTable result = Helper.ConsultarBanco(connection, query.ToString(), parameters);

                if (!isUpdate && System.Convert.ToInt32(result.Rows[0][0]) == -1)
                {
                    return false;
                }
                else if (isUpdate && System.Convert.ToInt32(result.Rows[0][0]) == -1)
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
            IDialect dialect = DialectFactory.CreateDialect(connection);


            Join[] joins = GetPlainJoins(conditions, table.RelatedTable);
            
            TenorParameter[] parameters = null;
            string query = dialect.CreateDeleteSql(this.GetType(), conditions, joins, out parameters);

            if (this.transaction != null && this.transaction.transaction != null)
                Helper.ExecuteQuery(query, parameters, transaction.transaction, dialect);
            else
                Helper.ExecuteQuery(query, parameters, connection);
        }

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
            IDialect dialect = DialectFactory.CreateDialect(connection);


            TenorParameter[] parameters = null;
            string query = dialect.CreateDeleteSql(type, conditions, joinList.ToArray(), out parameters);

            if (transaction != null && transaction.transaction != null)
                Helper.ExecuteQuery(query, parameters, transaction.transaction, dialect);
            else
                Helper.ExecuteQuery(query, parameters, connection);


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
            SetFindDefinitions<t>(new string[] { PropertyExpression }, new object[] { Value }, new Tenor.Data.CompareOperator[] { @Operator }, new Tenor.Data.LogicalOperator[0]);
        }

        private static void SetFindDefinitions<t>(string[] PropertyExpression, object[] Value, Tenor.Data.CompareOperator[] @Operator, Tenor.Data.LogicalOperator[] Logical) where t : BLLBase
        {
            BLL.BLLCollection<t>._findProp = PropertyExpression;
            BLL.BLLCollection<t>._findValue = Value;
            BLL.BLLCollection<t>._findOperator = @Operator;
            BLL.BLLCollection<t>._findLogical = Logical;
        }

        /// <summary>
        /// Realiza uma busca nos objetos desta coleção
        /// </summary>
        /// <param name="Items">Array com a coleção dos itens</param>
        /// <param name="PropertyExpression">Nome da propriedade do tipo usada para a busca</param>
        /// <param name="Value">Valor usado para a busca</param>
        /// <returns>O primeiro objeto encontrado</returns>
        /// <remarks></remarks>
        public static t Find<t>(t[] Items, string PropertyExpression, object Value) where t : BLLBase
        {
            return Find(Items, PropertyExpression, Value, Tenor.Data.CompareOperator.Equal);
        }

        /// <summary>
        /// Realiza uma busca nos objetos desta coleção
        /// </summary>
        /// <param name="Items">Array com a coleção dos itens</param>
        /// <param name="PropertyExpression">Nome da propriedade do tipo usada para a busca</param>
        /// <param name="Value">Valor usado para a busca</param>
        /// <param name="Operator">Operador de comparação para a busca</param>
        /// <returns>O primeiro objeto encontrado</returns>
        /// <remarks></remarks>
        public static T Find<T>(T[] Items, string PropertyExpression, object Value, Tenor.Data.CompareOperator @Operator) where T : BLLBase
        {
            SetFindDefinitions<T>(PropertyExpression, Value, @Operator);
            return Array.Find<T>(Items, new Predicate<T>(BLLCollection<T>.FindDelegate));
        }


        /// <summary>
        /// Realiza uma busca nos objetos e retorna todos os itens encontrados
        /// </summary>
        /// <param name="Items">Array com a coleção dos itens</param>
        /// <param name="PropertyExpression">Nome da propriedade do tipo usada para a busca</param>
        /// <param name="Value">Valor usado para a busca</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static T[] FindAll<T>(T[] Items, string PropertyExpression, object Value) where T : BLLBase
        {
            return FindAll(Items, PropertyExpression, Value, Tenor.Data.CompareOperator.Equal);
        }

        /// <summary>
        /// Realiza uma busca nos objetos e retorna todos os itens encontrados
        /// </summary>
        /// <param name="Items">Array com a coleção dos itens</param>
        /// <param name="PropertyExpression">Nome da propriedade do tipo usada para a busca</param>
        /// <param name="Value">Valor usado para a busca</param>
        /// <param name="Operator">Operador de comparação para a busca</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static T[] FindAll<T>(T[] Items, string PropertyExpression, object Value, Tenor.Data.CompareOperator @Operator) where T : BLLBase
        {
            SetFindDefinitions<T>(PropertyExpression, Value, @Operator);
            return Array.FindAll<T>(Items, new Predicate<T>(BLLCollection<T>.FindDelegate));
        }

        /// <summary>
        /// Realiza uma busca nos objetos e retorna todos os itens encontrados. Esta busca permite que sejam especifícados mais de uma condição de pesquisa.
        /// </summary>
        /// <param name="PropertyExpressions">Um Array de caminhos para propriedade.s</param>
        /// <param name="Values">Array de valores usados na comparação.</param>
        /// <param name="Operators">Array de Operadores de comparação usados para comparar o conteúdo da propriedade com o valor fornecido.</param>
        /// <param name="LogicalOperators">Array de operadores lógicos para a união das condições de pesquisa. Este array deverá ter um item a menos que os outros arrays.</param>
        /// <returns>Um Array de elementos que satisfazem à busca</returns>
        /// <remarks></remarks>
        public static T[] FindAll<T>(T[] Items, string[] PropertyExpressions, object[] Values, Tenor.Data.CompareOperator[] Operators, Tenor.Data.LogicalOperator[] LogicalOperators) where T : BLLBase
        {
            SetFindDefinitions<T>(PropertyExpressions, Values, Operators, LogicalOperators);
            return Array.FindAll<T>(Items, new Predicate<T>(BLLCollection<T>.FindDelegate));
        }

        /// <summary>
        /// Realiza uma busca nos objetos desta coleção.
        /// </summary>
        /// <param name="Items">Array com a coleção dos itens</param>
        /// <param name="PropertyExpression">Nome da propriedade do tipo usada para a busca</param>
        /// <param name="Value">Valor usado para a busca</param>
        /// <returns>O último objeto encontrado</returns>
        /// <remarks></remarks>
        public static T FindLast<T>(T[] Items, string PropertyExpression, object Value) where T : BLLBase
        {
            return FindLast(Items, PropertyExpression, Value, Tenor.Data.CompareOperator.Equal);
        }

        /// <summary>
        /// Realiza uma busca nos objetos desta coleção.
        /// </summary>
        /// <param name="Items">Array com a coleção dos itens</param>
        /// <param name="PropertyExpression">Nome da propriedade do tipo usada para a busca</param>
        /// <param name="Value">Valor usado para a busca</param>
        /// <param name="Operator">Operador de comparação para a busca</param>
        /// <returns>O último objeto encontrado</returns>
        /// <remarks></remarks>
        public static T FindLast<T>(T[] Items, string PropertyExpression, object Value, Tenor.Data.CompareOperator @Operator) where T : BLLBase
        {
            SetFindDefinitions<T>(PropertyExpression, Value, @Operator);
            return Array.FindLast<T>(Items, new Predicate<T>(BLLCollection<T>.FindDelegate));
        }

        /// <summary>
        /// Realiza uma busca nos objetos desta coleção.
        /// </summary>
        /// <param name="Items">Array com a coleção dos itens</param>
        /// <param name="PropertyExpression">Nome da propriedade do tipo usada para a busca</param>
        /// <param name="Value">Valor usado para a busca</param>
        /// <returns>O índice do primeiro objeto encontrado</returns>
        /// <remarks></remarks>
        public static int FindIndex<T>(T[] Items, string PropertyExpression, object Value) where T : BLLBase
        {
            SetFindDefinitions<T>(PropertyExpression, Value, Tenor.Data.CompareOperator.Equal);
            return Array.FindIndex<T>(Items, new Predicate<T>(BLLCollection<T>.FindDelegate));
        }

        /// <summary>
        /// Realiza uma busca nos objetos desta coleção.
        /// </summary>
        /// <param name="PropertyExpression">Nome da propriedade do tipo usada para a busca</param>
        /// <param name="Value">Valor usado para a busca</param>
        /// <returns>O índice do último objeto encontrado</returns>
        /// <remarks></remarks>
        public static int FindLastIndex<T>(T[] Items, string PropertyExpression, object Value) where T : BLLBase
        {
            SetFindDefinitions<T>(PropertyExpression, Value, Tenor.Data.CompareOperator.Equal);
            return Array.FindLastIndex<T>(Items, new Predicate<T>(BLLCollection<T>.FindDelegate));
        }

        #region " Overloads de legacy "
        /// <summary>
        /// Realiza uma busca nos objetos desta coleção
        /// </summary>
        /// <param name="Items">Array com a coleção dos itens</param>
        /// <param name="PropertyExpression">Nome da propriedade do tipo usada para a busca</param>
        /// <param name="Value">Valor usado para a busca</param>
        /// <returns>O primeiro objeto encontrado</returns>
        /// <remarks></remarks>
        public static BLLBase Find(BLLBase[] Items, string PropertyExpression, object Value)
        {
            return Find<BLLBase>(Items, PropertyExpression, Value);
        }

        /// <summary>
        /// Realiza uma busca nos objetos desta coleção
        /// </summary>
        /// <param name="Items">Array com a coleção dos itens</param>
        /// <param name="PropertyExpression">Nome da propriedade do tipo usada para a busca</param>
        /// <param name="Value">Valor usado para a busca</param>
        /// <param name="Operator">Operador de comparação para a busca</param>
        /// <returns>O primeiro objeto encontrado</returns>
        /// <remarks></remarks>
        public static BLLBase Find(BLLBase[] Items, string PropertyExpression, object Value, Tenor.Data.CompareOperator @Operator)
        {
            return Find<BLLBase>(Items, PropertyExpression, Value, @Operator);
        }


        /// <summary>
        /// Realiza uma busca nos objetos e retorna todos os itens encontrados
        /// </summary>
        /// <param name="Items">Array com a coleção dos itens</param>
        /// <param name="PropertyExpression">Nome da propriedade do tipo usada para a busca</param>
        /// <param name="Value">Valor usado para a busca</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static BLLBase[] FindAll(BLLBase[] Items, string PropertyExpression, object Value)
        {
            return FindAll<BLLBase>(Items, PropertyExpression, Value);
        }

        /// <summary>
        /// Realiza uma busca nos objetos e retorna todos os itens encontrados
        /// </summary>
        /// <param name="Items">Array com a coleção dos itens</param>
        /// <param name="PropertyExpression">Nome da propriedade do tipo usada para a busca</param>
        /// <param name="Value">Valor usado para a busca</param>
        /// <param name="Operator">Operador de comparação para a busca</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static BLLBase[] FindAll(BLLBase[] Items, string PropertyExpression, object Value, Tenor.Data.CompareOperator @Operator)
        {
            return FindAll<BLLBase>(Items, PropertyExpression, Value, @Operator);
        }

        /// <summary>
        /// Realiza uma busca nos objetos desta coleção.
        /// </summary>
        /// <param name="Items">Array com a coleção dos itens</param>
        /// <param name="PropertyExpression">Nome da propriedade do tipo usada para a busca</param>
        /// <param name="Value">Valor usado para a busca</param>
        /// <returns>O último objeto encontrado</returns>
        /// <remarks></remarks>
        public static BLLBase FindLast(BLLBase[] Items, string PropertyExpression, object Value)
        {
            return FindLast<BLLBase>(Items, PropertyExpression, Value);
        }

        /// <summary>
        /// Realiza uma busca nos objetos desta coleção.
        /// </summary>
        /// <param name="Items">Array com a coleção dos itens</param>
        /// <param name="PropertyExpression">Nome da propriedade do tipo usada para a busca</param>
        /// <param name="Value">Valor usado para a busca</param>
        /// <param name="Operator">Operador de comparação para a busca</param>
        /// <returns>O último objeto encontrado</returns>
        /// <remarks></remarks>
        public static BLLBase FindLast(BLLBase[] Items, string PropertyExpression, object Value, Tenor.Data.CompareOperator @Operator)
        {
            return FindLast<BLLBase>(Items, PropertyExpression, Value, @Operator);
        }

        /// <summary>
        /// Realiza uma busca nos objetos desta coleção.
        /// </summary>
        /// <param name="Items">Array com a coleção dos itens</param>
        /// <param name="PropertyExpression">Nome da propriedade do tipo usada para a busca</param>
        /// <param name="Value">Valor usado para a busca</param>
        /// <returns>O índice do primeiro objeto encontrado</returns>
        /// <remarks></remarks>
        public static int FindIndex(BLLBase[] Items, string PropertyExpression, object Value)
        {
            return FindIndex<BLLBase>(Items, PropertyExpression, Value);
        }

        /// <summary>
        /// Realiza uma busca nos objetos desta coleção.
        /// </summary>
        /// <param name="PropertyExpression">Nome da propriedade do tipo usada para a busca</param>
        /// <param name="Value">Valor usado para a busca</param>
        /// <returns>O índice do último objeto encontrado</returns>
        /// <remarks></remarks>
        public static int FindLastIndex(BLLBase[] Items, string PropertyExpression, object Value)
        {
            return FindLastIndex<BLLBase>(Items, PropertyExpression, Value);
        }
        #endregion


        #endregion

        #region "Sort Functions"
        /// <summary>
        /// Realiza ordenação dos elementos de acordo com a expressão fornecida.
        /// </summary>
        /// <param name="Items">Array de elementos à ordenar</param>
        /// <param name="PropertyExpression">
        /// Uma PropertyExpression para usar na ordenação. Você pode especificar mais de uma expressão separando-as por vírgula (,)
        /// </param>
        /// <remarks></remarks>
        public static void Sort(BLLBase[] Items, string PropertyExpression)
        {
            Array.Sort(Items, new BLLBaseComparer(PropertyExpression));
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


        //Private __EmAprova As Boolean
        //<Field(autonumber:=False, FieldName:="_EmAprova", PrimaryKey:=False)> _
        //Public Property _EmAprova() As Boolean
        //    Get
        //        Return __EmAprova
        //    End Get
        //    Set(ByVal value As Boolean)
        //        __EmAprova = value
        //    End Set
        //End Property




        //        /* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
        //BEGIN TRANSACTION
        //SET QUOTED_IDENTIFIER ON
        //SET ARITHABORT ON
        //SET NUMERIC_ROUNDABORT OFF
        //SET CONCAT_NULL_YIELDS_NULL ON
        //SET ANSI_NULLS ON
        //SET ANSI_PADDING ON
        //SET ANSI_WARNINGS ON
        //COMMIT
        //BEGIN TRANSACTION
        //  ALTER TABLE dbo.SituacaoObra ADD   _EmAprova bit NOT NULL CONSTRAINT DF_SituacaoObra__EmAprova DEFAULT 0  GO
        //  ALTER TABLE dbo.TipoImagem ADD   _EmAprova bit NOT NULL CONSTRAINT DF_TipoImagem__EmAprova DEFAULT 0  GO
        //  ALTER TABLE dbo.Grupo ADD   _EmAprova bit NOT NULL CONSTRAINT DF_Grupo__EmAprova DEFAULT 0  GO
        //  ALTER TABLE dbo.Bloco ADD   _EmAprova bit NOT NULL CONSTRAINT DF_Bloco__EmAprova DEFAULT 0  GO
        //  ALTER TABLE dbo.Cronograma ADD   _EmAprova bit NOT NULL CONSTRAINT DF_Cronograma__EmAprova DEFAULT 0  GO
        //  ALTER TABLE dbo.Unidade ADD   _EmAprova bit NOT NULL CONSTRAINT DF_Unidade__EmAprova DEFAULT 0  GO
        //  ALTER TABLE dbo.Usuario ADD   _EmAprova bit NOT NULL CONSTRAINT DF_Usuario__EmAprova DEFAULT 0  GO
        //  ALTER TABLE dbo.UsuarioGrupo ADD   _EmAprova bit NOT NULL CONSTRAINT DF_UsuarioGrupo__EmAprova DEFAULT 0  GO
        //  ALTER TABLE dbo.Log ADD   _EmAprova bit NOT NULL CONSTRAINT DF_Log__EmAprova DEFAULT 0  GO
        //  ALTER TABLE dbo.ItemMarketing ADD   _EmAprova bit NOT NULL CONSTRAINT DF_ItemMarketing__EmAprova DEFAULT 0  GO
        //  ALTER TABLE dbo.ValorMarketing ADD   _EmAprova bit NOT NULL CONSTRAINT DF_ValorMarketing__EmAprova DEFAULT 0  GO
        //  ALTER TABLE dbo.ValorCronograma ADD   _EmAprova bit NOT NULL CONSTRAINT DF_ValorCronograma__EmAprova DEFAULT 0  GO
        //  ALTER TABLE dbo.Empreendimento ADD   _EmAprova bit NOT NULL CONSTRAINT DF_Empreendimento__EmAprova DEFAULT 0  GO
        //  ALTER TABLE dbo.TipoEmpreendimento ADD   _EmAprova bit NOT NULL CONSTRAINT DF_TipoEmpreendimento__EmAprova DEFAULT 0  GO
        //  ALTER TABLE dbo.MarketingEmpreendimento ADD   _EmAprova bit NOT NULL CONSTRAINT DF_MarketingEmpreendimento__EmAprova DEFAULT 0  GO
        //  ALTER TABLE dbo.Imagem ADD   _EmAprova bit NOT NULL CONSTRAINT DF_Imagem__EmAprova DEFAULT 0  GO
        //  ALTER TABLE dbo.ItemCronograma ADD   _EmAprova bit NOT NULL CONSTRAINT DF_ItemCronograma__EmAprova DEFAULT 0  GO
        //  ALTER TABLE dbo.Disponibilidade ADD   _EmAprova bit NOT NULL CONSTRAINT DF_Disponibilidade__EmAprova DEFAULT 0  GO
        //COMMIT


        //select '
        //ALTER TABLE dbo.' + name + ' ADD
        //	_EmAprova bit NOT NULL CONSTRAINT DF_' + name + '__EmAprova DEFAULT 0
        //GO
        //        ' from SYSOBJECTS WHERE xtype='U'
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