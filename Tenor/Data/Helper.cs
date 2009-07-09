using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Data.Common;
using System.Configuration;
using Tenor.Data.Dialects;


namespace Tenor.Data
{
    /// <summary>
    /// Autor: Rachel dos Santos Carvalho
    /// </summary>
    /// <remarks></remarks>
    public sealed class Helper
    {

        /// <summary>
        /// This is the default timeout of any DbCommand.
        /// TODO: Consider moving this to ConfigurationManager.
        /// </summary>
        public const int DefaultTimeout = 260;

        /// <summary>
        /// This is an static class.
        /// </summary>
        private Helper()
        { }

        /// <summary>
        /// Creates the DbProviderFactory. 
        /// TODO: Move all this stuff to dialects.
        /// </summary>
        internal static DbProviderFactory GetFactory(ConnectionStringSettings Connection)
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory(Connection.ProviderName);
            return factory;
        }
        /// <summary>
        /// Creates the CommandBuilder. 
        /// TODO: Move all this stuff to dialects.
        /// </summary>
        internal static DbCommandBuilder GetCommandBuilder(ConnectionStringSettings connection)
        {
            DbCommandBuilder builder = GetFactory(connection).CreateCommandBuilder();
            return builder;
        }

        /// <summary>
        /// Converts a system type name into a database type.
        /// </summary>
        /// <param name="systemType">A system type</param>
        /// <param name="factory">A system database factory</param>
        /// <returns>The database name.</returns>
        /// <exception cref="Tenor.TenorException">Throws TenorException when cannot convert the desired system type.</exception>
        /// <exception cref="System.ArgumentNullException">Throws ArgumentNullException when a null parameter was supplied.</exception>
        /// <remarks></remarks>
        public static string GetDbTypeName(Type systemType, DbProviderFactory factory)
        {
            if (systemType == null)
            {
                throw (new ArgumentNullException("systemType"));
            }
            else if (factory == null)
            {
                throw (new ArgumentNullException("factory"));
            }

            string typeName = string.Empty;
            DbType tipo = DbType.String;

            System.ComponentModel.TypeConverter conversor = System.ComponentModel.TypeDescriptor.GetConverter(tipo);
            if (conversor == null)
            {
                throw (new TenorException("GetDbTypeName: Cannot create converter."));
            }
            tipo = (DbType)(conversor.ConvertFrom(systemType.Name));




            System.Data.Common.DbParameter param = factory.CreateParameter();
            if (param == null)
            {
                throw (new TenorException("GetDbTypeName: Cannot create parameter."));
            }
            param.DbType = tipo;

            foreach (System.Reflection.PropertyInfo prop in param.GetType().GetProperties())
            {
                //This loop is necessary to set dbms specific Parameter properties.
                if (prop.Name.Contains("DbType") && !prop.Name.Equals("DbType"))
                {
                    typeName = prop.GetValue(param, new object[] { }).ToString();
                    break;
                }
            }

            return typeName;

        }

        /// <summary>
        /// Executes a query on the database and them returns a datatable.
        /// </summary>
        /// <param name="sqlSelect">A SQL query to execute.</param>
        /// <param name="parameters">Parameters collection.</param>
        /// <returns>A <see cref="System.Data.DataTable">DataTable</see> with results of the query.</returns>
        public static DataTable QueryData(string sqlSelect, TenorParameter[] parameters)
        {
            return QueryData(null, sqlSelect, parameters);
        }

