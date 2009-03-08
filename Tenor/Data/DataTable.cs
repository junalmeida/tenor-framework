using System.Diagnostics;
using System;
using System.Collections;
using Microsoft.VisualBasic;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Configuration;
using ConnectionState = System.Data.ConnectionState;
using SchemaType = System.Data.SchemaType;
using System.Data.Common;
//using DataRow = System.Data.DataRow;


namespace Tenor
{
	namespace Data
	{
		/// <summary>
		/// Autor: Marcos A. P. de Almeida Jr.
		/// </summary>
		/// <remarks></remarks>
		public class DataTable : System.Data.DataTable
		{
			
			
			private System.Data.Common.DbProviderFactory factory;
			
			
			/// <summary>
			/// Cria uma nova instancia do DataTable.
			/// </summary>
			/// <remarks></remarks>
			public DataTable()
			{
				
			}
			/// <summary>
			/// Cria uma nova instancia do DataTable.
			/// </summary>
			/// <param name="CommandText">A string SQL de consulta para o DataTable.</param>
			/// <param name="Parameters">Array de parametros para a consulta.</param>
			/// <remarks></remarks>
			public DataTable(string CommandText, TenorParameter[] Parameters) : this()
			{
				
				if (ConfigurationManager.ConnectionStrings.Count == 0)
				{
					throw (new Exception("Cannot find a ConnectionString. Check your configuration file."));
				}
				AttachConnection(ConfigurationManager.ConnectionStrings[0]);
				AttachParameters(Parameters);
				this.CommandText = CommandText;
				
				
			}
			
			/// <summary>
			/// Cria uma nova instancia do DataTable.
			/// </summary>
			/// <param name="CommandText">A string SQL de consulta para o DataTable.</param>
			/// <param name="Parameters">Array de parametros para a consulta.</param>
			/// <param name="Connection">ConnectionString da web.config</param>
			/// <remarks></remarks>
			public DataTable(string CommandText, TenorParameter[] Parameters, ConnectionStringSettings Connection) : this()
			{
				AttachConnection(Connection);
				AttachParameters(Parameters);
				this.CommandText = CommandText;
			}
			
			/// <summary>
			/// Cria uma nova instancia do DataTable.
			/// </summary>
			/// <param name="Provider">Provedor de tipos de banco</param>
			/// <remarks></remarks>
			public DataTable(System.Data.Common.DbProviderFactory Provider) : this()
			{
				factory = Provider;
				this._ActiveConnection = factory.CreateConnection();
				this._Ad = factory.CreateDataAdapter();
				this._Cmd = _Ad.SelectCommand;
				this._Cmds = factory.CreateCommandBuilder();
				_Cmds.DataAdapter = _Ad;
				
				if (_ActiveConnection.State == ConnectionState.Closed)
				{
					_ActiveConnection.Open();
				}
				
				
			}
			
			private void AttachConnection(ConnectionStringSettings Connection)
			{
				factory = Helper.GetFactory(Connection);
				_ActiveConnection = factory.CreateConnection();
				_ActiveConnection.ConnectionString = Connection.ConnectionString;
				_Cmd = _ActiveConnection.CreateCommand();
				_Cmd.CommandTimeout = Helper.DefaultTimeout;
				_Ad = factory.CreateDataAdapter();
				_Ad.SelectCommand = _Cmd;
				_Cmds = factory.CreateCommandBuilder();
				_Cmds.DataAdapter = _Ad;
				
				if (_ActiveConnection.State == ConnectionState.Closed)
				{
					_ActiveConnection.Open();
				}
				
				
			}
			
			//Private Sub AttachConnection(ByVal Connection As System.Data.Common.DbConnection)
			//    _ActiveConnection = Connection
			//    If _ActiveConnection.State <> ConnectionState.Open Then
			//        _ActiveConnection.Open()
			//    End If
			
			//    If _Ad Is Nothing Then
			//        If Connection.GetType().FullName.StartsWith("System.Data.SqlClient") Then
			
			//            Dim Con As SqlClient.SqlConnection = CType(Connection, SqlConnection)
			//            Dim Cmd As SqlClient.SqlCommand = Con.CreateCommand()
			//            Dim Ad As New SqlClient.SqlDataAdapter(Cmd)
			//            Dim cmds As New SqlClient.SqlCommandBuilder(Ad)
			
			//            _Cmd = Cmd
			//            _Ad = Ad
			//            _Cmds = cmds
			//        ElseIf Connection.GetType().FullName.StartsWith("System.Data.Odbc") Then
			
			//            Dim Con As Odbc.OdbcConnection = CType(Connection, Odbc.OdbcConnection)
			//            Dim Cmd As Odbc.OdbcCommand = Con.CreateCommand()
			//            Dim Ad As New Odbc.OdbcDataAdapter(Cmd)
			//            Dim cmds As New Odbc.OdbcCommandBuilder(Ad)
			
			//            _Cmd = Cmd
			//            _Ad = Ad
			//            _Cmds = cmds
			//        Else
			//            Throw New Exception("This provider is not supported")
			//        End If
			//    End If
			
			//End Sub
			
