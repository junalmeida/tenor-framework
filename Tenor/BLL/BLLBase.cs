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
//using MissingPrimaryKeyException = System.Data.MissingPrimaryKeyException;
//using DataRow = System.Data.DataRow;
//using DataSet = System.Data.DataSet;
using DataTable = Tenor.Data.DataTable;
using DbType = System.Data.DbType;
using System.Text;
using System.Threading;
using System.ComponentModel;


namespace Tenor.BLL
{

    /// <summary>
    /// Esta é a classe base de persistência de dados.
    /// As classes que herdam desta ganham funções para ler e salvar dados em bancos de dados. <br />
    /// Você pode utilizar o <a href="../zeus.htm">Modelo do MyGeneration</a> no arquivo BLLBased.zeus, contido nesta documentação.<br />
    /// Autor: Marcos A. P. Almeida jr.
    /// </summary>
    [Serializable()]
    public abstract class BLLBase : object
    {


        #region "Ctors"


        //Private __LoadedFields As ObjectModel.Collection(Of String)
        //Protected ReadOnly Property _LoadedFields() As ObjectModel.Collection(Of String)
        //    Get
        //        If __LoadedFields Is Nothing Then __LoadedFields = New ObjectModel.Collection(Of String)
        //        Return __LoadedFields
        //    End Get
        //End Property

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

        #region " Connection "

        /// <summary>
        /// Usado em aplicações Desktop
        /// </summary>
        /// <remarks></remarks>
        private static ConnectionStringSettings _SystemConnection;

        /// <summary>
        /// Armazena aqui uma conexão a ser usada por todo o sistema.
        ///
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>
        /// O valor dessa propriedade define o valor da propriedade Connection de todas as BLLBases
        /// </remarks>
        public static ConnectionStringSettings SystemConnection
        {
            get
            {
                System.Web.HttpContext Context = System.Web.HttpContext.Current;
                if (Context != null)
                {
                    if (!Context.Request.RawUrl.ToLower().Contains("/publicador"))
                    {
                        return null;
                    }

                    if (Context.Session != null)
                    {
                        string name = Context.Session[Tenor.Configuration.TenorModule.IdPrefix + "SYSTEMCONNECTION_name"].ToString();
                        string cn = Context.Session[Tenor.Configuration.TenorModule.IdPrefix + "SYSTEMCONNECTION_cn"].ToString();
                        string pn = Context.Session[Tenor.Configuration.TenorModule.IdPrefix + "SYSTEMCONNECTION_pn"].ToString();
                        if (cn == null)
                        {
                            return null;
                        }
                        else
                        {
                            return new ConnectionStringSettings(name, cn, pn);
                        }
                    }
                    else
                    {
                        throw (new Exception("No session state available."));
                    }
                }
                else
                {
                    return _SystemConnection;
                }
            }
            set
            {
                System.Web.HttpContext Context = System.Web.HttpContext.Current;

                string name = Context.Session[Tenor.Configuration.TenorModule.IdPrefix + "SYSTEMCONNECTION_name"].ToString();
                string cn = Context.Session[Tenor.Configuration.TenorModule.IdPrefix + "SYSTEMCONNECTION_cn"].ToString();
                string pn = Context.Session[Tenor.Configuration.TenorModule.IdPrefix + "SYSTEMCONNECTION_pn"].ToString();

                
                if (Context != null)
                {
                    if (Context.Session != null)
                    {
                        if (value == null)
                        {
                            Context.Session.Remove(name);
                            Context.Session.Remove(cn);
                            Context.Session.Remove(pn);
                        }
                        else
                        {
                            Context.Session[name] = value.Name;
                            Context.Session[cn] = value.ConnectionString;
                            Context.Session[pn] = value.ProviderName;
                        }
                    }
                    else
                    {
                        throw (new Exception("No session state available."));
                    }
                }
                else
                {
                    _SystemConnection = value;
                }
            }
        }

        [NonSerialized()]
        private ConnectionStringSettings _Connection;
        /// <summary>
        /// Armazena configurações de conexão.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// Quando não setada, retorna a propriedade SystemConnection, ou a primeira conexão encontrada na web.config
        /// </returns>
        /// <remarks></remarks>
        protected virtual ConnectionStringSettings Connection
        {
            get
            {
                if (_Connection == null)
                {
                    return GetDefaultConnection();
                }
                else
                {
                    return _Connection;
                }
            }
        }

        /// <summary>
        /// Retonar a conexão padrão para ser usada com a BLLBase.
        /// </summary>
        public static ConnectionStringSettings GetDefaultConnection()
        {
            if (SystemConnection != null)
            {
                return SystemConnection;
            }

            if (ConfigurationManager.ConnectionStrings.Count == 0)
            {
                throw (new ConfigurationErrorsException("Cannot find any usable connection string."));
            }
            else
            {
                int I = 0;
                do
                {
                    if (!ConfigurationManager.ConnectionStrings[I].ConnectionString.ToLower().Contains("|DataFile|") && !ConfigurationManager.ConnectionStrings[I].ConnectionString.ToLower().Contains("aspnetdb.mdf"))
                    {
                        break;
                    }

                    I++;
                    if (I > ConfigurationManager.ConnectionStrings.Count - 1)
                    {
                        throw (new ConfigurationErrorsException("Cannot find any usable connection string."));
                    }
                } while (true);
                return ConfigurationManager.ConnectionStrings[I];
            }

        }

        /// <summary>
        /// Muda a conexão atual do objeto para a instancia escolhida da config
        /// </summary>
        /// <param name="configName">Nome da configuração na config</param>
        /// <remarks></remarks>
        public void SetActiveConnection(string configName)
        {
            SetActiveConnection(ConfigurationManager.ConnectionStrings[configName]);
        }

        /// <summary>
        /// Muda a conexão atual do objeto para a instancia escolhida da config
        /// </summary>
        /// <param name="config">Configurações de conexão</param>
        /// <remarks></remarks>
        public void SetActiveConnection(ConnectionStringSettings config)
        {
            _Connection = config;
        }

