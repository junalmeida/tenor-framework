using System;
using System.Configuration;
using System.Data;
using ConnectionState = System.Data.ConnectionState;
using SchemaType = System.Data.SchemaType;
/*
 * using DataRow = System.Data.DataRow;
 */


namespace Tenor.Data
{
    /// <summary>
    /// This is an extended version of System.Data.DataTable. This datatable can consume data on the database automatically.
    /// </summary>
    /// <remarks></remarks>
    public class DataTable : System.Data.DataTable
    {


        private System.Data.Common.DbProviderFactory factory;


        /// <summary>
        /// Creates a new instance of a DataTable.
        /// </summary>
        /// <remarks></remarks>
        public DataTable()
        {

        }
        /// <summary>
        /// Creates a new instance of a DataTable.
        /// </summary>
        /// <param name="commandText">A SQL query string that will be executed.</param>
        /// <param name="parameters">An array of TenorParameter with query parameters.</param>
        public DataTable(string commandText, TenorParameter[] parameters)
            : this(commandText, parameters, null)
        {
        }

        /// <summary>
        /// Creates a new instance of a DataTable.
        /// </summary>
        /// <param name="commandText">A SQL query string that will be executed.</param>
        /// <param name="parameters">An array of TenorParameter with query parameters.</param>
        /// <param name="connection">The connection.</param>
        /// <remarks></remarks>
        public DataTable(string commandText, TenorParameter[] parameters, ConnectionStringSettings connection)
            : this()
        {
            if (connection == null)
                connection = EntityBase.SystemConnection;

            AttachConnection(connection);
            AttachParameters(parameters);
            this.CommandText = commandText;
            this.currentConnection = connection;
        }

        ConnectionStringSettings currentConnection;

        /// <summary>
        /// Creates a new instance of a DataTable.
        /// </summary>
        public DataTable(System.Data.Common.DbProviderFactory provider)
            : this()
        {
            factory = provider;
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

        private void AttachConnection(ConnectionStringSettings connection)
        {
            Tenor.Data.Dialects.GeneralDialect dialect = Dialects.DialectFactory.CreateDialect(connection);

            factory = dialect.Factory;
            _ActiveConnection = Helper.CreateConnection(connection);

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

        /*
        Private Sub AttachConnection(ByVal Connection As System.Data.Common.DbConnection)
            _ActiveConnection = Connection
            If _ActiveConnection.State <> ConnectionState.Open Then
                _ActiveConnection.Open()
            End If

            If _Ad Is Nothing Then
                If Connection.GetType().FullName.StartsWith("System.Data.SqlClient") Then

                    Dim Con As SqlClient.SqlConnection = CType(Connection, SqlConnection)
                    Dim Cmd As SqlClient.SqlCommand = Con.CreateCommand()
                    Dim Ad As New SqlClient.SqlDataAdapter(Cmd)
                    Dim cmds As New SqlClient.SqlCommandBuilder(Ad)

                    _Cmd = Cmd
                    _Ad = Ad
                    _Cmds = cmds
                ElseIf Connection.GetType().FullName.StartsWith("System.Data.Odbc") Then

                    Dim Con As Odbc.OdbcConnection = CType(Connection, Odbc.OdbcConnection)
                    Dim Cmd As Odbc.OdbcCommand = Con.CreateCommand()
                    Dim Ad As New Odbc.OdbcDataAdapter(Cmd)
                    Dim cmds As New Odbc.OdbcCommandBuilder(Ad)

                    _Cmd = Cmd
                    _Ad = Ad
                    _Cmds = cmds
                Else
                    Throw New Exception("This provider is not supported")
                End If
            End If

        End Sub
         */

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
        /// Gets or sets a query that will be executed.
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
        /// Gets or sets the wait time in seconds before terminating the attempt to execute a command and generating an error.
        /// </summary>
        /// <returns>The time in seconds to wait for the command to execute.
        /// </returns>
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
        /// Gets the Connection used on this DataTable.
        /// </summary>
        public System.Data.Common.DbConnection ActiveConnection
        {
            get
            {
                return _ActiveConnection;
            }
        }

        /// <summary>
        /// Gets a collection of DbParameter.
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

        /*
        Public Sub Bind()
            If _Cmd Is Nothing Or _Ad Is Nothing Then
                Throw New Exception("The database connection is not set")
            End If
            _Ad.Fill(Me)
        End Sub
         */

        /// <summary>
        /// Executes the query and fills this datatable.
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
                //Diagnostics.Debug.DebugSQL("Tenor.Data.DataTable", this.CommandText, null, currentConnection);
                _Ad.Fill(this);
            }
            catch (Exception up)
            {
                exception = true;
                up.Data.Add("CommandText", CommandText);
                throw;
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