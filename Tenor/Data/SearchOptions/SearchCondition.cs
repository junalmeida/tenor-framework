using System.Diagnostics;
using System;
using System.Collections;
using Microsoft.VisualBasic;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Configuration;
using Tenor.BLL;


namespace Tenor
{
	namespace Data
	{
		/// <summary>
		/// Indica uma condição de busca
		/// </summary>
		/// <remarks></remarks>
		public class SearchCondition
		{
			
			
			/// <summary>
			///
			/// </summary>
			/// <param name="Table"></param>
			/// <param name="Property"></param>
			/// <param name="Value"></param>
			/// <remarks></remarks>
			public SearchCondition(Type Table, string @Property, object Value) : this(Table, @Property, Value, Data.CompareOperator.Equal)
			{
			}
			
			/// <summary>
			///
			/// </summary>
			/// <param name="Table"></param>
			/// <param name="Property"></param>
			/// <param name="Value"></param>
			/// <param name="TableAlias"></param>
			/// <remarks></remarks>
			public SearchCondition(Type Table, string @Property, object Value, string TableAlias) : this(Table, @Property, Value, Data.CompareOperator.Equal, TableAlias)
			{
			}
			
			/// <summary>
			///
			/// </summary>
			/// <param name="Table"></param>
			/// <param name="Property"></param>
			/// <param name="Value"></param>
			/// <param name="CompareOperator"></param>
			/// <remarks></remarks>
			public SearchCondition(Type Table, string @Property, object Value, Data.CompareOperator CompareOperator) : this(Table, @Property, Value, CompareOperator, ((Type) null))
			{
			}
			
			/// <summary>
			///
			/// </summary>
			/// <param name="Table"></param>
			/// <param name="Property"></param>
			/// <param name="Value"></param>
			/// <param name="CompareOperator"></param>
			/// <param name="TableAlias"></param>
			/// <remarks></remarks>
			public SearchCondition(Type Table, string @Property, object Value, Data.CompareOperator CompareOperator, string TableAlias) : this(Table, @Property, Value, CompareOperator, null, TableAlias)
			{
			}
			
			/// <summary>
			///
			/// </summary>
			/// <param name="Table"></param>
			/// <param name="Property"></param>
			/// <param name="Value"></param>
			/// <param name="CompareOperator"></param>
			/// <param name="CastType"></param>
			/// <remarks></remarks>
			public SearchCondition(Type Table, string @Property, object Value, Data.CompareOperator CompareOperator, Type CastType) : this(Table, @Property, Value, CompareOperator, CastType, string.Empty)
			{
			}
			
			/// <summary>
			///
			/// </summary>
			/// <param name="Table"></param>
			/// <param name="Property"></param>
			/// <param name="Value"></param>
			/// <param name="CompareOperator"></param>
			/// <param name="CastType"></param>
			/// <param name="TableAlias"></param>
			/// <remarks></remarks>
			public SearchCondition(Type Table, string @Property, object Value, Data.CompareOperator CompareOperator, Type CastType, string TableAlias)
			{
				_Table = Table;
				_Property = @Property;
				
				
				_TableAlias = TableAlias;
				if (_TableAlias != null)
				{
					if (_TableAlias.Contains("\"") || _TableAlias.Contains("\'") || _TableAlias.Contains("\r") || _TableAlias.Contains("\n"))
					{
						throw (new ArgumentException("Invalid characters on TableAlias", "TableAlias"));
					}
					else if (_TableAlias.Contains(" "))
					{
						_TableAlias = "\"" + TableAlias + "\"";
					}
				}
				
				
				if (CastType == null)
				{
					_Value = Value;
				}
				else
				{
					_Value = Convert.ChangeType(Value, CastType);
				}
				
				//_Value = CType(Value, casttype)
				_CompareOperator = CompareOperator;
				_CastType = CastType;
			}
			
			internal Type _Table;
			public Type Table
			{
				get
				{
					return _Table;
				}
			}
			
			private string _TableAlias;
			public string TableAlias
			{
				get
				{
					if (string.IsNullOrEmpty(_TableAlias))
					{
						return Table.Name;
					}
					else
					{
						return _TableAlias;
					}
				}
			}
			
			private string _Property;
			public string @Property
			{
				get
				{
					return _Property;
				}
			}
			
			private FieldInfo _FieldInfo;
			internal FieldInfo FieldInfo
			{
				get
				{
					if (_FieldInfo == null)
					{
						try
						{
							_FieldInfo = new FieldInfo(Table.GetProperty(@Property));
						}
						catch (Exception)
						{
						}
					}
					return _FieldInfo;
				}
			}
			
			private SpecialFieldInfo _SpecialFieldInfo;
			internal SpecialFieldInfo SpecialFieldInfo
			{
				get
				{
					if (_SpecialFieldInfo == null)
					{
						try
						{
							_SpecialFieldInfo = new SpecialFieldInfo(Table.GetProperty(@Property));
						}
						catch (Exception)
						{
						}
					}
					return _SpecialFieldInfo;
				}
			}
			
			private object _Value;
			public object Value
			{
				get
				{
					return _Value;
				}
			}
			
			private Data.CompareOperator _CompareOperator = Data.CompareOperator.Equal;
			public Data.CompareOperator CompareOperator
			{
				get
				{
					return _CompareOperator;
				}
			}
			
