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


namespace Tenor
{
	namespace Data
	{
		/// <summary>
		/// Autor: Rachel dos Santos Carvalho
		/// </summary>
		/// <remarks></remarks>
		public class Helper
		{
			
			
			public const int DefaultTimeout = 260;
			
			/// <summary>
			/// A classe não pode ser instanciada
			/// </summary>
			/// <remarks></remarks>
			private Helper()
			{
			}
			
			/// <summary>
			/// Cria um factory para criação de objetos baseados na provider
			/// </summary>
			/// <param name="Connection">ConnectionString da Web.Config</param>
			/// <returns>Factory específica</returns>
			/// <remarks></remarks>
			internal static DbProviderFactory GetFactory(ConnectionStringSettings Connection)
			{
				DbProviderFactory factory = DbProviderFactories.GetFactory(Connection.ProviderName);
				return factory;
			}
            internal static DbCommandBuilder GetCommandBuilder(ConnectionStringSettings connection)
            {
                DbCommandBuilder builder = GetFactory(connection).CreateCommandBuilder();
                return builder;
            }
			
			///' <summary>
			///' Cria conexão e adapter para a ConnectionString especificada.
			///' </summary>
			///' <param name="Connection">ConnectionString da config</param>
			///' <returns>Array de Object com Connection e Adapter</returns>
			///' <remarks></remarks>
			//<Obsolete(Nothing, True)> _
			//Friend Shared Function CreateConnection(ByVal Connection As ConnectionStringSettings) As System.Data.Common.DbConnection
			//    If Connection Is Nothing Then
			//        Throw New NullReferenceException()
			//    End If
			//    Select Case Connection.ProviderName.ToLower()
			//        Case "system.data.sqlclient"
			//            Return New SqlConnection(Connection.ConnectionString)
			//        Case "system.data.odbc"
			//            Return New Odbc.OdbcConnection(Connection.ConnectionString)
			//        Case Else
			//            Throw New Exception("The provider specified on connection '" + Connection.Name + "' cannot be found or is not supported.")
			//    End Select
			
			//End Function
			
			///' <summary>
			///' A estrutura de um SqlParameter
			///' </summary>
			//<Serializable()> _
			//Public Structure Parametro
			//    Dim Nome As String
			//    Dim Tipo As DbType
			//    Dim Valor As Object
			
			//    Public Sub New(ByVal nome As String, ByVal valor As Object)
			//        Me.Nome = nome
			//        Me.Valor = valor
			//    End Sub
			
			//    Public Sub New(ByVal nome As String, ByVal tipo As DbType, ByVal valor As Object)
			//        Me.Nome = nome
			//        Me.Tipo = tipo
			//        Me.Valor = valor
			//    End Sub
			//End Structure
			
			/// <summary>
			/// Resolve o nome do tipo do sistema no banco de dados
			/// </summary>
			/// <param name="systemType">A system type</param>
			/// <param name="factory">A system database factory</param>
			/// <returns></returns>
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
                    throw (new Exception("GetDbTypeName: Cannot create converter."));
                }
                tipo = (DbType)(conversor.ConvertFrom(systemType.Name));




