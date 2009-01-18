using System.Diagnostics;
using System;
using System.Collections;
using Microsoft.VisualBasic;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Configuration;


namespace Tenor
{
	namespace Data
	{
		public class SortingCriteria
		{
			
			
			/// <summary>
			///
			/// </summary>
			/// <param name="Table"></param>
			/// <param name="Property"></param>
			/// <remarks></remarks>
			public SortingCriteria(Type Table, string @Property, Type CastType) : this(Table, @Property, Data.SortOrder.Ascending, CastType)
			{
			}
			
			/// <summary>
			///
			/// </summary>
			/// <param name="Table"></param>
			/// <param name="Property"></param>
			/// <remarks></remarks>
			public SortingCriteria(Type Table, string @Property) : this(Table, @Property, Data.SortOrder.Ascending)
			{
			}
			
			/// <summary>
			///
			/// </summary>
			/// <param name="Table"></param>
			/// <param name="Property"></param>
			/// <param name="SortOrder"></param>
			/// <remarks></remarks>
			public SortingCriteria(Type Table, string @Property, Data.SortOrder SortOrder) : this(Table, @Property, SortOrder, null)
			{
			}
			
			/// <summary>
			///
			/// </summary>
			/// <param name="Table"></param>
			/// <param name="Property"></param>
			/// <param name="SortOrder"></param>
			/// <param name="CastType"></param>
			/// <remarks></remarks>
			public SortingCriteria(Type Table, string @Property, Data.SortOrder SortOrder, Type CastType)
			{
				_Table = Table;
				_Property = @Property;
				_CastType = CastType;
				_SortOrder = SortOrder;
			}
			
			
			internal Type _Table;
			public Type Table
			{
				get
				{
					return _Table;
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
			
			private Type _CastType;
			public Type CastType
			{
				get
				{
					return _CastType;
				}
			}
			
			private Data.SortOrder _SortOrder;
			public Data.SortOrder SortOrder
			{
				get
				{
					return _SortOrder;
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
			
			[Obsolete("Passe a Connection", true)]public override string ToString()
			{
				return ToString(null, true);
			}
			
			public string ToString(ConnectionStringSettings Connection)
			{
				return ToString(Connection, true);
			}
			
			public string ToString(ConnectionStringSettings Connection, bool WithClassName)
			{
				if (Table == null || ! Table.IsSubclassOf(typeof(BLL.BLLBase)))
				{
					throw (new ArgumentException("Invalid Table type. You must specify a derived type of BLL.BLLBase", "Table", null));
				}
				
				System.Text.StringBuilder builder = new System.Text.StringBuilder();
				
				if (FieldInfo != null)
				{
					BLL.BLLBase currentInstance = (BLL.BLLBase) (Activator.CreateInstance(Table));
					currentInstance.SetActiveConnection(Connection);
					
					if (CastType != null)
					{
						builder.Append("CAST(");
					}
					
					//Implementar o Alias do sort.
					if (WithClassName)
					{
						builder.Append(currentInstance.GetType().Name);
					}
					else
					{
						builder.Append(BLL.BLLBase.GetSchemaAndTable(currentInstance));
					}
					builder.Append(".");
					builder.Append(currentInstance.GetCommandBuilder().QuoteIdentifier(FieldInfo.DataFieldName));
					
					if (CastType != null)
					{
						builder.Append(" as " + Helper.GetDbTypeName(CastType, currentInstance) + ")");
					}
				}
				else if (SpecialFieldInfo != null)
				{
					builder.Append(SpecialFieldInfo.Alias);
				}
				else
				{
					throw (new ArgumentException("Invalid Property. You must define a Field or a SpecialField property item", "Property", null));
				}
				
				switch (SortOrder)
				{
					case SortOrder.Descending:
						builder.Append(" DESC");
						break;
					case SortOrder.Ascending:
						builder.Append(" ASC");
						break;
				}
				
				return builder.ToString();
			}
		}
		
	}
	
}