			private Type _CastType;
			public Type CastType
			{
				get
				{
					return _CastType;
				}
			}
			
            public override string ToString()
			{
				return ToString(null);
			}
			
			public string ToString(ConnectionStringSettings Connection)
			{
				return ToString(Connection, true);
			}
			
			
			
			public string ToString(ConnectionStringSettings Connection, bool WithClassName)
			{
                TableInfo table = TableInfo.CreateTableInfo(this.Table);

                if (Connection == null)
                    Connection = table.GetConnection();
				
				System.Text.StringBuilder str = new System.Text.StringBuilder();
				
				//Lado esquerdo da condição
				
				if (FieldInfo != null)
				{
					//BLL.BLLBase currentInstance = (BLL.BLLBase) (Activator.CreateInstance(Table));
					//currentInstance.Connection = Connection;
					
					if (CastType != null)
					{
						str.Append("CAST(");
					}
					
					if (string.IsNullOrEmpty(_TableAlias))
					{
						if (WithClassName)
						{
							str.Append(this.Table.Name);
						}
						else
						{
							str.Append(table.GetSchemaAndTable());
						}
					}
					else
					{
						str.Append(_TableAlias);
					}
					
					str.Append(".");
					str.Append(Helper.GetCommandBuilder(Connection).QuoteIdentifier(FieldInfo.DataFieldName));
					
					if (CastType != null)
					{
                        str.Append(" as " + Helper.GetDbTypeName(CastType, Helper.GetFactory(Connection)) + ")");
					}
					
				}
				else if (SpecialFieldInfo != null)
				{
					str.Append("(" + string.Format(SpecialFieldInfo.Expression, this.TableAlias) + ")");
				}
				else
				{
					throw (new ArgumentException("Invalid Property \'" + this.Property + "\'. You must define a Field or a SpecialField property item.", "Property", null));
				}
				
				//Fim do lado esquerdo da condição
				
				//Lado direito da condição
				if ((Value == null) || Value == DBNull.Value)
				{
					if (CompareOperator == CompareOperator.Equal)
					{
						str.Append(" IS NULL ");
					}
					else if (CompareOperator == CompareOperator.NotEqual)
					{
						str.Append(" IS NOT NULL ");
					}
					else
					{
						throw (new InvalidOperationException("Cannot use this operator with null expressions."));
					}
				}
				else
				{
					//TenorParameter p = new Parameter(ParameterName, Value);
                    string parameterPrefix = Helper.GetParameterPrefix(Connection);
					switch (CompareOperator)
					{
						case Data.CompareOperator.Equal:
							str.Append(" = " + parameterPrefix + ParameterName);
							break;
						case Data.CompareOperator.NotEqual:
                            str.Append(" <>  " + parameterPrefix + ParameterName);
							break;
						case Data.CompareOperator.LessThan:
                            str.Append(" <  " + parameterPrefix + ParameterName);
							break;
						case Data.CompareOperator.LessThanOrEqual:
                            str.Append(" <=  " + parameterPrefix + ParameterName);
							break;
						case Data.CompareOperator.GreaterThan:
                            str.Append(" >  " + parameterPrefix + ParameterName);
							break;
						case Data.CompareOperator.GreaterThanOrEqual:
                            str.Append(" >=  " + parameterPrefix + ParameterName);
							break;
						case Data.CompareOperator.Like:
                            str.Append(" LIKE  " + parameterPrefix + ParameterName);
							break;
						case Data.CompareOperator.NotLike:
                            str.Append(" NOT LIKE  " + parameterPrefix + ParameterName);
							break;
						case Data.CompareOperator.ContainsInFlags:
							if (Connection.ProviderName != "System.Data.SqlClient")
							{
								throw (new ConfigurationErrorsException("ContainsInFlags can only be used with SqlClient"));
							}
                            str = new System.Text.StringBuilder("(IsNull(" + str.ToString() + ", 0) & " + parameterPrefix + ParameterName + ") = " + parameterPrefix + ParameterName);
							break;
						default:
							throw (new ArgumentOutOfRangeException("CompareOperator", "Specified argument was out of the range of valid values."));
					}
				}
				
				return str.ToString();
			}
			
			private string _ParameterName;
			public string ParameterName
			{
				get
				{
					if (string.IsNullOrEmpty(_ParameterName))
					{
						if (FieldInfo != null)
						{
							_ParameterName = FieldInfo.DataFieldName.ToLower().Trim().Replace(" ", "_");
						}
						else if (SpecialFieldInfo != null)
						{
							_ParameterName = SpecialFieldInfo.Alias.ToLower().Trim().Replace(" ", "_");
						}
						
						if (_ParameterName.Length > 30)
						{
							throw (new InvalidOperationException("Cannot generate parameter name. Identifier \'" + _ParameterName + "\' is too long."));
						}
						
						_ParameterName += Guid.NewGuid().ToString().Replace("-", "");
						if (_ParameterName.Length > 30)
						{
							_ParameterName = _ParameterName.Substring(0, 30);
						}
						
					}
					return _ParameterName;
				}
				set
				{
					_ParameterName = value;
				}
			}
		}
		
		
	}
	
}