                System.Data.Common.DbParameter param = factory.CreateParameter();
                if (param == null)
                {
                    throw (new Exception("GetDbTypeName: Cannot create parameter."));
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
			/// Consulta o banco com os parâmetros passados e retorna uma DataTable
			/// </summary>
			/// <param name="sqlSelect">SQL de select</param>
			/// <param name="parametros">Array de Parametros</param>
			/// <returns>DataTable com os resultados, ou Nothing, caso nada tenha sido retornado.</returns>
			public static DataTable ConsultarBanco(string sqlSelect, TenorParameter[] parametros)
			{
				return ConsultarBanco(null, sqlSelect, parametros);
			}
			
			/// <summary>
			/// Consulta o banco com os parâmetros passados e retorna uma DataTable
			/// </summary>
			/// <param name="ConnectionString">Conexão no web.config</param>
			/// <param name="sqlSelect">SQL de select</param>
			/// <param name="parametros">Coleção de Parametros</param>
			/// <returns>DataTable com os resultados, ou Nothing, caso nada tenha sido retornado.</returns>
			public static DataTable ConsultarBanco(ConnectionStringSettings connectionString, string sqlSelect, TenorParameter[] parametros)
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
                        Tenor.Diagnostics.Debug.DebugSQL("Helper: ConsultarBanco()", sqlSelect, parametros, connectionString);
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
					if (parametros != null)
					{
                        foreach (TenorParameter param in parametros)
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
			/// Consulta o banco com os parâmetros passados e retorna o valor
			/// </summary>
			/// <param name="sqlSelect">SQL de select</param>
			/// <param name="parametros">Coleção de Parametros</param>
			/// <returns>O resultado, ou Nothing, caso nada tenha sido retornado.</returns>
			public static object ConsultarValor(string sqlSelect, TenorParameter[] parametros)
			{
				return ConsultarValor(Tenor.BLL.BLLBase.SystemConnection, sqlSelect, parametros);
			}
			
			/// <summary>
			/// Consulta o banco com os parâmetros passados e retorna o valor
			/// </summary>
			/// <param name="ConnectionString">Conexão no web.config</param>
			/// <param name="sqlSelect">SQL de select</param>
			/// <param name="parametros">Coleção de Parametros</param>
			/// <returns>O resultado, ou Nothing, caso nada tenha sido retornado.</returns>
			public static object ConsultarValor(ConnectionStringSettings connectionString, string sqlSelect, TenorParameter[] parametros)
			{
				object valor = null;
				DataTable tabela = ConsultarBanco(connectionString, sqlSelect, parametros);
				if (tabela != null&& tabela.Rows.Count > 0)
				{
					valor = tabela.Rows[0][0];
				}
				return valor;
			}
			
			/// <summary>
            /// Executes a query on your database.
			/// </summary>
			/// <param name="sql">SQL query</param>
			/// <param name="parametros">Parameters</param>
			/// <returns></returns>
			/// <remarks></remarks>
			public static int ExecuteQuery(string sql, TenorParameter[] parameters)
			{
				return ExecuteQuery(BLL.BLLBase.SystemConnection, sql, parameters, null, false);
			}
			
			/// <summary>
			/// Executa funções de atualização no banco de dados
			/// </summary>
			/// <param name="ConnectionString">Conexão no web.config</param>
			/// <param name="sql">SQL de atualização (UPDATE, INSERT INTO, DELETE)</param>
			/// <param name="parametros">Coleção de Parametros</param>
			/// <returns>Número de linhas afetadas para Update e Delete;
			/// Índice atual da chave primária para INSERT INTO</returns>
			/// <remarks></remarks>
			public static int ExecuteQuery(ConnectionStringSettings connection, string sql, TenorParameter[] parameters, string identityQuery, bool runOnSameQuery)
			{
				DbProviderFactory factory = GetFactory(connection);
				
				DbConnection conn = factory.CreateConnection();
				conn.ConnectionString = connection.ConnectionString;
                DbTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();
                    int retVal = ExecuteQuery(connection, transaction, sql, parameters, identityQuery, runOnSameQuery);
                    transaction.Commit();
                    return retVal;
                }
                catch
                {
                    transaction.Rollback();
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

            internal static int ExecuteQuery(ConnectionStringSettings connSettings, DbTransaction transaction, string sql, TenorParameter[] parameters, string identityQuery, bool runOnSameQuery)
            {
                DbConnection conn = transaction.Connection;
                DbProviderFactory factory = GetFactory(connSettings);
                DbCommand cmd;

                int returnValue = -99;

                cmd = conn.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandText = sql;
                cmd.CommandTimeout = Helper.DefaultTimeout;
                if (parameters != null)
                {
                    foreach (TenorParameter param in parameters)
                    {
                        cmd.Parameters.Add(param.ToDbParameter(factory));
                    }
                }



                try
                {
                    if (System.Diagnostics.Debugger.IsAttached)
                    {

                        System.Text.StringBuilder traceInfo = new System.Text.StringBuilder();
                        traceInfo.AppendLine("Helper: AtualizarBanco()");
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

                if (!string.IsNullOrEmpty(identityQuery) && runOnSameQuery)
                {
                    cmd.CommandText += "\r\n" + identityQuery;
                    object result = cmd.ExecuteScalar();
                    if (!result.Equals(DBNull.Value))
                    {
                        returnValue = Convert.ToInt32(result);
                    }
                }
                else if (!string.IsNullOrEmpty(identityQuery) && !runOnSameQuery)
                {
                    returnValue = cmd.ExecuteNonQuery();

                    cmd.CommandText = identityQuery;
                    object result = cmd.ExecuteScalar();
                    if (!result.Equals(DBNull.Value))
                    {
                        returnValue = Convert.ToInt32(result);
                    }
                }
                else
                {
                    returnValue = cmd.ExecuteNonQuery();
                }

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
	
}