        [NonSerialized()]
        private DbProviderFactory _factory;
        [NonSerialized()]
        private DbCommandBuilder _cmdbuild;
        /// <summary>
        /// CommandBuilder necessário para construír os objetos para conexão ao banco de dados.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        protected DbCommandBuilder CommandBuilder
        {
            get
            {
                GetFactory();
                if (_cmdbuild == null)
                {
                    _cmdbuild = _factory.CreateCommandBuilder();
                    try
                    {
                        _cmdbuild.QuoteIdentifier("teste");
                    }
                    catch (Exception)
                    {
                        //Gambiarra para OLEDB
                        _cmdbuild.QuotePrefix = "[";
                        _cmdbuild.QuoteSuffix = "]";

                        //_cmdbuild.RefreshSchema()

                        //_cmdbuild.DataAdapter = _factory.CreateDataAdapter()
                        //_cmdbuild.DataAdapter.SelectCommand = _factory.CreateCommand
                        //_cmdbuild.DataAdapter.SelectCommand.Connection = _factory.CreateConnection
                        //_cmdbuild.DataAdapter.SelectCommand.Connection.ConnectionString = Me.Connection.ConnectionString
                        //_cmdbuild.DataAdapter.SelectCommand.Connection.Open()
                    }
                }

                return _cmdbuild;
            }
        }

        /// <summary>
        /// Retorna o nome da tabela real no modelo relacional
        /// </summary>
        /// <remarks></remarks>
        public virtual string TableName
        {
            get
            {
                return this.GetType().Name;
            }
        }

        /// <summary>
        /// Retorna o schema no modelo relacional
        /// </summary>
        /// <remarks></remarks>
        public virtual string SchemaName
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Retorna o nome do banco de dados.
        /// Esta propriedade não é escapada, para compatibilidade com Linked Servers.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual string Database
        {
            get
            {
                return string.Empty;
            }
        }

        internal DbCommandBuilder GetCommandBuilder()
        {
            return CommandBuilder;
        }

        internal DbProviderFactory GetFactory()
        {
            if (_factory == null)
            {
                _factory = DbProviderFactories.GetFactory(Connection.ProviderName);
            }
            return _factory;
        }

        #endregion

        #region " Bind "


        /// <summary>
        /// Faz uma consulta ao banco usando usando a ActiveConnection e preenche os valores nas propriedades do tipo Field
        /// </summary>
        /// <remarks>Faz a consulta com LazyLoading</remarks>
        public void Bind()
        {
            Bind(true);
        }


        ///'' <exception cref="MissingTableAttributeException">Ocorre se a classe não implementar um TableAttribute</exception>
        ///' <summary>
        ///' Faz uma consulta ao banco usando usando a ActiveConnection e preenche os valores nas propriedades do tipo Field
        ///' </summary>
        ///' <param name="LazyLoading">Ativa ou desativa o modo lazy loading para relacionamentos. O padrão é Verdadeiro.</param>
        ///' <exception cref="MissingFieldsException">Ocorre se a classe não implementar nenhum campo com FieldAttribute</exception>
        ///' <exception cref="MissingPrimaryKeyException">Ocorre se a classe não implementar nenhum campo com FieldAttribute marcado como PrimaryKey</exception>
        ///' <remarks></remarks>
        //Public Overridable Sub Bind(ByVal LazyLoading As Boolean)
        //    Bind(Me, LazyLoading)
        //End Sub



        /// <summary>
        /// Faz uma consulta ao banco usando usando a ActiveConnection e preenche os valores nas propriedades do tipo Field
        /// </summary>
        /// <param name="LazyLoading">Ativa ou desativa o Lazy Loading</param>
        /// <exception cref="MissingFieldsException">Ocorre se a classe não implementar nenhum campo com FieldAttribute</exception>
        /// <exception cref="System.Data.MissingPrimaryKeyException">Ocorre se a classe não implementar nenhum campo com FieldAttribute marcado como PrimaryKey</exception>
        /// <remarks></remarks>
        protected void Bind(bool LazyLoading)
        {
            BLLBase Instance = this;

            List<string> fields = new List<string>();


            foreach (FieldInfo f in GetFields(Instance.GetType()))
            {
                if (f.PrimaryKey)
                {
                    fields.Add(f.RelatedProperty.Name);
                }
            }
            if (fields.Count == 0)
            {
                throw (new MissingPrimaryKeyException());
            }

            //For Each i As Reflection.PropertyInfo In Instance.GetType().GetProperties()
            //    Dim attsfields As FieldAttribute() = CType(i.GetCustomAttributes(GetType(FieldAttribute), True), FieldAttribute())
            //    If Not attsfields Is Nothing AndAlso attsfields.Length > 0 Then
            //        If attsfields(0).PrimaryKey Then
            //            fields.Add(i.Name)
            //        End If
            //    End If
            //Next
            Bind(LazyLoading, fields.ToArray());
        }

        /// <summary>
        /// Faz uma consulta ao banco usando usando a ActiveConnection e preenche os valores nas propriedades do tipo Field
        /// </summary>
        /// <param name="LazyLoading">Ativa ou desativa o Lazy Loading</param>
        /// <param name="FilterMembers">Membros da classe usados nos filtros</param>
        /// <remarks></remarks>
        protected void Bind(bool LazyLoading, string[] FilterMembers)
        {
            Bind(LazyLoading, FilterMembers, null);
        }