			private void AttachParameters(TenorParameter[] parameters)
			{
				if (Parameters != null)
				{
					foreach (TenorParameter i in parameters)
					{
						_Cmd.Parameters.Add(i.ToDbParameter(factory));
					}
				}
			}
			
			/// <summary>
			/// Retorna ou define a CommandText que irá executar na fonte de dados.
			/// </summary>
			public string CommandText
			{
				get
				{
					return _Cmd.CommandText;
				}
				set
				{
					_Cmd.CommandText = value;
					if (value.Trim().ToUpper().StartsWith("SELECT "))
					{
						try
						{
							_Ad.FillSchema(this, SchemaType.Mapped);
						}
						catch (Exception)
						{
						}
					}
				}
			}
			
			
			/// <summary>
			/// Retorna ou define a CommandTimeout para executar na fonte de dados.
			/// </summary>
			public int CommandTimeout
			{
				get
				{
					return _Cmd.CommandTimeout;
				}
				set
				{
					_Cmd.CommandTimeout = value;
				}
			}
			
			private System.Data.Common.DbConnection _ActiveConnection;
			/// <summary>
			/// Retorna a Connection usada neste DataTable.
			/// </summary>
			public System.Data.Common.DbConnection ActiveConnection
			{
				get
				{
					return _ActiveConnection;
				}
			}
			
			/// <summary>
			/// Retorna uma coleção de DbParameters.
			/// </summary>
			public System.Data.Common.DbParameterCollection Parameters
			{
				get
				{
					return _Cmd.Parameters;
				}
			}
			
			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
			}
			
			~DataTable()
			{
				if (_Cmd != null)
				{
					_Cmd.Dispose();
					_Cmd = null;
				}
				if (_ActiveConnection != null)
				{
					if (_ActiveConnection.State != ConnectionState.Closed)
					{
						_ActiveConnection.Close();
					}
					_ActiveConnection.Dispose();
					_ActiveConnection = null;
				}
			}
			
			//'--- Database Logic
			private System.Data.Common.DbCommand _Cmd;
			private System.Data.Common.DbDataAdapter _Ad;
			private System.Data.Common.DbCommandBuilder _Cmds;
			
			
			//Public Sub Bind()
			//    If _Cmd Is Nothing Or _Ad Is Nothing Then
			//        Throw New Exception("The database connection is not set")
			//    End If
			//    _Ad.Fill(Me)
			//End Sub
			
			/// <summary>
            /// Runs the query and fills this datatable.
			/// </summary>
			public void Bind()
			{
				if (_Cmd == null || _Ad == null)
				{
					throw (new TenorException("The database connection is not set"));
				}
				
				bool exception = false;
				DateTime sqlTime = DateTime.Now;
				try
				{
					if (_ActiveConnection.State == ConnectionState.Closed)
					{
						_ActiveConnection.Open();
					}
					sqlTime = DateTime.Now;
					_Ad.Fill(this);
				}
				catch (Exception up)
				{
					exception = true;
					up.Data.Add("CommandText", CommandText);
					throw (up);
				}
				finally
				{
					try
					{
						if (exception)
						{
							System.Diagnostics.Trace.TraceInformation("DataTable.Bind Time: " + (DateTime.Now - sqlTime).TotalSeconds.ToString() + " sec. (SqlException)");
						}
						else
						{
							System.Diagnostics.Trace.TraceInformation("DataTable.Bind Time: " + (DateTime.Now - sqlTime).TotalSeconds.ToString() + " sec.");
						}
					}
					catch
					{
						
					}
					if (_ActiveConnection.State != ConnectionState.Closed)
					{
						_ActiveConnection.Close();
					}
				}
				
			}
			
			/// <summary>
            /// Persists on the database the datatable changes.
			/// </summary>
			public void Update()
			{
				bool exception = false;
				DateTime sqlTime = DateTime.Now;
				try
				{
					if (_ActiveConnection.State == ConnectionState.Closed)
					{
						_ActiveConnection.Open();
					}
					
					if (_Ad.DeleteCommand != null)
					{
						_Ad.DeleteCommand.CommandTimeout = _Cmd.CommandTimeout;
					}
					if (_Ad.InsertCommand != null)
					{
						_Ad.InsertCommand.CommandTimeout = _Cmd.CommandTimeout;
					}
					if (_Ad.UpdateCommand != null)
					{
						_Ad.UpdateCommand.CommandTimeout = _Cmd.CommandTimeout;
					}
					
					sqlTime = DateTime.Now;
					_Ad.Update(this);
				}
				catch (Exception up)
				{
					throw (up);
				}
				finally
				{
					try
					{
						if (exception)
						{
							System.Diagnostics.Trace.TraceInformation("DataTable.Update Time: " + (DateTime.Now - sqlTime).TotalSeconds.ToString() + " sec. (SqlException)");
						}
						else
						{
							System.Diagnostics.Trace.TraceInformation("DataTable.Update Time: " + (DateTime.Now - sqlTime).TotalSeconds.ToString() + " sec.");
						}
					}
					catch
					{
						
					}
					
					if (_ActiveConnection.State != ConnectionState.Closed)
					{
						_ActiveConnection.Close();
					}
				}
			}
			
			public DataRow this[int Index]
			{
				get
				{
					return Rows[Index];
				}
			}
		}
	}
}
