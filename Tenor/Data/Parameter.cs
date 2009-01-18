using System.Diagnostics;
using System;
using System.Collections;
using Microsoft.VisualBasic;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Common;


namespace Tenor
{
	namespace Data
	{
		
		public class Parameter : System.Data.Common.DbParameter
		{
			
			
			internal System.Data.Common.DbParameter root;
			
			private string _name;
			private object _value;
			
			private DbProviderFactory _factory;
			
			/// <summary>
			/// Cria uma instancia de parametro com Nome e Valor
			/// </summary>
			/// <param name="Name">Nome do parametro. Ex. CPF ou @CPF.</param>
			/// <param name="Value">Valor do parametro</param>
			/// <remarks></remarks>
			public Parameter(string Name, object Value)
			{
				_name = Name;
				_value = Value;
				SetParameter(System.Data.Common.DbProviderFactories.GetFactory(BLL.BLLBase.GetDefaultConnection().ProviderName));
			}
			
			
			
			
			/// <summary>
			/// Cria uma instancia de parametro com Nome e Valor
			/// </summary>
			/// <param name="Name">Nome do parametro. Ex. @CPF</param>
			/// <param name="Value">Valor do parametro</param>
			/// <param name="ConnectionString"></param>
			/// <remarks></remarks>
			public Parameter(ConnectionStringSettings ConnectionString, string Name, object Value)
			{
				_name = Name;
				_value = Value;
				_factory = System.Data.Common.DbProviderFactories.GetFactory(ConnectionString.ProviderName);
				SetParameter(_factory);
			}
			
			/// <summary>
			/// Cria uma instancia de parametro com tipo da fonte de dados
			/// </summary>
			/// <param name="factory">Factory do tipo da fonte de dados</param>
			/// <param name="Name">Nome do parametro. Ex. @CPF</param>
			/// <param name="Value">Valor do parametro</param>
			/// <remarks></remarks>
			public Parameter(System.Data.Common.DbProviderFactory factory, string Name, object Value)
			{
				_name = Name;
				_value = Value;
				_factory = factory;
				SetParameter(_factory);
			}
			
			internal void SetParameter(System.Data.Common.DbProviderFactory provider)
			{
				root = provider.CreateParameter();
				ParameterName = _name;
				Value = _value;
			}
			
			internal string DbTypeName
			{
				get
				{
					SqlParameter sql = root as SqlParameter;
					string res;
					if (sql != null)
					{
						res = sql.SqlDbType.ToString().ToLower();
						if (res.Contains("varchar"))
						{
							//Compatível somente com SQL 2005 +
							res += "(MAX)";
						}
					}
					else
					{
						res = root.DbType.ToString().ToLower();
					}
					
					
					return res;
				}
			}
			
			public string ParameterPrefix
			{
				get
				{
					switch (root.GetType().Name)
					{
						case "SqlParameter":
						case "OleDbParameter":
						case "SQLiteParameter":
							return "@";
						case "OracleParameter":
							return ":";
						default:
							return "";
					}
				}
			}
			
			
			public override System.Data.DbType DbType
			{
				get
				{
					return root.DbType;
				}
				set
				{
					root.DbType = value;
				}
			}
			
			public override System.Data.ParameterDirection Direction
			{
				get
				{
					return root.Direction;
				}
				set
				{
					root.Direction = value;
				}
			}
			
			public override bool IsNullable
			{
				get
				{
					return root.IsNullable;
				}
				set
				{
					root.IsNullable = value;
				}
			}
			
			public override string ParameterName
			{
				get
				{
					return root.ParameterName;
				}
				set
				{
					root.ParameterName = value;
				}
			}
			
			public override void ResetDbType()
			{
				root.ResetDbType();
			}
			
			public override int Size
			{
				get
				{
					return root.Size;
				}
				set
				{
					root.Size = value;
				}
			}
			
			public override string SourceColumn
			{
				get
				{
					return root.SourceColumn;
				}
				set
				{
					root.SourceColumn = value;
				}
			}
			
			public override bool SourceColumnNullMapping
			{
				get
				{
					return root.SourceColumnNullMapping;
				}
				set
				{
					root.SourceColumnNullMapping = value;
				}
			}
			
			public override System.Data.DataRowVersion SourceVersion
			{
				get
				{
					return root.SourceVersion;
				}
				set
				{
					root.SourceVersion = value;
				}
			}
			
			public override object Value
			{
				get
				{
					return root.Value;
				}
				set
				{
					root.Value = value;
				}
			}
		}
		
		
	}
	
}
