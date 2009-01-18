using System.Diagnostics;
using System;
using System.Collections;
using Microsoft.VisualBasic;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;

namespace Tenor
{
	namespace Data
	{
		
		#region "Property Infos"
		
		internal class PropInfo
		{
			
			
			private System.Reflection.PropertyInfo _RelatedProperty;
			public virtual System.Reflection.PropertyInfo RelatedProperty
			{
				get
				{
					return _RelatedProperty;
				}
				set
				{
					_RelatedProperty = value;
				}
			}
			
			
			public object PropertyValue(object Instance, bool ConvertNullToDBNull)
			{
				try
				{
					
					if (RelatedProperty.ReflectedType != Instance.GetType())
					{
						throw (new InvalidCastException());
					}
					
					object res = RelatedProperty.GetValue(Instance, null);
					if (res == null)
					{
						return (ConvertNullToDBNull ? DBNull.Value : null);
					}
					else
					{
						if (res.GetType() == typeof(Nullable<>))
						{
							if (! System.Convert.ToBoolean(res.GetType().GetProperty("HasValue").GetValue(res, null)))
							{
								return (ConvertNullToDBNull ? DBNull.Value : null);
							}
							else
							{
								return res.GetType().GetProperty("Value").GetValue(res, null);
							}
						}
						else
						{
							return res;
						}
					}
				}
				catch (Exception ex)
				{
					throw (new InvalidOperationException("Cannot get \'" + RelatedProperty.Name + "\' value. See inner exception for details.", ex));
				}
				
			}
			public void SetPropertyValue(object Instance, bool ConvertNullToDBNull, object value)
			{
				try
				{
					if (value == DBNull.Value || value == null)
					{
						RelatedProperty.SetValue(Instance, null, null);
					}
					else
					{
						RelatedProperty.SetValue(Instance, value, null);
					}
					
				}
				catch (Exception ex)
				{
					throw (new InvalidOperationException("Cannot set \'" + this.RelatedProperty.Name + "\' value. See inner exception for details.", ex));
				}
				
			}
			
			public object PropertyValue(object Instance)
			{
				return PropertyValue(Instance, true);
			}
			public void SetPropertyValue(object Instance, object value)
			{
				SetPropertyValue(Instance, true, value);
			}
			
		}
		
		internal class ForeignKeyInfo : PropInfo
		{
			
			
			public ForeignKeyInfo(System.Reflection.PropertyInfo @Property)
			{
				RelatedProperty = @Property;
				
				_RelatedAttributes = (ForeignKeyAttribute[]) (RelatedProperty.GetCustomAttributes(typeof(ForeignKeyAttribute), true));
				if (_RelatedAttributes.Length == 0)
				{
					throw (new MissingFieldException("Could not find the foreign key information for \'" + @Property.Name + "\'"));
					
				}
				//acertar aki
			}
			
			
			private ForeignKeyAttribute[] _RelatedAttributes;
			public ForeignKeyAttribute[] RelatedAttributes
			{
				get
				{
					return _RelatedAttributes;
				}
				set
				{
					_RelatedAttributes = value;
				}
			}
			
			
			public bool IsArray
			{
				get
				{
					return RelatedProperty.PropertyType.IsArray || RelatedProperty.PropertyType.GetInterface("System.Collections.IList") != null || (RelatedProperty.PropertyType.IsGenericType && RelatedProperty.PropertyType.GetGenericTypeDefinition() == typeof(BLL.BLLCollection<>));
				}
			}
			
			public Type ElementType
			{
				get
				{
					
					if (RelatedProperty.PropertyType.IsArray)
					{
						return RelatedProperty.PropertyType.GetElementType();
					}
					else if ( RelatedProperty.PropertyType.GetInterface("System.Collections.IList") != null || (RelatedProperty.PropertyType.IsGenericType && RelatedProperty.PropertyType.GetGenericTypeDefinition() == typeof(BLL.BLLCollection<>)))
					{
						
						
						Type ftype = typeof(object);
						foreach (System.Reflection.MemberInfo i in RelatedProperty.PropertyType.GetDefaultMembers())
						{
							if (i.MemberType == System.Reflection.MemberTypes.Property)
							{
								ftype = ((System.Reflection.PropertyInfo) i).PropertyType;
								break;
							}
						}
						if (ftype == typeof(object))
						{
							throw (new InvalidOperationException("Could not find the collection element type"));
						}
						else
						{
							return ftype;
						}
					}
					else
					{
						return RelatedProperty.PropertyType;
					}
				}
			}
			
			
			//Public ReadOnly Property ForeignPropertyName() As String
			//    Get
			//        'Return RelatedAttribute.ForeignPropertyName
			//    End Get
			//End Property
			
			
			//Public ReadOnly Property LocalPropertyName() As String
			//    Get
			//        'Return RelatedAttribute.LocalPropertyName
			//    End Get
			//End Property
			