        /// <summary>
        /// Executes a query on the database and them returns a datatable.
        /// </summary>
        /// <param name="connectionString">The connection parameters.</param>
        /// <param name="sqlSelect">A SQL query to execute.</param>
        /// <param name="parameters">Parameters collection.</param>
        /// <returns>A <see cref="System.Data.DataTable">DataTable</see> with results of the query.</returns>
        public static DataTable QueryData(ConnectionStringSettings connectionString, string sqlSelect, TenorParameter[] parameters)
        {
            if (connectionString == null)
                throw new ArgumentNullException("connectionString");
            DbProviderFactory factory = GetFactory(connectionString);

            DataTable dtRetorno = null;

            DbConnection conn = factory.CreateConnection();
            conn.ConnectionString = connectionString.ConnectionString;
            DbCommand cmd = null;
            DbDataReader reader;

            try
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    Tenor.Diagnostics.Debug.DebugSQL("Helper: ConsultarBanco()", sqlSelect, parameters, connectionString);
                }
            }
            catch (Exception ex)
            {
                Diagnostics.Debug.HandleError(ex);
            }



            try
            {
                cmd = conn.CreateCommand();
                cmd.CommandText = sqlSelect;
                cmd.CommandTimeout = Helper.DefaultTimeout;
                if (parameters != null)
                {
                    foreach (TenorParameter param in parameters)
                    {
                        cmd.Parameters.Add(param.ToDbParameter(factory));
                    }
                }
                conn.Open();
                reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    dtRetorno = new DataTable();
                    dtRetorno.Load(reader);
                }



            }
            catch (Exception up)
            {
                if (cmd != null)
                {
                    up.Data.Add("CommandText", cmd.CommandText);
                }
                throw (up);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Dispose();
            }

            return dtRetorno;
        }

        /// <summary>
        /// Executes a query on the database and them returns the scalar value.
        /// </summary>
        /// <param name="sqlSelect">A SQL query to execute.</param>
        /// <param name="parameters">Parameters collection.</param>
        /// <returns>The scalar value returned from the database. It could be any valued type, string, or null.</returns>
        public static object QueryValue(string sqlSelect, TenorParameter[] parameters)
        {
            return QueryValue(Tenor.BLL.BLLBase.SystemConnection, sqlSelect, parameters);
        }

        /// <summary>
        /// Executes a query on the database and them returns the scalar value.
        /// </summary>
        /// <param name="connectionString">The connection parameters.</param>
        /// <param name="sqlSelect">A SQL query to execute.</param>
        /// <param name="parameters">Parameters collection.</param>
        /// <returns>The scalar value returned from the database. It could be any valued type, string, or null.</returns>
        public static object QueryValue(ConnectionStringSettings connectionString, string sqlSelect, TenorParameter[] parameters)
        {
            object valor = null;
            DataTable tabela = QueryData(connectionString, sqlSelect, parameters);
            if (tabela != null && tabela.Rows.Count > 0)
            {
                valor = tabela.Rows[0][0];
            }
            return valor;
        }

        /// <summary>
        /// Executes a query on the database.
        /// </summary>
        /// <param name="sqlSelect">A SQL query to execute.</param>
        /// <param name="parameters">Parameters collection.</param>
        public static object UpdateData(string sql, TenorParameter[] parameters)
        {
            return UpdateData(sql, parameters, null);
        }

        /// <summary>
        /// Executes a query on the database.
        /// </summary>
        /// <param name="sql">SQL query</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="connection">Connection</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static object UpdateData(string sql, TenorParameter[] parameters, ConnectionStringSettings connection)
        {
            return UpdateData(sql, parameters, connection, (string)null);
        }

        /// <summary>
        /// Executes a query on the database.
        /// </summary>
        /// <param name="sql">SQL query</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="connection">Connection</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static object UpdateData(string sql, TenorParameter[] parameters, ConnectionStringSettings connection, string secondSql)
        {
            if (connection == null)
                connection = BLL.BLLBase.SystemConnection;

            IDialect dialect = DialectFactory.CreateDialect(connection);

            DbConnection conn = dialect.Factory.CreateConnection();
            conn.ConnectionString = connection.ConnectionString;
            DbTransaction transaction = null;
            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();
                object retVal = ExecuteQuery(sql, parameters, transaction, dialect);
                if (!string.IsNullOrEmpty(secondSql))
                {
                    retVal = ExecuteQuery(secondSql, null, transaction, dialect);
                }
                transaction.Commit();
                return retVal;
            }
            catch
            {
                if (transaction != null) transaction.Rollback();
                throw;
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Dispose();
            }
        }

        internal static object ExecuteQuery(string sql, TenorParameter[] parameters, DbTransaction transaction, IDialect dialect)
        {
            DbConnection conn = transaction.Connection;
            DbCommand cmd;

            object returnValue = 0;

            cmd = conn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = sql;
            cmd.CommandTimeout = Helper.DefaultTimeout;
            
            if (parameters != null)
            {
                foreach (TenorParameter param in parameters)
                {
                    cmd.Parameters.Add(param.ToDbParameter(dialect.Factory));
                }
            }

            try
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Text.StringBuilder traceInfo = new System.Text.StringBuilder();
                    traceInfo.AppendLine("Tenor.Data.Helper.ExecuteQuery()");
                    traceInfo.AppendLine(" > " + conn.ConnectionString);
                    if (parameters != null)
                    {
                        foreach (TenorParameter p in parameters)
                        {
                            if (p.Value == null)
                            {
                                traceInfo.AppendLine(" > " + p.ParameterName + ": (null)");
                            }
                            else
                            {
                                traceInfo.AppendLine(" > " + p.ParameterName + ": " + p.Value.ToString());
                            }
                        }
                    }
                    traceInfo.AppendLine(sql);

                    string st = Environment.StackTrace;
                    string fimDaondeNaoImporta = "get_StackTrace()" + Environment.NewLine;
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

            object result = cmd.ExecuteScalar();
            if (result != null && !result.Equals(DBNull.Value))
            {
                returnValue = result;
            }

            /*else if (dialect != null && (!string.IsNullOrEmpty(dialect.IdentityBeforeQuery) || !string.IsNullOrEmpty(dialect.IdentityAfterQuery)) && !dialect.GetIdentityOnSameCommand)
            {
                string currentQuery = cmd.CommandText;
                cmd.CommandText = dialect.IdentityBeforeQuery;
                object value1 = cmd.ExecuteScalar();


                cmd.CommandText = currentQuery;
                returnValue = cmd.ExecuteNonQuery();

                cmd.CommandText = dialect.IdentityAfterQuery;
                object value2 = cmd.ExecuteScalar();
            }
            else
            {
                returnValue = cmd.ExecuteNonQuery();
            }*/

            /*
            if (sql.Trim().StartsWith("INSERT INTO", StringComparison.CurrentCultureIgnoreCase))
            {
                if (cmd.Connection.GetType() == typeof(System.Data.SqlClient.SqlConnection))
                {
                    cmd.CommandText += " SELECT SCOPE_IDENTITY()";
                }
                else if (cmd.Connection.GetType().FullName.Contains("MySQL"))
                {
                    cmd.CommandText += " SELECT LAST_INSERT_ID()";
                }
                else if (cmd.Connection.GetType().FullName.Contains("SQLite"))
                {
                    cmd.CommandText += ";" + Environment.NewLine + "SELECT LAST_INSERT_ROWID();";
                }
                else
                {
                    cmd.CommandText += " SELECT -1";
                }
						
                object resultado = cmd.ExecuteScalar();
                if (! resultado.Equals(DBNull.Value))
                {
                    retorno = System.Convert.ToInt32(resultado);
                }
						
            }
            else
            {
                retorno = cmd.ExecuteNonQuery();
            }
            */

            return returnValue;

        }

        [Obsolete()]
        internal static string GetParameterPrefix(ConnectionStringSettings Connection)
        {
            return GetParameterPrefix(GetFactory(Connection));
        }

        [Obsolete()]
        internal static string GetParameterPrefix(DbProviderFactory factory)
        {
            string param = factory.CreateParameter().GetType().Name;
            switch (param)
            {
                case "SqlParameter":
                case "SQLiteParameter":
                    return "@";
                case "OracleParameter":
                    return ":";
                default:
                    throw new InvalidOperationException("Invalid provider");
            }

        }

    }
}