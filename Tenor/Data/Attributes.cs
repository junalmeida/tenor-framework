using System.Diagnostics;
using System;
using System.Collections;
using Microsoft.VisualBasic;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Configuration;
using System.Data.Common;
using Tenor.BLL;

namespace Tenor.Data
{
    #region "Property Infos"
    internal sealed class TableInfo
    {
        private TableInfo()        {        }

        private TableAttribute attribute;
        private Type type;
        public static TableInfo CreateTableInfo(Type type)
        {
            if (type.BaseType == typeof(object))
            {
                throw new InvalidMappingException();
            }

            TableAttribute[] att = (TableAttribute[])type.GetCustomAttributes(typeof(TableAttribute), false);
            if (att.Length == 0)
                return null;

            TableInfo info = new TableInfo();
            info.type = type;
            info.attribute = att[0];

            if (type.BaseType != typeof(BLL.BLLBase))
                info.parent = CreateTableInfo(type.BaseType);
            return info;
        }

        private TableInfo parent;
        public TableInfo Parent
        {
            get
            {
                return parent;
            }
        }

        public Type RelatedTable
        {
            get
            {
                return type;
            }
        }


        public string TableName
        {
            get
            {
                if (string.IsNullOrEmpty(attribute.TableName))
                    return type.Name;
                else
                    return attribute.TableName;
            }
        }

        public string Prefix
        {
            get
            {
                return attribute.Prefix;
            }
        }

        public bool Cacheable
        {
            get
            {
                return attribute.Cacheable;
            }
        }