        /// <summary>
        /// Faz uma consulta ao banco usando usando a ActiveConnection e preenche os valores nas propriedades do tipo Field
        /// </summary>
        /// <param name="LazyLoading">Ativa ou desativa o Lazy Loading</param>
        /// <param name="FilterMembers">Membros da classe usados nos filtros</param>
        /// <param name="DataRow">DataRow para usar no mapeamento dos dados</param>
        /// <remarks>Se o DataRow for Nulo o sistema irá ao banco busca-lo</remarks>
        protected virtual void Bind(bool LazyLoading, string[] FilterMembers, DataRow DataRow)
        {
            BLLBase Instance = this;

            bool fromSearch = (DataRow != null);
            //datarow é nothing quando for LoadForeign ou bind simples
            //datarow não é nothing quando for pesquisa (Search)

            if (!fromSearch && Cacheable && LoadFromCache())
            {
                //LoadFromCache retorna true
                return;
            }


            //Carregar campos
            DataRow dr;
            if (!fromSearch)
            {

                SearchOptions so = new SearchOptions(Instance.GetType());



                // SELECT e FILTROS

                List<FieldInfo> filters = new List<FieldInfo>();
                foreach (string s in FilterMembers)
                {
                    FieldInfo field = new FieldInfo(Instance.GetType().GetProperty(s));
                    filters.Add(field);
                }


                foreach (FieldInfo field in filters)
                {
                    if (so.Conditions.Count > 0)
                    {
                        so.Conditions.Add(Tenor.Data.LogicalOperator.And);
                    }
                    so.Conditions.Add(field.RelatedProperty.Name, field.PropertyValue(Instance));
                }


                System.Data.DataTable dt = SearchWithDataTable(so, this.Connection);

                if (dt.Rows.Count == 0)
                {
                    dt.Dispose();
                    throw (new RecordNotFoundException());
                }
                else if (dt.Rows.Count > 1)
                {
                    Trace.TraceWarning("More than one row was returned. Using only the first row.");
                }

                dr = dt.Rows[0];

            }
            else
            {
                dr = DataRow;
            }




            FieldInfo[] fields = GetFields(Instance.GetType());
            SpecialFieldInfo[] spfields = GetSpecialFields(Instance.GetType());
            ForeignKeyInfo[] foreignkeys = GetForeignKeys(Instance.GetType());

            foreach (FieldInfo f in fields)
            {
                if (f.PrimaryKey || !f.LazyLoading)
                {

                    //colocar os itens da tabela em suas propriedades correspondentes
                    f.SetPropertyValue(Instance, dr[f.DataFieldName]);

                    //----
                }
            }
            foreach (SpecialFieldInfo f in spfields)
            {
                //colocar os itens da tabela em suas propriedades correspondentes
                f.SetPropertyValue(Instance, dr[f.Alias]);
            }


            dr = null;



            if (!LazyLoading)
            {
                foreach (ForeignKeyInfo f in foreignkeys)
                {
                    //Continuar em LazyLoading para evitar loops infinitos.
                    Instance.LoadForeign(f.RelatedProperty.Name, true, this.Connection);
                }
            }


            if (Cacheable)
            {
                SaveToCache();
            }
        }

        #endregion

        #region " Search "