			public FieldInfo[] ForeignFields
			{
				get
				{
					List<FieldInfo> res = new List<FieldInfo>();
					foreach (ForeignKeyAttribute i in this.RelatedAttributes)
					{
						System.Reflection.PropertyInfo prop = this.ElementType.GetProperty(i.ForeignPropertyName);
						if (prop != null)
						{
							FieldAttribute[] atts = (FieldAttribute[]) (prop.GetCustomAttributes(typeof(FieldAttribute), true));
							if (atts.Length > 0)
							{
								FieldInfo field = new FieldInfo(prop);
								res.Add(field);
							}
							
						}
					}
					return res.ToArray();
				}
			}
			
			public FieldInfo[] LocalFields
			{
				get
				{
					List<FieldInfo> res = new List<FieldInfo>();
					foreach (ForeignKeyAttribute i in this.RelatedAttributes)
					{
						System.Reflection.PropertyInfo prop = this.RelatedProperty.DeclaringType.GetProperty(i.LocalPropertyName);
						if (prop != null)
						{
							FieldAttribute[] atts = (FieldAttribute[]) (prop.GetCustomAttributes(typeof(FieldAttribute), true));
							if (atts.Length > 0)
							{
								FieldInfo field = new FieldInfo(prop);
								res.Add(field);
							}
							
						}
					}
					return res.ToArray();
				}
			}
			
			public override string ToString()
			{
				return this.GetType().Name + ": " + RelatedProperty.DeclaringType.Name + "." + RelatedProperty.Name;
			}
		}
		
		internal class FieldInfo : PropInfo
		{
			
			
			public FieldInfo(System.Reflection.PropertyInfo @Property)
			{
				
				
				RelatedProperty = @Property;
				FieldAttribute[] atts = (FieldAttribute[]) (RelatedProperty.GetCustomAttributes(typeof(FieldAttribute), true));
				if (atts.Length == 0)
				{
					throw (new Exception("Missing field attribute for \'" + @Property.Name + "\'"));
				}
				_Attribute = atts[0];
				
				FieldType = RelatedProperty.PropertyType;
				if (string.IsNullOrEmpty(_Attribute.FieldName))
				{
					DataFieldName = RelatedProperty.Name;
				}
				else
				{
					DataFieldName = _Attribute.FieldName;
				}
				AutoNumber = _Attribute.AutoNumber;
				PrimaryKey = _Attribute.PrimaryKey;
				LazyLoading = _Attribute.LazyLoading;
			}
			private FieldAttribute _Attribute;
			
			
			
			private Type _FieldType;
			public Type FieldType
			{
				get
				{
					return _FieldType;
				}
				set
				{
					_FieldType = value;
				}
			}
			
			private bool _PrimaryKey;
			public bool PrimaryKey
			{
				get
				{
					return _PrimaryKey;
				}
				set
				{
					_PrimaryKey = value;
				}
			}
			
			private bool _AutoNumber;
			public bool AutoNumber
			{
				get
				{
					return _AutoNumber;
				}
				set
				{
					_AutoNumber = value;
				}
			}
			
			private bool _LazyLoading;
			public bool LazyLoading
			{
				get
				{
					return _LazyLoading;
				}
				set
				{
					_LazyLoading = value;
				}
			}
			
			private string _DataFieldName;
			public string DataFieldName
			{
				get
				{
					return _DataFieldName;
				}
				set
				{
					_DataFieldName = value;
				}
			}
			
			
			
			public string ParamName
			{
				get
				{
					return DataFieldName.ToLower().Replace(" ", "_");
				}
			}
			
		}
		
		internal class SpecialFieldInfo : PropInfo
		{
			
			
			public SpecialFieldInfo(System.Reflection.PropertyInfo @Property)
			{
				
				
				RelatedProperty = @Property;
				SpecialFieldAttribute[] atts = (SpecialFieldAttribute[]) (RelatedProperty.GetCustomAttributes(typeof(SpecialFieldAttribute), true));
				if (atts.Length == 0)
				{
					throw (new Exception("Missing SpecialFieldAttribute for \'" + @Property.Name + "\'"));
				}
				_Attribute = atts[0];
				
			}
			
			private SpecialFieldAttribute _Attribute;
			
