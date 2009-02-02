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

        private static string ReadSelectFields(string tableAlias, DbCommandBuilder builder, FieldInfo[] Fields, SpecialFieldInfo[] SpFields)
        {
            string campos = "";
            foreach (FieldInfo f in Fields)
            {
                if (f.PrimaryKey || !f.LazyLoading)
                {
                    //determina o nome do campo
                    campos += ", " + tableAlias + "." + builder.QuoteIdentifier(f.DataFieldName);
                }
            }
            if (campos.Length < 2)
            {
                throw (new InvalidOperationException("Cannot find any TableFields for loading this type"));
            }

            foreach (SpecialFieldInfo f in SpFields)
            {
                campos += ", (" + string.Format(f.Expression, tableAlias) + ") " + f.Alias;
            }

            return campos.Substring(2);
        }


        /// <summary>
        /// Faz o carregamento das condições com parametros e inner joins possíveis.
        /// </summary>
        /// <param name="Conditions">Uma instância com as informações da pesquisa.</param>
        /// <param name="params"></param>
        /// <param name="innerjoins"></param>
        /// <param name="BaseClass"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static string ReadConditions(ConditionCollection Conditions, List<TenorParameter> @params, ref Dictionary<string, Type> innerjoins, Type BaseClass, ConnectionStringSettings Connection)
        {
            if (Conditions == null || Conditions.Count == 0)
            {
                return "";
            }

            System.Text.StringBuilder sqlWHERE = new System.Text.StringBuilder();


            //Monta a WHERE
            for (int i = 0; i <= Conditions.Count - 1; i++)
            {
                object obj = Conditions[i];

                if (obj.GetType() == typeof(SearchCondition))
                {
                    SearchCondition sc = (SearchCondition)obj;
                    if (sc.Table == null)
                    {
                        sc._Table = BaseClass;
                    }

                    sqlWHERE.Append(sc.ToString(Connection));

                    bool containsKey = innerjoins.ContainsKey(sc.TableAlias);

                    if (containsKey && !(innerjoins[sc.TableAlias] == sc.Table))
                    {
                        throw (new InvalidOperationException("You cannot use the same table alias for different types."));
                    }
                    else if (!containsKey && !(sc.Table == BaseClass))
                    {
                        innerjoins.Add(sc.TableAlias, sc.Table);
                    }

                    if (sc.Value != null)
                    {
                        @params.Add(new TenorParameter(sc.ParameterName, sc.Value));
                    }
                }
                else if (obj.GetType() == typeof(LogicalOperator))
                {
                    LogicalOperator lOp = (LogicalOperator)obj;
                    switch (lOp)
                    {
                        case Tenor.Data.LogicalOperator.And:
                            sqlWHERE.AppendLine(" AND ");
                            break;
                        case Tenor.Data.LogicalOperator.Or:
                            sqlWHERE.AppendLine(" OR ");
                            break;
                    }

                    if (i == Conditions.Count - 1)
                    {
                        throw (new ArgumentException("Cannot have a collection that ends with an operator."));
                    }

                }
                else if (obj.GetType() == typeof(ConditionCollection))
                {
                    string parenteses = ReadConditions(((ConditionCollection)obj), @params, ref innerjoins, BaseClass, Connection);
                    if (string.IsNullOrEmpty(parenteses))
                    {
                        throw (new ArgumentException("Cannot have null or empty ConditionCollecion"));
                    }
                    sqlWHERE.AppendLine(" ( " + parenteses + " ) ");

                }

            }

            return sqlWHERE.ToString();
        }

        /// <summary>
        /// Faz o carregamento das classes relacionadas para inner join
        /// </summary>
        private static Dictionary<string, Type> ReadIncludes(ConditionCollection Conditions)
        {
            Dictionary<string, Type> dict = new Dictionary<string, Type>();
            foreach (KeyValuePair<string, Type> i in Conditions.Includes)
            {
                if (!dict.ContainsKey(i.Key))
                {
                    dict.Add(i.Key, i.Value);
                }
            }

            foreach (object i in Conditions)
            {
                if ((i) is ConditionCollection)
                {
                    foreach (KeyValuePair<string, Type> j in ReadIncludes((ConditionCollection)i))
                    {
                        if (!dict.ContainsKey(j.Key))
                        {
                            dict.Add(j.Key, j.Value);
                        }
                    }
                }
            }

            return dict;
        }



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
        public static string GetSearchSql(SearchOptions SearchOptions, ref List<TenorParameter> Params, bool Count, ConnectionStringSettings Connection)
        {
            if (@Params == null)
            {
                throw (new ArgumentNullException("Params", "You must specify a List Of Parameter."));
            }
            @Params.Clear();



            if (SearchOptions == null)
            {
                throw (new ArgumentNullException("SearchOptions", "You must specify a SearchOptions instance."));
            }

            TableInfo table = TableInfo.CreateTableInfo(SearchOptions._BaseClass);
      //BLLBase instance = (BLLBase)(Activator.CreateInstance(SearchOptions._BaseClass));


            if (Connection == null)
            {
                Connection = table.GetConnection();
            }

            
            DbCommandBuilder builder = Helper.GetCommandBuilder(Connection);
            
            
            //Get necessary fields to create the select statement.
            FieldInfo[] fields = BLLBase.GetFields(SearchOptions._BaseClass);
            SpecialFieldInfo[] spfields = BLLBase.GetSpecialFields(SearchOptions._BaseClass);
            string campos = ReadSelectFields(table.GetTableAlias(), builder, fields, spfields);

            Dictionary<string, Type> innerjoins = new Dictionary<string, Type>();

            //Força inner joins sem busca
            foreach (KeyValuePair<string, Type> i in ReadIncludes(SearchOptions.Conditions))
            {
                if (!innerjoins.ContainsKey(i.Key))
                {
                    innerjoins.Add(i.Key, i.Value);
                }
            }

            //Sorting
            StringBuilder sqlSort = new StringBuilder();

            foreach (SortingCriteria sort in SearchOptions.Sorting)
            {
                if (sort.Table == null)
                {
                    sort._Table = SearchOptions._BaseClass;
                }
    
                if (sort.FieldInfo == null && sort.SpecialFieldInfo == null)
                {
                    throw (new ArgumentException("Invalid Property \'" + sort.Property + "\'. You must define a Field or a SpecialField property item on SortCriteria class.", "Property", null));
                }

                if (sort.Table != SearchOptions._BaseClass && !innerjoins.ContainsKey(sort.Table.Name))
                {
                    if (!innerjoins.ContainsKey(sort.Table.Name))
                    {
                        innerjoins.Add(sort.Table.Name, sort.Table);
                    }
                }

                //BLLBase sortIntance = (BLLBase)(Activator.CreateInstance(sort.Table));

                if (sqlSort.Length > 0)
                {
                    sqlSort.Append(", ");
                }

                sqlSort.Append(sort.ToString(Connection));
                //campos que entram no sort
                if (SearchOptions.Distinct && !(sort.Table == SearchOptions._BaseClass))
                {
                    string tableAlias = sort.Table.Name;

                    string fieldExpression = string.Empty;


                    FieldInfo fieldInfo = sort.FieldInfo;
                    SpecialFieldInfo spInfo = sort.SpecialFieldInfo;

                    if (fieldInfo != null)
                    {
                        fieldExpression = tableAlias + "." + builder.QuoteIdentifier(fieldInfo.DataFieldName);
                    }
                    else if (spInfo != null)
                    {
                        fieldExpression = string.Format(spInfo.Expression, tableAlias) + " " + spInfo.Alias;
                    }

                    string campo = ", " + fieldExpression;
                    if (!campos.Contains(campo))
                    {
                        campos += campo;
                    }
                }
            }



            //Creates the where part
            string sqlWHERE = ReadConditions(SearchOptions.Conditions, @Params, ref innerjoins, SearchOptions._BaseClass, Connection);

            //Creates the innerJoins conditions
            StringBuilder sqlIJ = new StringBuilder();

            if (innerjoins.Count > 0)
            {
                if (!innerjoins.ContainsKey(SearchOptions._BaseClass.Name))
                {
                    innerjoins.Add(SearchOptions._BaseClass.Name, SearchOptions._BaseClass);
                }
            }



            List<Type> encontrados = new List<Type>();

            foreach (string IJkey in innerjoins.Keys)
            {
                Type IJ = innerjoins[IJkey];

                //BLLBase IJinstance = (BLLBase)(Activator.CreateInstance(IJ));



                ForeignKeyInfo[] fks = BLLBase.GetForeignKeys(IJ);
                foreach (ForeignKeyInfo fk in fks)
                {
                    //se a tabela atual contem referencia para classe base ou para outro item das condicoes
                    if (fk.ElementType == SearchOptions._BaseClass || innerjoins.ContainsValue(fk.ElementType))
                    {

                        //BLLBase tabela = (BLLBase)(Activator.CreateInstance(fk.ElementType));
                        //tabela.SetActiveConnection(Connection);
                        TableInfo ijInfo = TableInfo.CreateTableInfo(fk.ElementType);
                        string IJKeyleft = ijInfo.RelatedTable.Name;

                        foreach (string i in innerjoins.Keys)
                        {
                            if (innerjoins[i] == fk.ElementType)
                            {
                                IJKeyleft = i;
                                break;
                            }
                        }

                        if (fk.ForeignFields.Length == 0)
                        {
                            throw (new InvalidOperationException("Cannot find foreign field for \'" + fk.ToString() + "\'"));
                        }
                        if (fk.LocalFields.Length == 0)
                        {
                            throw (new InvalidOperationException("Cannot find local field for \'" + fk.ToString() + "\'"));
                        }

                        string left = IJKeyleft + "." + builder.QuoteIdentifier(fk.ForeignFields[0].DataFieldName);
                        string right = IJkey + "." + builder.QuoteIdentifier(fk.LocalFields[0].DataFieldName);

                        if (!sqlIJ.ToString().Contains(left + " = " + right) && !sqlIJ.ToString().Contains(right + " = " + left))
                        {
                            if (sqlIJ.Length > 0)
                            {
                                sqlIJ.Append(" AND ");
                            }
                            sqlIJ.AppendLine(left + " = " + right);
                        }

                        if (!encontrados.Contains(fk.ElementType))
                        {
                            encontrados.Add(fk.ElementType);
                        }
                        if (!encontrados.Contains(IJ))
                        {
                            encontrados.Add(IJ);
                        }

                    }
                }

            }

            foreach (string IJkey in innerjoins.Keys)
            {
                Type IJ = innerjoins[IJkey];
                if (!encontrados.Contains(IJ))
                {
                    throw (new ArgumentException("Cannot create all Inner Join relations (" + IJ.Name + ")."));
                }
            }


            //Montar SQL
            System.Text.StringBuilder sql = new System.Text.StringBuilder();

            sql.Append("SELECT ");
            if (!Count)
            {
                if (SearchOptions.Distinct)
                {
                    sql.Append("DISTINCT ");
                }
                if (SearchOptions.Top > 0)
                {
                    sql.Append(" TOP " + SearchOptions.Top.ToString() + " ");
                }
                sql.AppendLine(campos);
            }
            else
            {
                sql.Append(" COUNT(*) ");
            }


            string froms = table.GetSchemaAndTable();

            //TODO: Implement localizable searchs


            ////' Tradução automática com a view correspondente
            //if (instance.Localizable && CultureInfo.CurrentCulture.IetfLanguageTag != Configuration.Localization.DefaultCulture)
            //{
            //    froms = GetSchemaAndView(instance);

            //    if (sqlWHERE.Length > 0)
            //    {
            //        sqlWHERE += " AND ";
            //    }
            //    sqlWHERE += "IetfLanguageTag = \'" + CultureInfo.CurrentCulture.IetfLanguageTag + "\'";
            //}

            froms += " " + table.GetTableAlias(); 
            // TODO: Create different aliases.

            foreach (string ijkey in innerjoins.Keys)
            {
                Type ij = innerjoins[ijkey];
                TableInfo ijInfo = TableInfo.CreateTableInfo(ij);
                //BLLBase tabela = (BLLBase)(Activator.CreateInstance(ij));
                //tabela.SetActiveConnection(Connection);
                string tbl = ijInfo.GetSchemaAndTable() + " " + ijkey;

                if (!froms.ToString().Contains(tbl))
                {
                    froms += ", " + tbl;
                }

            }


            sql.AppendLine(" FROM " + froms);

            if (sqlWHERE.Length > 0 || sqlIJ.Length > 0)
            {
                sql.Append(" WHERE ");
                if (sqlIJ.Length > 0)
                {
                    sql.Append(sqlIJ.ToString());
                    if (sqlWHERE.Length > 0)
                    {
                        sql.AppendLine(" AND ");
                    }
                }
                if (sqlWHERE.Length > 1)
                {
                    sql.AppendLine("(" + sqlWHERE.ToString() + ")");
                }
            }

            if (sqlSort.Length > 0)
            {
                sql.Append(" ORDER BY ");
                sql.AppendLine(sqlSort.ToString());
            }


            Tenor.Diagnostics.Debug.DebugSQL("GetSearchSql()", sql.ToString(), Params.ToArray(), Connection);

            return sql.ToString();
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

        private static Tenor.Data.DataTable SearchWithDataTable(SearchOptions SearchOptions, ConnectionStringSettings Connection, bool Count)
        {

            List<TenorParameter> parameters = new List<TenorParameter>();
            if (Connection == null)
            {
                TableInfo table = TableInfo.CreateTableInfo(SearchOptions._BaseClass);
                Connection = table.GetConnection();
            }

            string sql = GetSearchSql(SearchOptions, ref parameters, Count, Connection);
            Tenor.Data.DataTable rs = new Tenor.Data.DataTable(sql, parameters.ToArray(), Connection);
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
        internal string GetSaveSql(bool isUpdate, List<TenorParameter> parameters, ref FieldInfo autoKeyField, NameValueCollection specialValues, ConnectionStringSettings connection)
        {
            TableInfo table = TableInfo.CreateTableInfo(this.GetType());
            if (connection == null)
                connection = table.GetConnection();

            string paramPrefix = Helper.GetParameterPrefix(connection); // The @, :, ?
            DbCommandBuilder builder = Helper.GetCommandBuilder(connection);

            string fieldValues = "";
            string clause = "";
            string fields = "";
            string values = "";

            autoKeyField = null;
            foreach (FieldInfo field in GetFields(this.GetType()))
            {
                if (!field.LazyLoading || propertyData.ContainsKey(field.RelatedProperty.Name))
                {
                    string paramName = GetParamName(table.GetTableAlias(), field);
                    TenorParameter param = new TenorParameter(paramName, field.PropertyValue(this));
                    if (field.AutoNumber)
                    {
                        autoKeyField = field;
                    }
                    else
                    {

                        if (!string.IsNullOrEmpty(fields))
                        {
                            fields += ", ";
                        }
                        fields += builder.QuoteIdentifier(field.DataFieldName);
                        if (!string.IsNullOrEmpty(values))
                        {
                            values += ", ";
                        }


                        if (specialValues == null || string.IsNullOrEmpty(specialValues[paramName]))
                        {
                            values += paramPrefix + paramName; // The straigt parameter
                        }
                        else
                        {
                            values += specialValues[paramName]; //Replaced by a SQL statement
                        }

                        if (!field.PrimaryKey)
                        {
                            if (!string.IsNullOrEmpty(fieldValues))
                            {
                                fieldValues += ", ";
                            }
                            if (specialValues == null || string.IsNullOrEmpty(specialValues[paramName]))
                            {
                                fieldValues += builder.QuoteIdentifier(field.DataFieldName) + " = " + paramPrefix + paramName;
                            }
                            else
                            {
                                fieldValues += builder.QuoteIdentifier(field.DataFieldName) + " = " + specialValues[paramName];
                            }
                        }


                    }

                    if (field.PrimaryKey)
                    {
                        if (!string.IsNullOrEmpty(clause))
                        {
                            clause += " AND ";
                        }
                        clause += "" + builder.QuoteIdentifier(field.DataFieldName) + " = " + paramPrefix + paramName;
                    }

                    parameters.Add(param);
                }
            }

            string query = "";
            if (isUpdate)
            {
                query = string.Format("UPDATE {0} SET {1} WHERE {2}",
                    table.GetSchemaAndTable(),
                    fieldValues,
                    clause);
            }
            else
            {
                query = string.Format("INSERT INTO {0} ({1}) VALUES ({2})",
                    table.GetSchemaAndTable(), 
                    fields,
                    values);
            }

            return query;
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

                FieldInfo AutoKeyField = null;

                List<TenorParameter> @params = new List<TenorParameter>();
                string query = GetSaveSql(isUpdate, @params, ref AutoKeyField, null, connection);

                int result = Helper.AtualizarBanco(connection, query, @params);

                if (!isUpdate && (AutoKeyField != null))
                {
                    AutoKeyField.SetPropertyValue(this, Convert.ChangeType(result, AutoKeyField.FieldType));
                }
            }
        }

        /// <summary>
        /// Somente salva a instancia se o Campo Condicional não existir.
        /// </summary>
        public virtual bool SaveConditional(string ConditionalField)
        {
            return SaveConditional(new string[] { ConditionalField });
        }

        /// <summary>
        /// Somente salva a instancia se o Campo Condicional não existir.
        /// </summary>
        public virtual bool SaveConditional(string[] ConditionalFields)
        {
            return SaveConditional(ConditionalFields, false);
        }

        /// <summary>
        /// Salva ou atualiza a instancia de acordo com isUpdate
        /// </summary>
        public virtual bool SaveConditional(string ConditionalField, bool isUpdate)
        {
            return SaveConditional(new string[] { ConditionalField }, isUpdate);
        }

                /// <summary>
        /// Quando isUpdate é False, o sistema cria um registro novo, se o mesmo não existir. Se o registro for criado,
        /// retorna True.
        /// Quando isUpdate é True, o sistema cria um registro novo se o mesmo não exister, caso contrário, atualiza.
        /// Se o registro for criado, retorna True.
        /// </summary>
        public virtual bool SaveConditional(string[] ConditionalFields, bool isUpdate)
        {
            return SaveConditional(ConditionalFields, isUpdate, null);
        }
        /// <summary>
        /// Quando isUpdate é False, o sistema cria um registro novo, se o mesmo não existir. Se o registro for criado,
        /// retorna True.
        /// Quando isUpdate é True, o sistema cria um registro novo se o mesmo não exister, caso contrário, atualiza.
        /// Se o registro for criado, retorna True.
        /// </summary>
        public virtual bool SaveConditional(string[] ConditionalFields, bool isUpdate, ConnectionStringSettings connection)
        {
            if (Validate())
            {
                TableInfo table = TableInfo.CreateTableInfo(this.GetType());
                if (connection == null)
                    connection = table.GetConnection();
                DbProviderFactory factory = Helper.GetFactory(connection);
                string paramPrefix = Helper.GetParameterPrefix(factory);
                
                FieldInfo[] fields = BLLBase.GetFields(this.GetType(), null, ConditionalFields);

                if (ConditionalFields.Length == 0)
                {
                    throw (new ArgumentException("Cannot find one or more ConditionalFields", "ConditionalFields"));
                }
                else if (fields.Length != ConditionalFields.Length)
                {
                    throw (new ArgumentException("Cannot find one or more ConditionalFields", "ConditionalFields"));
                }

                FieldInfo[] fieldsPrimary = BLLBase.GetFields(this.GetType(), true);


                FieldInfo AutoKeyField = null;

                List<TenorParameter> @params = new List<TenorParameter>();
                string insertQuery = GetSaveSql(false, @params, ref AutoKeyField, null, connection);

                //updateQuery está com new List<Parameter> pois os parametros já foram definidos no insertQuery.
                string updateQuery = GetSaveSql(true, new List<TenorParameter>(), ref AutoKeyField, null, connection);

                StringBuilder query = new StringBuilder();
                StringBuilder queryDeclares = new StringBuilder();

                StringBuilder querySelect = new StringBuilder();
                StringBuilder queryIsNull = new StringBuilder();
                StringBuilder queryWhere = new StringBuilder();

                foreach (FieldInfo f in fieldsPrimary)
                {
                    TenorParameter p = new TenorParameter(f.RelatedProperty.Name + "__test", f.PropertyValue(this));
                    querySelect.Append(",");
                    querySelect.Append(paramPrefix + table.GetTableAlias() + f.RelatedProperty.Name);
                    querySelect.Append(" = ");
                    querySelect.Append(f.DataFieldName);
                }


                foreach (FieldInfo f in fields)
                {
                    TenorParameter p = new TenorParameter(f.RelatedProperty.Name + "__test", f.PropertyValue(this));
                    queryDeclares.AppendLine("DECLARE " + paramPrefix + f.RelatedProperty.Name + "_ " + Helper.GetDbTypeName(p.Value.GetType(), factory));
                    querySelect.Append(",");
                    querySelect.Append(paramPrefix + f.RelatedProperty.Name + "_");
                    querySelect.Append(" = ");
                    querySelect.Append(f.DataFieldName);

                    queryWhere.Append(" AND ");
                    queryWhere.Append(f.DataFieldName);
                    queryWhere.Append(" = ");
                    queryWhere.Append(paramPrefix + table.GetTableAlias() + f.RelatedProperty.Name);

                    queryIsNull.Append(" AND ");
                    queryIsNull.Append(paramPrefix + f.RelatedProperty.Name + "_ IS NULL");

                }
                query.Append(queryDeclares);
                query.AppendLine();
                query.Append("SELECT ");

                querySelect.Remove(0, 1);
                query.Append(querySelect);
                query.Append(" FROM ");
                query.Append(table.GetSchemaAndTable());
                query.Append(" WHERE ");
                queryWhere.Remove(0, 4);
                query.AppendLine(queryWhere.ToString());

                queryIsNull.Remove(0, 4);
                query.Append("IF ");
                query.Append(queryIsNull);
                query.AppendLine(" BEGIN");
                query.AppendLine("      " + insertQuery);
                query.AppendLine("      SELECT SCOPE_IDENTITY()");
                query.AppendLine(" END ELSE BEGIN ");
                if (isUpdate)
                {
                    query.AppendLine("      " + updateQuery);
                }
                query.AppendLine("      SELECT -1 ");
                query.AppendLine(" END ");


                DataTable result = Helper.ConsultarBanco(connection, query.ToString(), @params);

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
                    if (AutoKeyField != null)
                    {
                        AutoKeyField.SetPropertyValue(this, Convert.ChangeType(result.Rows[0][0], AutoKeyField.FieldType));
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
        /// Remove esta instancia do banco de dados
        /// </summary>
        public void Delete()
        {
            TableInfo table = TableInfo.CreateTableInfo(this.GetType());
            ConnectionStringSettings connection = table.GetConnection();
            DbCommandBuilder builder = Helper.GetCommandBuilder(connection);
            string paramPrefix = Helper.GetParameterPrefix(connection);

            string clause = "";
            List<TenorParameter> @params = new List<TenorParameter>();
            foreach (FieldInfo i in GetFields(this.GetType()))
            {
                if (i.PrimaryKey)
                {
                    TenorParameter param = new TenorParameter(i.DataFieldName.Replace(" ", "_"), i.PropertyValue(this));
                    clause += " AND " + builder.QuoteIdentifier(i.DataFieldName) + (" = " + paramPrefix + param.ParameterName);
                    @params.Add(param);
                }
            }
            if (string.IsNullOrEmpty(clause))
            {
                throw (new MissingPrimaryKeyException());
            }
            else
            {
                clause = clause.Substring(4);
            }

            string query = "DELETE FROM " + table.GetSchemaAndTable() + " WHERE " + clause;

            Helper.AtualizarBanco(connection, query, @params);
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
        /// Valida se os dados estão consistentes para a persistência no banco
        /// </summary>
        /// <returns>Se validou ou não</returns>
        /// <remarks>Sempre retorna True, deve ser sobrescrita</remarks>
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

        #region " Operadores "

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
                    throw (new InvalidCastException());
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

        /// <summary>
        /// Compara duas instancias utilizando suas chaves primárias definidas.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool operator ==(BLLBase x, BLLBase y)
        {
            return object.Equals(x, y);
        }

        public static bool operator !=(BLLBase x, BLLBase y)
        {
            return !object.Equals(x, y);
        }
        #endregion

    }
}