        public ConnectionStringSettings GetConnection()
        {
            if (string.IsNullOrEmpty(attribute.ConnectionName))
            {
                return BLL.BLLBase.SystemConnection;
            }
            else
            {
                ConnectionStringSettings conn = ConfigurationManager.ConnectionStrings[attribute.ConnectionName];
                if (conn == null)
                {
                    throw new InvalidOperationException(string.Format("Cannot find a connection with the name '{0}'", attribute.ConnectionName));
                }
                return conn;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetSchemaAndTable()
        {
            DbCommandBuilder builder = Helper.GetCommandBuilder(this.GetConnection());

            string res = builder.QuoteIdentifier(this.TableName);
            if (!string.IsNullOrEmpty(this.Prefix))
            {
                res = this.Prefix + "." + res;
            }

            return res;
        }

        ///// <summary>
        ///// Gets the full table identifier using a localization view. See the documentation.
        ///// </summary>
        ///// <param name="instance"></param>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //protected static string GetSchemaAndView(BLLBase instance)
        //{
        //    string schemaName = "Localization";


        //    string res = string.Empty;
        //    res = instance.CommandBuilder.QuoteIdentifier(schemaName) + ".";
        //    res += instance.CommandBuilder.QuoteIdentifier(instance.SchemaName + instance.TableName);

        //    return res;
        //}

        public string GetTableAlias()
        {
            return type.Name; //Change this to a valid table alias.
        }
    }

    internal abstract class PropInfo
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
                        if (!System.Convert.ToBoolean(res.GetType().GetProperty("HasValue").GetValue(res, null)))
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

    internal sealed class ForeignKeyInfo : PropInfo
    {


        public ForeignKeyInfo(System.Reflection.PropertyInfo @Property)
        {
            RelatedProperty = @Property;

            _RelatedAttributes = (ForeignKeyAttribute[])(RelatedProperty.GetCustomAttributes(typeof(ForeignKeyAttribute), true));
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
                else if (RelatedProperty.PropertyType.GetInterface("System.Collections.IList") != null || (RelatedProperty.PropertyType.IsGenericType && RelatedProperty.PropertyType.GetGenericTypeDefinition() == typeof(BLL.BLLCollection<>)))
                {


                    Type ftype = typeof(object);
                    foreach (System.Reflection.MemberInfo i in RelatedProperty.PropertyType.GetDefaultMembers())
                    {
                        if (i.MemberType == System.Reflection.MemberTypes.Property)
                        {
                            ftype = ((System.Reflection.PropertyInfo)i).PropertyType;
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
                        FieldAttribute[] atts = (FieldAttribute[])(prop.GetCustomAttributes(typeof(FieldAttribute), true));
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
                        FieldAttribute[] atts = (FieldAttribute[])(prop.GetCustomAttributes(typeof(FieldAttribute), true));
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

    internal sealed class FieldInfo : PropInfo
    {


        public FieldInfo(System.Reflection.PropertyInfo @Property)
        {


            RelatedProperty = @Property;
            FieldAttribute[] atts = (FieldAttribute[])(RelatedProperty.GetCustomAttributes(typeof(FieldAttribute), true));
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

    internal sealed class SpecialFieldInfo : PropInfo
    {


        public SpecialFieldInfo(System.Reflection.PropertyInfo @Property)
        {


            RelatedProperty = @Property;
            SpecialFieldAttribute[] atts = (SpecialFieldAttribute[])(RelatedProperty.GetCustomAttributes(typeof(SpecialFieldAttribute), true));
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

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class TableAttribute : Attribute
    {
        readonly string tableName;
        readonly string prefix;

        /// <summary>
        /// Represents an entity persistence.
        /// </summary>
        public TableAttribute()
        {
        }

        /// <summary>
        /// Represents an entity persistence.
        /// </summary>
        /// <param name="tableName">Sets the physical table name.</param>
        public TableAttribute(string tableName)
        {
            this.tableName = tableName;
        }

        /// <summary>
        /// Represents an entity persistence.
        /// </summary>
        /// <param name="tableName">Sets the physical table name. This attribute will be quoted.</param>
        /// <param name="prefix">Sets the physical table prefix. This attribute will not be quoted, and you can 
        /// specify here this such as schemas and database names in yours sgdb language.</param>
        public TableAttribute(string tableName, string prefix) : this(tableName)
        {
            this.prefix  = prefix;
        }

        public string TableName
        {
            get
            {
                return tableName;
            }
        }

        public string Prefix
        {
            get
            {
                return prefix;
            }
        }

        private bool cacheable = false;
        public bool Cacheable
        {
            get
            {
                return cacheable;
            }
            set
            {
                cacheable = value;
            }
        }

        private string connectionName;
        public string ConnectionName
        {
            get
            {
                return connectionName;
            }
            set
            {
                connectionName = value;
            }
        }
    }

    /// <summary>
    /// Represents a a table field.
    /// </summary>
    /// <remarks></remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class FieldAttribute : Attribute
    {

        public FieldAttribute()
        {
        }

        public FieldAttribute(bool PrimaryKey, bool AutoNumber)
        {
            _PrimaryKey = PrimaryKey;
            _AutoNumber = AutoNumber;
        }

        public FieldAttribute(string FieldName, bool AutoNumber, bool PrimaryKey)
        {
            _FieldName = FieldName;
            _PrimaryKey = PrimaryKey;
            _AutoNumber = AutoNumber;
        }

        private string _FieldName;
        /// <summary>
        /// Sets the physical field name.
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
        /// Defines wheather this field is part of a primary key index.
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
        /// Defines wheather this field is an auto-number field. Generally this indicates wheater or not include this field on an insert field.
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

        private string defaultValue;
        /// <summary>
        /// Sets the SQL instruction to run on insert commands.
        /// Generally used to get a sequence value when this is not autmatic on your sgdb.
        /// </summary>
        public string InsertSQL
        {
            get
            {
                return defaultValue;
            }
            set
            {
                defaultValue = value;
            }
        }

    }

    /// <summary>
    /// Represents a foreign relationship.
    /// </summary>
    /// <remarks></remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class ForeignKeyAttribute : Attribute
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
    /// Represents a special field. Generally used to return values from a SQL instruction.
    /// </summary>
    /// <remarks></remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class SpecialFieldAttribute : Attribute
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
