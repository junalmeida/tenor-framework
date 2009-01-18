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
			/// <param name="systemType">Tipo de sistema</param>
			/// <param name="instance">Instância de BLL Base</param>
			/// <returns></returns>
			/// <remarks></remarks>
			public static string GetDbTypeName(Type systemType, BLL.BLLBase instance)
			{
				if (systemType == null)
				{
					throw (new ArgumentNullException("systemType"));
				}
				else if (instance == null)
				{
					throw (new ArgumentNullException("instance"));
				}
				
				string typeName = string.Empty;
				DbType tipo = DbType.String;
				
				System.ComponentModel.TypeConverter conversor = System.ComponentModel.TypeDescriptor.GetConverter(tipo);
				if (conversor == null)
				{
					throw (new Exception("GetDbTypeName: Cannot create converter."));
				}
				tipo = (DbType) (conversor.ConvertFrom(systemType.Name));
				
				System.Data.Common.DbProviderFactory fac = instance.GetFactory();
				if (fac == null)
				{
					throw (new Exception("instance.GetFactory()"));
				}
				else
				{
					
					
					System.Data.Common.DbParameter param = fac.CreateParameter();
					if (param == null)
					{
						throw (new Exception("GetDbTypeName: Cannot create parameter."));
					}
					param.DbType = tipo;
					
					foreach (System.Reflection.PropertyInfo prop in param.GetType().GetProperties())
					{
						if (prop.Name.Contains("DbType") && ! prop.Name.Equals("DbType"))
						{
							typeName = prop.GetValue(param, new object[] {}).ToString();
							break;
						}
					}
					
					return typeName;
				}
			}
			
			/// <summary>
			/// Consulta o banco com os parâmetros passados e retorna uma DataTable
			/// </summary>
			/// <param name="sqlSelect">SQL de select</param>
			/// <param name="parametros">Array de Parametros</param>
			/// <returns>DataTable com os resultados, ou Nothing, caso nada tenha sido retornado.</returns>
			public static DataTable ConsultarBanco(string sqlSelect, List<Parameter> parametros)
			{
				if (ConfigurationManager.ConnectionStrings.Count == 0)
				{
					throw (new Exception("Cannot find a ConnectionString. Check your configuration file."));
				}
				return ConsultarBanco(ConfigurationManager.ConnectionStrings[0], sqlSelect, parametros);
			}
			
			/// <summary>
			/// Consulta o banco com os parâmetros passados e retorna uma DataTable
			/// </summary>
			/// <param name="ConnectionString">Conexão no web.config</param>
			/// <param name="sqlSelect">SQL de select</param>
			/// <param name="parametros">Coleção de Parametros</param>
			/// <returns>DataTable com os resultados, ou Nothing, caso nada tenha sido retornado.</returns>
			public static DataTable ConsultarBanco(ConnectionStringSettings ConnectionString, string sqlSelect, List<Parameter> parametros)
			{
				DbProviderFactory factory = GetFactory(ConnectionString);
				
				DataTable dtRetorno = null;
				
				DbConnection conn = factory.CreateConnection();
				conn.ConnectionString = ConnectionString.ConnectionString;
				DbCommand cmd = null;
				DbDataReader reader;
				
				
				try
				{
					if (System.Diagnostics.Debugger.IsAttached)
					{
						
						System.Text.StringBuilder traceInfo = new System.Text.StringBuilder();
						traceInfo.AppendLine("Helper: ConsultarBanco()");
						traceInfo.AppendLine(" > " + ConnectionString.ConnectionString);
						if (parametros != null)
						{
							foreach (Parameter p in parametros)
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
						}
						
						traceInfo.AppendLine(sqlSelect);
						
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
				
				
				
				try
				{
					cmd = conn.CreateCommand();
					cmd.CommandText = sqlSelect;
					cmd.CommandTimeout = Helper.DefaultTimeout;
					if (parametros != null)
					{
                        foreach (Parameter param in parametros)
                        {

                            System.Data.Common.DbParameter parametro = factory.CreateParameter();
                            parametro.ParameterName = param.ParameterName;
                            parametro.Value = param.Value;

                            parametro.DbType = param.DbType;

                            cmd.Parameters.Add(parametro);
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
				}
				
				return dtRetorno;
			}
			
			/// <summary>
			/// Consulta o banco com os parâmetros passados e retorna o valor
			/// </summary>
			/// <param name="sqlSelect">SQL de select</param>
			/// <param name="parametros">Coleção de Parametros</param>
			/// <returns>O resultado, ou Nothing, caso nada tenha sido retornado.</returns>
			public static object ConsultarValor(string sqlSelect, List<Parameter> parametros)
			{
				if (ConfigurationManager.ConnectionStrings.Count == 0)
				{
					throw (new Exception("Cannot find a ConnectionString. Check your configuration file."));
				}
				return ConsultarValor(ConfigurationManager.ConnectionStrings[0], sqlSelect, parametros);
			}
			
			/// <summary>
			/// Consulta o banco com os parâmetros passados e retorna o valor
			/// </summary>
			/// <param name="ConnectionString">Conexão no web.config</param>
			/// <param name="sqlSelect">SQL de select</param>
			/// <param name="parametros">Coleção de Parametros</param>
			/// <returns>O resultado, ou Nothing, caso nada tenha sido retornado.</returns>
			public static object ConsultarValor(ConnectionStringSettings ConnectionString, string sqlSelect, List<Parameter> parametros)
			{
				object valor = null;
				DataTable tabela = ConsultarBanco(ConnectionString, sqlSelect, parametros);
				if (tabela != null&& tabela.Rows.Count > 0)
				{
					valor = tabela.Rows[0][0];
				}
				return valor;
			}
			
			/// <summary>
			/// Executa funções de atualização no banco de dados
			/// </summary>
			/// <param name="sql">sql de atualização (UPDATE, INSERT INTO, DELETE)</param>
			/// <param name="parametros">Coleção de Parametros</param>
			/// <returns></returns>
			/// <remarks></remarks>
			public static int AtualizarBanco(string sql, List<Parameter> parametros)
			{
				if (ConfigurationManager.ConnectionStrings.Count == 0)
				{
					throw (new Exception("Cannot find a ConnectionString. Check your configuration file."));
				}
				return AtualizarBanco(ConfigurationManager.ConnectionStrings[0], sql, parametros);
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
			public static int AtualizarBanco(ConnectionStringSettings ConnectionString, string sql, List<Parameter> parametros)
			{
				DbProviderFactory factory = GetFactory(ConnectionString);
				
				DbConnection conn = factory.CreateConnection();
				conn.ConnectionString = ConnectionString.ConnectionString;
				DbCommand cmd;
				
				int retorno = - 99;
				
				
				
				
				try
				{
					cmd = conn.CreateCommand();
					cmd.CommandText = sql;
					cmd.CommandTimeout = Helper.DefaultTimeout;
					if (parametros != null)
					{
                        foreach (Parameter param in parametros)
                        {
                            System.Data.Common.DbParameter parametro = factory.CreateParameter();
                            parametro.ParameterName = param.ParameterName;
                            parametro.Value = param.Value;

                            parametro.DbType = param.DbType;



                            cmd.Parameters.Add(parametro);
                        }
					}
					
					conn.Open();
					
					
					try
					{
						if (System.Diagnostics.Debugger.IsAttached)
						{
							
							System.Text.StringBuilder traceInfo = new System.Text.StringBuilder();
							traceInfo.AppendLine("Helper: AtualizarBanco()");
							traceInfo.AppendLine(" > " + ConnectionString.ConnectionString);
							if (parametros != null)
							{
								foreach (Parameter p in parametros)
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
					
				}
				catch (Exception)
				{
					throw;
				}
				finally
				{
					if (conn.State == System.Data.ConnectionState.Open)
					{
						conn.Close();
					}
				}
				
				return retorno;
			}
		}
	}
	
}