        private static string ReadSelectFields(BLLBase Instance, FieldInfo[] Fields, SpecialFieldInfo[] SpFields)
        {
            string campos = "";
            foreach (FieldInfo f in Fields)
            {
                if (f.PrimaryKey || !f.LazyLoading)
                {
                    //determina o nome do campo
                    campos += ", " + Instance.GetType().Name + "." + Instance.CommandBuilder.QuoteIdentifier(f.DataFieldName);
                }
            }
            if (campos.Length < 2)
            {
                throw (new InvalidOperationException("Cannot find any TableFields for loading this type"));
            }

            foreach (SpecialFieldInfo f in SpFields)
            {
                campos += ", (" + string.Format(f.Expression, Instance.GetType().Name) + ") " + f.Alias;
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
        private static string ReadConditions(ConditionCollection Conditions, List<Parameter> @params, ref Dictionary<string, Type> innerjoins, Type BaseClass, ConnectionStringSettings Connection)
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
                        @params.Add(new Parameter(Connection, sc.ParameterName, sc.Value));
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
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GetSchemaAndTable(BLLBase instance)
        {
            string schemaName = instance.SchemaName;

            string res = instance.Database;
            if (!string.IsNullOrEmpty(res))
            {
                res += ".";
            }

            if (!string.IsNullOrEmpty(instance.SchemaName))
            {
                res += instance.CommandBuilder.QuoteIdentifier(schemaName) + ".";
            }
            else if (!string.IsNullOrEmpty(res))
            {
                res += ".";
            }
            res += instance.CommandBuilder.QuoteIdentifier(instance.TableName);

            return res;
        }

        /// <summary>
        /// Retorna o schema e a tabela. Caso a tabela seja localizada, usa a View correspondente no
        /// Schema Localization.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected static string GetSchemaAndView(BLLBase instance)
        {
            string schemaName = "Localization";


            string res = string.Empty;
            res = instance.CommandBuilder.QuoteIdentifier(schemaName) + ".";
            res += instance.CommandBuilder.QuoteIdentifier(instance.SchemaName + instance.TableName);

            return res;
        }


        /// <summary>
        /// Retorna o SQL gerado pelas condições.
        /// </summary>
        /// <param name="SearchOptions"></param>
        /// <param name="Connection">By ref</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GetSearchSql(SearchOptions SearchOptions, ref List<Parameter> Params, bool Count, ref ConnectionStringSettings Connection)
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

            BLLBase instance = (BLLBase)(Activator.CreateInstance(SearchOptions._BaseClass));


            if (Connection == null)
            {
                Connection = instance.Connection;
            }
            else
            {
                instance.SetActiveConnection(Connection);
            }

            //Escaneia os campos necessários para a consulta da classe base
            FieldInfo[] fields = BLLBase.GetFields(SearchOptions._BaseClass);
            SpecialFieldInfo[] spfields = BLLBase.GetSpecialFields(SearchOptions._BaseClass);
            string campos = ReadSelectFields(instance, fields, spfields);

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

                if (!sort.Table.IsSubclassOf(typeof(BLL.BLLBase)))
                {
                    throw (new ArgumentException("Invalid Table type. You must specify a derived type of BLL.BLLBase on SortCriteria class", "Table", null));
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

                BLLBase sortIntance = (BLLBase)(Activator.CreateInstance(sort.Table));

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
                        fieldExpression = tableAlias + "." + instance.CommandBuilder.QuoteIdentifier(fieldInfo.DataFieldName);
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



            //Monta a WHERE
            string sqlWHERE = ReadConditions(SearchOptions.Conditions, @Params, ref innerjoins, SearchOptions._BaseClass, Connection);

            //WHERES dos INNERJOINS
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

                BLLBase IJinstance = (BLLBase)(Activator.CreateInstance(IJ));

                

                ForeignKeyInfo[] fks = BLLBase.GetForeignKeys(IJ);
                foreach (ForeignKeyInfo fk in fks)
                {
                    //se a tabela atual contem referencia para classe base ou para outro item das condicoes
                    if (fk.ElementType == SearchOptions._BaseClass || innerjoins.ContainsValue(fk.ElementType))
                    {

                        BLLBase tabela = (BLLBase)(Activator.CreateInstance(fk.ElementType));
                        tabela.SetActiveConnection(Connection);

                        string IJKeyleft = tabela.GetType().Name;
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

                        string left = IJKeyleft + "." + tabela.CommandBuilder.QuoteIdentifier(fk.ForeignFields[0].DataFieldName);
                        string right = IJkey + "." + tabela.CommandBuilder.QuoteIdentifier(fk.LocalFields[0].DataFieldName);

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


                        tabela = null;
                        //Exit For
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


            string froms = GetSchemaAndTable(instance);

            //' Tradução automática com a view correspondente
            if (instance.Localizable && CultureInfo.CurrentCulture.IetfLanguageTag != Configuration.Localization.DefaultCulture)
            {
                froms = GetSchemaAndView(instance);

                if (sqlWHERE.Length > 0)
                {
                    sqlWHERE += " AND ";
                }
                sqlWHERE += "IetfLanguageTag = \'" + CultureInfo.CurrentCulture.IetfLanguageTag + "\'";
            }

            froms += " " + instance.GetType().Name; //alias da tabela é o nome da classe
            // se houver classes em namepasces diferentes

            foreach (string ijkey in innerjoins.Keys)
            {
                Type ij = innerjoins[ijkey];
                BLLBase tabela = (BLLBase)(Activator.CreateInstance(ij));
                tabela.SetActiveConnection(Connection);
                string tbl = GetSchemaAndTable(tabela) + " " + ijkey;

                if (!froms.ToString().Contains(tbl))
                {
                    froms += ", " + tbl;
                }

                tabela = null;
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

            try
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {

                    System.Text.StringBuilder traceInfo = new System.Text.StringBuilder();
                    traceInfo.AppendLine(SearchOptions.ToString());
                    traceInfo.AppendLine(" > " + Connection.ConnectionString);
                    foreach (Parameter p in @Params)
                    {
                        traceInfo.AppendLine("DECLARE " + p.ParameterPrefix + p.ParameterName + " " + p.DbTypeName);
                        if (p.Value == null)
                        {
                            traceInfo.AppendLine("SET " + p.ParameterPrefix + p.ParameterName + " = NULL");
                        }
                        else
                        {
                            string value = p.Value.ToString();
                            if (p.Value is bool)
                            {
                                value = Math.Abs(System.Convert.ToInt32(p.Value)).ToString();
                            }

                            if ((((((p.DbType == DbType.String) || (p.DbType == DbType.AnsiString)) || (p.DbType == DbType.AnsiStringFixedLength)) || (p.DbType == DbType.DateTime)) || (p.DbType == DbType.Date)) || (p.DbType == DbType.Time))
                            {
                                value = "\'" + value.Replace("\'", "\'\'") + "\'";
                            }

                            traceInfo.AppendLine("SET " + p.ParameterPrefix + p.ParameterName + " = " + value);
                        }
                    }
                    traceInfo.AppendLine(sql.ToString());

                    string st = Environment.StackTrace;
                    string fimDaondeNaoImporta = "Data.BulkInsert.DoDebug(String sql)" + Environment.NewLine;
                    int i = st.IndexOf(fimDaondeNaoImporta);
                    if (i > 0)
                    {
                        st = st.Substring(i + fimDaondeNaoImporta.Length);
                    }

                    traceInfo.AppendLine("Stack Trace:");
                    traceInfo.AppendLine(st);
                    traceInfo.AppendLine("---------------------");

                    System.Diagnostics.Trace.TraceInformation(traceInfo.ToString());

                }
            }
            catch (Exception ex)
            {
                Diagnostics.Debug.HandleError(ex);
            }

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
                instances[i]._Connection = Connection;
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
            List<Parameter> @params = new List<Parameter>();
            string sql = GetSearchSql(SearchOptions, ref @params, Count, ref Connection);
            Tenor.Data.DataTable rs = new Tenor.Data.DataTable(sql, @params.ToArray(), Connection);
            DataSet ds = new DataSet();
            ds.Tables.Add(rs);
            ds.EnforceConstraints = false;
            rs.Bind();
            return rs;
        }



        #endregion

        #region " Lazy Loading "

        /// <summary>
        /// Carrega as propriedades marcadas como chave extrangeira
        /// </summary>
        /// <param name="Property">Nome da propriedade que irá receber a carga</param>
        /// <param name="LazyLoading">Ativa ou desativa o Lazy Loading</param>
        /// <remarks></remarks>
        protected virtual void LoadForeign(string @Property, bool LazyLoading)
        {
            LoadForeign(@Property, LazyLoading, ((Type)null));
        }

        /// <summary>
        /// Carrega as propriedades marcadas como chave extrangeira
        /// </summary>
        /// <param name="Property">Nome da propriedade que irá receber a carga</param>
        /// <param name="LazyLoading">Ativa ou desativa o Lazy Loading</param>
        /// <remarks></remarks>
        protected virtual void LoadForeign(string @Property, bool LazyLoading, Type Type)
        {
            System.Reflection.PropertyInfo FieldP = null;
            if (Type != null)
            {
                FieldP = this.GetType().GetProperty(@Property, Type);
            }
            else
            {
                FieldP = this.GetType().GetProperty(@Property);
            }
            ForeignKeyInfo Field = new ForeignKeyInfo(FieldP);

            BLLBase instance = (BLLBase)(Activator.CreateInstance(Field.ElementType));
            LoadForeign(@Property, LazyLoading, Type, instance.Connection);
        }

        /// <summary>
        /// Carrega as propriedades marcadas como chave extrangeira
        /// </summary>
        /// <param name="Property">Nome da propriedade que irá receber a carga</param>
        /// <param name="LazyLoading">Ativa ou desativa o Lazy Loading</param>
        /// <remarks></remarks>
        protected virtual void LoadForeign(string @Property, bool LazyLoading, ConnectionStringSettings Connection)
        {
            LoadForeign(@Property, LazyLoading, null, Connection);
        }

        /// <summary>
        /// Carrega as propriedades marcadas como chave extrangeira
        /// </summary>
        /// <param name="Property">Nome da propriedade que irá receber a carga</param>
        /// <param name="LazyLoading">Ativa ou desativa o Lazy Loading</param>
        /// <remarks></remarks>
        protected virtual void LoadForeign(string @Property, bool LazyLoading, Type Type, ConnectionStringSettings Connection)
        {

            System.Reflection.PropertyInfo FieldP = null;
            if (Type != null)
            {
                FieldP = this.GetType().GetProperty(@Property, Type);
            }
            else
            {
                FieldP = this.GetType().GetProperty(@Property);
            }
            ForeignKeyInfo Field = new ForeignKeyInfo(FieldP);


            //Dim filters As String = ""
            //Dim params As New List(Of Data.Parameter)

            BLLBase instance = (BLLBase)(Activator.CreateInstance(Field.ElementType));
            instance._Connection = Connection;
            if (!Field.IsArray)
            {
                if ((instance != null) && instance.Cacheable)
                {
                    //Se for uma classe de cache
                    for (int i = 0; i <= Field.ForeignFields.Length - 1; i++)
                    {
                        Field.ForeignFields[i].SetPropertyValue(instance, Field.LocalFields[i].PropertyValue(this));
                    }
                    instance.Bind(LazyLoading);
                    Field.SetPropertyValue(this, instance);
                    return;
                }
            }


            SearchOptions sc = new SearchOptions(Field.ElementType);
            sc.LazyLoading = LazyLoading;



            for (int i = 0; i <= Field.ForeignFields.Length - 1; i++)
            {
                if (i > 0)
                {
                    sc.Conditions.Add(Tenor.Data.LogicalOperator.And);
                }
                // o  campo externo é igual ao campo local.
                if (Field.LocalFields.Length - 1 < i)
                {
                    throw (new MissingFieldsException());
                }
                sc.Conditions.Add(Field.ForeignFields[i].RelatedProperty.Name, Field.LocalFields[i].PropertyValue(this));

            }
            if (sc.Conditions.Count == 0)
            {
                throw (new MissingFieldsException());
            }


            BLLBase[] instances = Search(sc, Connection);

            if (this._IsLazyDisabled)
            {
                foreach (BLLBase i in instances)
                {
                    i._IsLazyDisabled = this._IsLazyDisabled;
                }
            }


            if (Field.IsArray)
            {
                if (Field.RelatedProperty.PropertyType.IsArray)
                {
                    Field.SetPropertyValue(this, instances);
                }
                else
                {

                    bool lastValue = this._IsLazyDisabled;
                    this._IsLazyDisabled = true;
                    object obj = null;
                    try
                    {
                        obj = Field.PropertyValue(this);
                    }
                    catch (Exception ex)
                    {
                        Exception up = new Exception("The property \'" + Field.RelatedProperty.Name + "\' must return an instance of a collection", ex);
                        throw (up);
                    }
                    this._IsLazyDisabled = lastValue;

                    if (obj == null)
                    {
                        NullReferenceException up = new NullReferenceException(null, new Exception("The property \'" + Field.RelatedProperty.Name + "\' must return an instance of a collection"));
                        throw (up);
                    }

                    Type listof = Field.RelatedProperty.PropertyType;

                    listof.GetMethod("Clear").Invoke(obj, null);
                    foreach (BLLBase i in instances)
                    {
                        listof.GetMethod("Add").Invoke(obj, new object[] { i });
                    }
                }
            }
            else
            {
                if (instances.Length == 0)
                {
                    Field.SetPropertyValue(this, null);
                }
                else
                {
                    Field.SetPropertyValue(this, instances[0]);
                    if (instances.Length > 1)
                    {
                        Trace.TraceWarning("LoadingForeignKey-1-1: More than one instance was returned");
                    }
                }
            }

        }





        /// <summary>
        /// Carrega as informações de um campo marcado para lazy loading
        /// </summary>
        /// <remarks></remarks>
        protected virtual void LoadProperty(string @Property)
        {
            System.Reflection.PropertyInfo fieldP = this.GetType().GetProperty(@Property);
            if (fieldP == null)
            {
                throw (new ArgumentException("The property specified was not found.", "Property", null));
            }
            FieldAttribute[] fieldAttrs = (FieldAttribute[])(fieldP.GetCustomAttributes(typeof(FieldAttribute), true));
            if (fieldAttrs.Length == 0)
            {
                throw (new ArgumentException("The property specified is not a FieldAttribute type.", "Property", null));
            }

            FieldInfo field = new FieldInfo(fieldP);

            string sql = "";
            sql += " SELECT " + "\r\n";
            sql += " --FIELDS--" + "\r\n";
            sql += " FROM --TABLE-- " + "\r\n";
            sql += " WHERE --CONDITION--";




            sql = sql.Replace("--FIELDS--", this.CommandBuilder.QuoteIdentifier(field.DataFieldName));
            sql = sql.Replace("--TABLE--", BLLBase.GetSchemaAndTable(this));

            List<Parameter> @params = new List<Parameter>();
            string filter = "";

            foreach (FieldInfo f in GetFields(this.GetType()))
            {
                if (f.PrimaryKey)
                {
                    string paramName = f.DataFieldName.ToLower().Replace(" ", "_");
                    Parameter param = new Parameter(this.Connection, paramName, f.PropertyValue(this));

                    filter += " AND " + this.CommandBuilder.QuoteIdentifier(f.DataFieldName) + (" = " + param.ParameterPrefix + paramName);
                    @params.Add(param);
                }

            }
            if (string.IsNullOrEmpty(filter))
            {
                throw (new MissingPrimaryKeyException());
            }
            sql = sql.Replace("--CONDITION--", filter.Substring(5));

            Tenor.Data.DataTable rs = new Tenor.Data.DataTable(sql, @params.ToArray(), this.Connection);
            rs.Bind();

            if (rs.Rows.Count == 0)
            {
                throw (new RecordNotFoundException());
            }
            else
            {

                field.SetPropertyValue(this, rs.Rows[0][field.DataFieldName]);

            }



        }



        protected bool _IsLazyDisabled = false;
        /// <summary>
        /// Returns a value indicating whether or not this class is serializing.
        /// </summary>
        /// <returns>True or False</returns>
        /// <remarks></remarks>
        protected virtual bool LazyLoadingDisabled
        {
            get
            {
                return _IsLazyDisabled;
            }
        }


        /// <summary>
        /// Desativa o LazyLoading para as intâncias passadas.
        /// </summary>
        /// <param name="items"></param>
        /// <remarks></remarks>
        public static void DisableLazyLoading(BLLBase[] items)
        {
            foreach (BLLBase i in items)
            {
                i.ChangeLazyLoadingStatus(true);
            }
        }

        /// <summary>
        /// Ativa o LazyLoading para as intâncias passadas.
        /// </summary>
        /// <param name="items"></param>
        /// <remarks></remarks>
        public static void EnableLazyLoading(BLLBase[] items)
        {
            foreach (BLLBase i in items)
            {
                i.ChangeLazyLoadingStatus(false);
            }
        }


        private void ChangeLazyLoadingStatus(bool status)
        {
            this._IsLazyDisabled = status;

            foreach (ForeignKeyInfo fk in BLLBase.GetForeignKeys(this.GetType()))
            {
                if (fk.IsArray)
                {
                    ICollection col = (ICollection)(fk.PropertyValue(this, false));
                    if (col != null)
                    {
                        foreach (BLLBase item in col)
                        {
                            item.ChangeLazyLoadingStatus(status);
                        }
                    }
                }
                else
                {
                    BLLBase obj = (BLLBase)(fk.PropertyValue(this, false));
                    if (obj != null)
                    {
                        obj.ChangeLazyLoadingStatus(status);
                    }
                }
            }
        }
        #endregion

        #region " Save "

        /// <summary>
        /// Persiste no banco os valores das propriedades do tipo Field
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
            Save(isUpdate);
        }


        internal static string GetParamName(string paramPrefix, FieldInfo field)
        {
            return paramPrefix + field.DataFieldName.Replace(" ", "_");
        }

        internal string GetSaveSql(bool Update, string paramPrefix, List<Parameter> @params, ref FieldInfo AutoKeyField, System.Collections.Specialized.NameValueCollection SpecialValues, ConnectionStringSettings Connection)
        {
            string fieldValues = "";
            string clause = "";
            string fields = "";
            string values = "";

            AutoKeyField = null;
            foreach (FieldInfo field in GetFields(this.GetType()))
            {
                if (IsLoaded(field.RelatedProperty))
                {
                    string paramName = GetParamName(paramPrefix, field);
                    Parameter param = new Parameter(Connection, paramName, field.PropertyValue(this));
                    if (field.AutoNumber)
                    {
                        AutoKeyField = field;
                    }
                    else
                    {



                        if (!string.IsNullOrEmpty(fields))
                        {
                            fields += ", ";
                        }
                        fields += this.CommandBuilder.QuoteIdentifier(field.DataFieldName);
                        if (!string.IsNullOrEmpty(values))
                        {
                            values += ", ";
                        }


                        if (SpecialValues == null || string.IsNullOrEmpty(SpecialValues[paramName]))
                        {
                            values += param.ParameterPrefix + paramName;
                        }
                        else
                        {
                            values += SpecialValues[paramName];
                        }

                        if (!field.PrimaryKey)
                        {
                            if (!string.IsNullOrEmpty(fieldValues))
                            {
                                fieldValues += ", ";
                            }
                            fieldValues += this.CommandBuilder.QuoteIdentifier(field.DataFieldName) + " = " + param.ParameterPrefix + paramName;
                        }


                    }

                    if (field.PrimaryKey)
                    {
                        if (!string.IsNullOrEmpty(clause))
                        {
                            clause += " AND ";
                        }
                        clause += "" + this.CommandBuilder.QuoteIdentifier(field.DataFieldName) + " = " + param.ParameterPrefix + paramName;
                    }

                    @params.Add(param);
                }
            }

            string query = "";
            if (Update)
            {
                query = "UPDATE ---TABLE_NAME--- SET ---FIELD_VALUES--- WHERE ---CLAUSE---";
            }
            else
            {
                query = "INSERT INTO ---TABLE_NAME--- (---FIELDS---) VALUES (---VALUES---)";
            }

            query = query.Replace("---TABLE_NAME---", GetSchemaAndTable(this));
            query = query.Replace("---FIELD_VALUES---", fieldValues);
            query = query.Replace("---CLAUSE---", clause);
            query = query.Replace("---FIELDS---", fields);
            query = query.Replace("---VALUES---", values);

            return query;
        }

        /// <summary>
        /// Persiste no banco os valores das propriedades do tipo Field
        /// </summary>
        /// <param name="Update">Determina se é um update ou um insert</param>
        /// <remarks></remarks>
        public virtual void Save(bool Update)
        {
            if (Validate())
            {
                FieldInfo AutoKeyField = null;

                List<Parameter> @params = new List<Parameter>();
                string query = GetSaveSql(Update, this.GetType().Name, @params, ref AutoKeyField, null, this.Connection);

                int result = Helper.AtualizarBanco(this.Connection, query, @params);

                if (!Update && (AutoKeyField != null))
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
            if (Validate())
            {
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

                List<Parameter> @params = new List<Parameter>();
                string insertQuery = GetSaveSql(false, this.GetType().Name, @params, ref AutoKeyField, null, this.Connection);

                //updateQuery está com new List<Parameter> pois os parametros já foram definidos no insertQuery.
                string updateQuery = GetSaveSql(true, this.GetType().Name, new List<Parameter>(), ref AutoKeyField, null, this.Connection);

                StringBuilder query = new StringBuilder();
                StringBuilder queryDeclares = new StringBuilder();

                StringBuilder querySelect = new StringBuilder();
                StringBuilder queryIsNull = new StringBuilder();
                StringBuilder queryWhere = new StringBuilder();

                foreach (FieldInfo f in fieldsPrimary)
                {
                    Data.Parameter p = new Data.Parameter(this.Connection, f.RelatedProperty.Name + "__test", f.PropertyValue(this));
                    querySelect.Append(",");
                    querySelect.Append(p.ParameterPrefix + this.GetType().Name + f.RelatedProperty.Name);
                    querySelect.Append(" = ");
                    querySelect.Append(f.DataFieldName);
                }


                foreach (FieldInfo f in fields)
                {
                    Data.Parameter p = new Data.Parameter(this.Connection, f.RelatedProperty.Name + "__test", f.PropertyValue(this));
                    queryDeclares.AppendLine("DECLARE " + p.ParameterPrefix + f.RelatedProperty.Name + "_ " + p.DbTypeName);
                    querySelect.Append(",");
                    querySelect.Append(p.ParameterPrefix + f.RelatedProperty.Name + "_");
                    querySelect.Append(" = ");
                    querySelect.Append(f.DataFieldName);

                    queryWhere.Append(" AND ");
                    queryWhere.Append(f.DataFieldName);
                    queryWhere.Append(" = ");
                    queryWhere.Append(p.ParameterPrefix + this.GetType().Name + f.RelatedProperty.Name);

                    queryIsNull.Append(" AND ");
                    queryIsNull.Append(p.ParameterPrefix + f.RelatedProperty.Name + "_ IS NULL");

                }
                query.Append(queryDeclares);
                query.AppendLine();
                query.Append("SELECT ");

                querySelect.Remove(0, 1);
                query.Append(querySelect);
                query.Append(" FROM ");
                query.Append(BLLBase.GetSchemaAndTable(this));
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


                DataTable result = Helper.ConsultarBanco(this.Connection, query.ToString(), @params);

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
            string clause = "";
            List<Parameter> @params = new List<Parameter>();
            foreach (FieldInfo i in GetFields(this.GetType()))
            {
                if (i.PrimaryKey)
                {
                    Parameter param = new Parameter(this.Connection, i.DataFieldName.Replace(" ", "_"), i.PropertyValue(this));
                    clause += " AND " + CommandBuilder.QuoteIdentifier(i.DataFieldName) + (" = " + param.ParameterPrefix + i.DataFieldName.Replace(" ", "_"));
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

            string query = "DELETE FROM " + GetSchemaAndTable(this) + " WHERE " + clause;

            Helper.AtualizarBanco(this.Connection, query, @params);
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

        #region " Cacheable "
        [Browsable(false)]
        public virtual bool Cacheable
        {
            get
            {
                return false;
            }
        }

        private string GetCacheKey()
        {

            FieldInfo[] chavesPrimaria = BLLBase.GetPrimaryKeys(this.GetType());
            if (chavesPrimaria.Length == 0)
            {
                throw (new MissingPrimaryKeyException());
            }
            string chavePrimaria = "";
            foreach (FieldInfo f in chavesPrimaria)
            {
                chavePrimaria += "," + f.PropertyValue(this).ToString();
            }


            if (chavePrimaria.Length == 0)
            {
                throw (new MissingPrimaryKeyException());
            }
            else
            {
                chavePrimaria = chavePrimaria.Substring(1);
            }
            return chavePrimaria;
        }


        /// <summary>
        /// Procura a instancia no cache e a inclue se não for encontrada.
        /// </summary>
        /// <returns>Verdadeiro se o item foi lido do cache. Falso se a intancia não foi encontrada. Caso retorne falso deverá ser carregado os dados da mesma.</returns>
        /// <remarks></remarks>
        private bool LoadFromCache()
        {

            string chavePrimaria = GetCacheKey();

            System.Web.Caching.Cache Cache = null;
            if (System.Web.HttpContext.Current != null)
            {
                Cache = System.Web.HttpContext.Current.Cache;
            }

            if (Cache != null)
            {
                Dictionary<string, BLLBase> obj = (Dictionary<string, BLLBase>)(Cache.Get(ChaveCache));

                if (obj == null)
                {
                    obj = new Dictionary<string, BLLBase>();
                    Cache.Add(ChaveCache, obj, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 60, 0), System.Web.Caching.CacheItemPriority.Default, null);
                }


                object sync = new object();
                lock (sync)
                {
                    if (obj.ContainsKey(chavePrimaria) && (obj[chavePrimaria] != null))
                    {
                        BLLBase item = obj[chavePrimaria];
                        item.CopyTo(this);
                        return true;
                    }
                }
            }
            return false;
        }

        private void SaveToCache()
        {
            System.Web.Caching.Cache Cache = null;
            if (System.Web.HttpContext.Current != null)
            {
                Cache = System.Web.HttpContext.Current.Cache;
            }
            if (Cache != null)
            {
                object sync = new object();
                lock (sync)
                {
                    Dictionary<string, BLLBase> obj = (Dictionary<string, BLLBase>)(Cache.Get(ChaveCache));
                    if (obj == null)
                    {
                        obj = new Dictionary<string, BLLBase>();
                        Cache.Add(ChaveCache, obj, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 60, 0), System.Web.Caching.CacheItemPriority.Default, null);
                    }
                    string chavePrimaria = GetCacheKey();
                    if (obj.ContainsKey(chavePrimaria))
                    {
                        //A chave do cache existe, mas por algum motivo não está lá.
                        obj[chavePrimaria] = this;
                    }
                    else
                    {
                        //A chave do cache não existe
                        obj.Add(chavePrimaria, this);
                    }
                }
            }
        }




        /// <summary>
        /// Copia os campos private para o objeto especificado.
        /// </summary>
        /// <param name="obj"></param>
        /// <remarks></remarks>
        private void CopyTo(BLLBase obj)
        {
            if (obj.GetType() != this.GetType())
            {
                throw (new InvalidCastException());
            }
            System.Reflection.FieldInfo[] fields = this.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(obj, field.GetValue(this));
            }

        }

        #endregion

        #region " Reflection "

        /// <summary>
        /// Indica status do lazyloading na propriedade
        /// </summary>
        /// <param name="Property"></param>
        /// <returns>True se a propriedade ainda não foi carregada.
        /// False se já carregada.
        /// </returns>
        /// <remarks></remarks>
        private bool IsLoaded(System.Reflection.PropertyInfo @Property)
        {
            System.Reflection.FieldInfo field = this.GetType().GetField("_" + @Property.Name + "_firstaccess", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (field != null)
            {
                return !System.Convert.ToBoolean(field.GetValue(this));
            }
            else
            {
                return true;
            }
        }


        /// <summary>
        /// Pega uma lista de FieldInfos com todos os fields da instância passada
        /// </summary>
        /// <param name="Instance">A instância do objeto que contém os campos</param>
        /// <returns>Uma lista de FieldInfos</returns>
        /// <remarks></remarks>
        [Obsolete()]
        private static List<FieldInfo> GetFields(BLLBase Instance)
        {
            return new List<FieldInfo>(GetFields(Instance.GetType()));
        }

        /// <summary>
        /// Pega uma lista de FieldInfos com todos os fields da instância passada
        /// </summary>
        /// <param name="InstanceType">O tipo do objeto que contém os campos</param>
        /// <returns>Uma lista de FieldInfos</returns>
        /// <remarks></remarks>
        internal static FieldInfo[] GetFields(Type InstanceType)
        {
            return GetFields(InstanceType, null);
        }
        /// <summary>
        /// Pega uma lista de FieldInfos com todos os fields da instância passada
        /// </summary>
        /// <param name="InstanceType">O tipo do objeto que contém os campos</param>
        /// <param name="IsPrimaryKey"></param>
        /// <returns>Uma lista de FieldInfos</returns>
        /// <remarks></remarks>
        internal static FieldInfo[] GetFields(Type InstanceType, Nullable<bool> IsPrimaryKey)
        {
            return GetFields(InstanceType, IsPrimaryKey, null);
        }


        /// <summary>
        /// Pega uma lista de FieldInfos com todos os fields da instância passada
        /// </summary>
        /// <param name="InstanceType">O tipo do objeto que contém os campos</param>
        /// <param name="IsPrimaryKey"></param>
        /// <returns>Uma lista de FieldInfos</returns>
        /// <remarks></remarks>
        internal static FieldInfo[] GetFields(Type InstanceType, System.Nullable<bool> IsPrimaryKey, string[] Filter)
        {
            List<FieldInfo> returnValue = new List<FieldInfo>();
            foreach (System.Reflection.PropertyInfo i in InstanceType.GetProperties())
            {
                if (((FieldAttribute[])(i.GetCustomAttributes(typeof(FieldAttribute), true))).Length > 0)
                {
                    //Dim attribute As FieldAttribute = CType(i.GetCustomAttributes(GetType(FieldAttribute), True), FieldAttribute())(0)
                    FieldInfo campo = new FieldInfo(i);

                    if (!IsPrimaryKey.HasValue || (campo.PrimaryKey == IsPrimaryKey.Value))
                    {
                        if (Filter == null || Filter.Length == 0 || Array.IndexOf<string>(Filter, i.Name) > -1)
                        {
                            returnValue.Add(campo);
                        }
                    }
                }
            }
            return returnValue.ToArray();
        }

        private static FieldInfo[] GetPrimaryKeys(Type InstanceType)
        {
            return GetFields(InstanceType, true);
        }

        /// <summary>
        /// Pega uma lista de FieldInfos com todos os fields da instância passada
        /// </summary>
        /// <param name="InstanceType">O tipo da instancia</param>
        /// <returns>Uma lista de FieldInfos</returns>
        /// <remarks></remarks>
        internal static ForeignKeyInfo[] GetForeignKeys(Type InstanceType)
        {
            List<ForeignKeyInfo> res = new List<ForeignKeyInfo>();
            foreach (System.Reflection.PropertyInfo i in InstanceType.GetProperties())
            {
                ForeignKeyAttribute[] foreignkeys = (ForeignKeyAttribute[])(i.GetCustomAttributes(typeof(ForeignKeyAttribute), true));
                if (foreignkeys.Length > 0)
                {
                    ForeignKeyInfo foreign = new ForeignKeyInfo(i);

                    res.Add(foreign);
                }
            }
            return res.ToArray();
        }

        private static SpecialFieldInfo[] GetSpecialFields(Type instanceType)
        {
            List<SpecialFieldInfo> res = new List<SpecialFieldInfo>();
            foreach (System.Reflection.PropertyInfo i in instanceType.GetProperties())
            {
                SpecialFieldAttribute[] sps = (SpecialFieldAttribute[])(i.GetCustomAttributes(typeof(SpecialFieldAttribute), true));
                if (sps.Length > 0)
                {
                    SpecialFieldInfo spInfo = new SpecialFieldInfo(i);
                    res.Add(spInfo);
                }
            }
            return res.ToArray();
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
        /// <summary>
        /// Compara duas instancias utilizando suas chaves primárias definidas.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool operator ==(BLLBase x, BLLBase y)
        {
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

        public static bool operator !=(BLLBase x, BLLBase y)
        {
            return !(x == y);
        }
        #endregion

    }
}