			public string Expression
			{
				get
				{
					return _Attribute.Expression;
				}
			}
			
			
			public string @Alias
			{
				get
				{
					return "spfield" + RelatedProperty.Name.ToLower();
				}
			}
		}
		#endregion
		
		#region "Attributes"
		
		/// <summary>
		/// Identifica um campo do modelo relacional
		/// </summary>
		/// <remarks></remarks>
		[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]public class FieldAttribute : Attribute
		{
			
			
			public FieldAttribute()
			{
			}
			
			public FieldAttribute(bool PrimaryKey, bool AutoNumber)
			{
				_PrimaryKey = PrimaryKey;
				_AutoNumber = AutoNumber;
			}
			
			public FieldAttribute(string FieldName, bool PrimaryKey, bool AutoNumber)
			{
				_FieldName = FieldName;
				_PrimaryKey = PrimaryKey;
				_AutoNumber = AutoNumber;
			}
			
			private string _FieldName;
			/// <summary>
			/// Define o nome do campo no modelo relacional
			/// </summary>
			/// <value></value>
			/// <returns></returns>
			/// <remarks></remarks>
			public string FieldName
			{
				get
				{
					return _FieldName;
				}
				set
				{
					_FieldName = value;
				}
			}
			
			
			private bool _PrimaryKey;
			
			/// <summary>
			/// Indica se o campo representado por esta instância faz parte de uma chave primária
			/// </summary>
			/// <value></value>
			/// <returns></returns>
			/// <remarks></remarks>
			public bool PrimaryKey
			{
				get
				{
					return _PrimaryKey;
				}
				set
				{
					_PrimaryKey = value;
				}
			}
			
			
			private bool _AutoNumber;
			/// <summary>
			/// Indica se o campo representado por esta instância tem auto numeração
			/// </summary>
			/// <value></value>
			/// <returns></returns>
			/// <remarks></remarks>
			public bool AutoNumber
			{
				get
				{
					return _AutoNumber;
				}
				set
				{
					_AutoNumber = value;
				}
			}
			
			
			
			//Private _DbType As System.Data.DbType
			//Public Property DbType() As System.Data.DbType
			//    Get
			//        Return _DbType
			//    End Get
			//    Set(ByVal value As System.Data.DbType)
			//        _DbType = value
			//    End Set
			//End Property
			
			private bool _LazyLoading = false;
			public bool LazyLoading
			{
				get
				{
					return _LazyLoading;
				}
				set
				{
					_LazyLoading = value;
				}
			}
			
		}
		
		/// <summary>
		/// Identifica uma relação extrangeira
		/// </summary>
		/// <remarks></remarks>
		[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]public class ForeignKeyAttribute : Attribute
		{
			
			
			private ForeignKeyAttribute()
			{
			}
			
			/// <summary>
			/// Instancia um atributo ForeignKey.
			/// </summary>
			/// <param name="ForeignPropertyName">Nome da propriedade na classe extrangeira</param>
			/// <param name="LocalPropertyName">Nome da propriedade na classe local</param>
			/// <remarks></remarks>
			public ForeignKeyAttribute(string ForeignPropertyName, string LocalPropertyName)
			{
				_ForeignPropertyName = ForeignPropertyName;
				_LocalPropertyName = LocalPropertyName;
				
			}
			
			private string _ForeignPropertyName;
			
			public string ForeignPropertyName
			{
				get
				{
					return _ForeignPropertyName;
				}
				set
				{
					_ForeignPropertyName = value;
				}
			}
			
			
			private string _LocalPropertyName;
			public string LocalPropertyName
			{
				get
				{
					return _LocalPropertyName;
				}
				set
				{
					_LocalPropertyName = value;
				}
			}
			
		}
		
		
		/// <summary>
		/// Identifica um campo especial. Utilizado para expressões nativas da linguagem SQL
		/// </summary>
		/// <remarks></remarks>
		[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]public class SpecialFieldAttribute : Attribute
		{
			
			
			private SpecialFieldAttribute()
			{
			}
			
			
			/// <summary>
			/// Inicializa uma instancia de campo especial
			/// </summary>
			/// <param name="Expression">Expressão a ser usada na construção da consulta. Utilize funções específicas da linguagem SQL (Ex. T-SQL, PL/SQL) que estiver usando.</param>
			/// <remarks></remarks>
			public SpecialFieldAttribute(string Expression)
			{
				this.Expression = Expression;
			}
			
			
			private string _Expression;
			public string Expression
			{
				get
				{
					return _Expression;
				}
				set
				{
					_Expression = value;
				}
			}
			
			
		}
		#endregion
		
	}
	
	
}
