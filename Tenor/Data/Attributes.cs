using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Configuration;
using System.Data.Common;
using Tenor.BLL;
using Tenor.Data.Dialects;
using System.Reflection;
using System.ComponentModel;

namespace Tenor.Data
{
    #region "Metadata - Property Infos"
    internal sealed class TableInfo
    {
        private TableInfo() { }

        private TableAttribute attribute;
        private Type type;
        public static TableInfo CreateTableInfo(Type type)
        {
            if (type.BaseType == typeof(object))
            {
                throw new InvalidMappingException(type);
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
            ConnectionStringSettings conn = null;
            if (string.IsNullOrEmpty(attribute.ConnectionName))
                conn = BLL.BLLBase.SystemConnection;
            else
            {
                conn = ConfigurationManager.ConnectionStrings[attribute.ConnectionName];
                if (conn == null)
                    throw new TenorException(string.Format("Cannot find a connection with the name '{0}'", attribute.ConnectionName));
            }

            return conn;
        }
    }

    public abstract class PropInfo
    {

        protected System.Reflection.PropertyInfo _RelatedProperty;
        public virtual System.Reflection.PropertyInfo RelatedProperty
        {
            get
            {
                return _RelatedProperty;
            }
        }

        public object PropertyValue(object Instance)
        {
            return PropertyValue(Instance, true);
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
                    res =  (ConvertNullToDBNull ? DBNull.Value : null);
                }
                else if (res.GetType() == typeof(Nullable<>))
                {

                    if (!System.Convert.ToBoolean(res.GetType().GetProperty("HasValue").GetValue(res, null)))
                    {
                        res = (ConvertNullToDBNull ? DBNull.Value : null);
                    }
                    else
                    {
                        res = res.GetType().GetProperty("Value").GetValue(res, null);
                    }
                }

                if (res != null && res.GetType().IsEnum)
                {
                    System.Reflection.FieldInfo fInfo = res.GetType().GetField(res.ToString());
                    if (fInfo != null) {
                        EnumDatabaseValueAttribute[] att = (EnumDatabaseValueAttribute[])fInfo.GetCustomAttributes(typeof(EnumDatabaseValueAttribute), true);
                        if (att.Length == 1)
                        {
                            res = att[0].Value;
                        }
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                throw (new TenorException("Cannot get \'" + RelatedProperty.Name + "\' value. See inner exception for details.", ex));
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
                    if (RelatedProperty.PropertyType.IsGenericType && RelatedProperty.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        Type[] nullType = RelatedProperty.PropertyType.GetGenericArguments();
                        
                        // Changes the type of the value for providers that don't detect types correctly
                        Type type = null;
                        if (nullType.Length > 0)
                            type = nullType[0];

                        if (type != null && value.GetType() != type)
                        {
                                CheckEnumType(ref value, ref type);
                            
                            value = Convert.ChangeType(value, type);
                        }

                        // if it's a nullable type, we need to set the value already converted
                        // we call the constructor of the non-nullable part and then set the nullable property value
                        ConstructorInfo ctor = RelatedProperty.PropertyType.GetConstructor(nullType);
                        value = ctor.Invoke(new object[] { value });
                        RelatedProperty.SetValue(Instance, value, null);
                    }
                    else
                    {
                        Type type = RelatedProperty.PropertyType;

                        // Changes the type of the value for providers that don't detect types correctly
                        if (value.GetType() != type)
                        {
                            CheckEnumType(ref value, ref type);

                            value = Convert.ChangeType(value, type);
                        }

                        RelatedProperty.SetValue(Instance, value, null);
                    }
                }
            }
            catch (Exception ex)
            {
                throw (new TenorException("Cannot set \'" + this.RelatedProperty.Name + "\' value. See inner exception for details.", ex));
            }

        }

        /// <summary>
        /// Converts the enum to the right type, and if its a string, converts it to the enum value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        private static void CheckEnumType(ref object value, ref Type type)
        {
            if (type.IsEnum)
            {
                System.Reflection.FieldInfo[] values = type.GetFields();
                //Only try to find EnumDatabaseValue if its a string 
                if (value.GetType() == typeof(string))
                {
                    Array realValues = Enum.GetValues(type);
                    for (int i = 1; i < values.Length; i++)
                    {
                        EnumDatabaseValueAttribute[] att = (EnumDatabaseValueAttribute[])values[i].GetCustomAttributes(typeof(EnumDatabaseValueAttribute), true);
                        if (att.Length == 1 && att[0].Value == value.ToString())
                        {
                            value = realValues.GetValue(i - 1);
                            break;
                        }
                    }
                }

                //Set the real enum type. Can be Int16,32 and 64.
                type = values[0].FieldType;
            }
        }

        public void SetPropertyValue(object Instance, object value)
        {
            SetPropertyValue(Instance, true, value);
        }
    }

    /// <summary>
    /// Encapsulates all foreign key metadata.
    /// </summary>
    internal sealed class ForeignKeyInfo : PropInfo
    {
        private ForeignKeyInfo()
        {
        }


        internal static ForeignKeyInfo Create(System.Reflection.PropertyInfo theProperty)
        {
            if (theProperty == null)
                return null;
            ForeignKeyInfo fk = new ForeignKeyInfo();

            fk._RelatedProperty = theProperty;

            fk.fieldAttributes = (ForeignKeyFieldAttribute[])(fk.RelatedProperty.GetCustomAttributes(typeof(ForeignKeyFieldAttribute), true));

            ForeignKeyAttribute[] fkAttribs = (ForeignKeyAttribute[])(fk.RelatedProperty.GetCustomAttributes(typeof(ForeignKeyAttribute), true));

            if (fk.fieldAttributes.Length == 0)
            {
                return null;
            }
            else
            {
                if (fkAttribs.Length == 0)
                    fk.foreignKeyDefinition = new ForeignKeyAttribute();
                else
                    fk.foreignKeyDefinition = fkAttribs[0];
                return fk;
            }
        }

        private ForeignKeyAttribute foreignKeyDefinition;

        public ForeignKeyAttribute ForeignKeyDefinition
        {
            get { return foreignKeyDefinition; }
        }

        private ForeignKeyFieldAttribute[] fieldAttributes;
        internal ForeignKeyFieldAttribute[] RelatedAttributes
        {
            get
            {
                return fieldAttributes;
            }
        }


        internal bool IsArray
        {
            get
            {
                return RelatedProperty.PropertyType.IsArray || RelatedProperty.PropertyType.GetInterface("System.Collections.IList") != null || (RelatedProperty.PropertyType.IsGenericType && RelatedProperty.PropertyType.GetGenericTypeDefinition() == typeof(BLL.BLLCollection<>));
            }
        }

        internal Type ElementType
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
                        throw (new TenorException("Could not find the collection element type"));
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


        internal FieldInfo[] ForeignFields
        {
            get
            {
                List<FieldInfo> res = new List<FieldInfo>();
                foreach (ForeignKeyFieldAttribute i in this.RelatedAttributes)
                {
                    System.Reflection.PropertyInfo prop = this.ElementType.GetProperty(i.ForeignPropertyName);
                    if (prop != null)
                    {
                        FieldInfo field = FieldInfo.Create(prop);
                        if (field != null)
                        {
                            res.Add(field);
                        }

                    }
                }
                return res.ToArray();
            }
        }

        internal FieldInfo[] LocalFields
        {
            get
            {
                List<FieldInfo> res = new List<FieldInfo>();
                foreach (ForeignKeyFieldAttribute i in this.RelatedAttributes)
                {
                    System.Reflection.PropertyInfo prop = this.RelatedProperty.DeclaringType.GetProperty(i.LocalPropertyName);
                    if (prop != null)
                    {
                        FieldInfo field = FieldInfo.Create(prop);
                        if (field != null)
                        {
                            res.Add(field);
                        }

                    }
                }
                return res.ToArray();
            }
        }


        internal bool IsManyToMany 
        {
            get 
            {
                return IsArray && ManyToManyTable != null;
            }
        }

        internal string ManyToManyTable
        {
            get
            {
                if (string.IsNullOrEmpty(this.ForeignKeyDefinition.ManyToManyTable))
                    return null;
                else
                    return this.ForeignKeyDefinition.ManyToManyTable;
            }
        }

        internal bool PersistDataOnSave
        {
            get
            {
                return IsManyToMany && this.ForeignKeyDefinition.PersistManyToManyOnSave;
            }
        }

        internal string ManyToManyTablePrefix
        {
            get
            {
                if (string.IsNullOrEmpty(this.ForeignKeyDefinition.ManyToManyTablePrefix))
                    return null;
                else
                    return this.ForeignKeyDefinition.ManyToManyTablePrefix;
            }
        }

        internal string[] LocalManyToManyFields
        {
            get
            {
                List<string> res = new List<string>();
                foreach (ForeignKeyFieldAttribute i in this.RelatedAttributes)
                {
                    if (string.IsNullOrEmpty(i.LocalFieldOnManyToManyTable))
                        throw new InvalidManyToManyMappingException(this.RelatedProperty.DeclaringType, this.RelatedProperty.Name);
                    res.Add(i.LocalFieldOnManyToManyTable);
                }
                return res.ToArray();
            }
        }

        internal string[] ForeignManyToManyFields
        {
            get
            {
                List<string> res = new List<string>();
                foreach (ForeignKeyFieldAttribute i in this.RelatedAttributes)
                {
                    if (string.IsNullOrEmpty(i.ForeignFieldOnManyToManyTable))
                        throw new InvalidManyToManyMappingException(this.RelatedProperty.DeclaringType, this.RelatedProperty.Name);
                    res.Add(i.ForeignFieldOnManyToManyTable);
                }
                return res.ToArray();
            }
        }


        public override string ToString()
        {
            return this.GetType().Name + ": " + RelatedProperty.DeclaringType.Name + "." + RelatedProperty.Name;
        }
    }

    /// <summary>
    /// Encapsulates all field metadata.
    /// </summary>
    internal sealed class FieldInfo : PropInfo
    {
        
        private FieldAttribute _Attribute;

        private FieldInfo() { }

        internal static FieldInfo Create(System.Reflection.PropertyInfo theProperty)
        {
            FieldInfo fi = new FieldInfo();

            fi._RelatedProperty = theProperty;
            FieldAttribute[] atts = (FieldAttribute[])(fi.RelatedProperty.GetCustomAttributes(typeof(FieldAttribute), true));
            if (atts.Length > 1)
            {
                throw new MissingFieldException(theProperty.DeclaringType, theProperty.Name);
            }
            if (atts.Length == 0)
            {
                return null;
            }

        
            fi._Attribute = atts[0];

            return fi;
        }

        internal Type FieldType
        {
            get
            {
                return RelatedProperty.PropertyType;
            }
        }

        internal bool PrimaryKey
        {
            get
            {
                return _Attribute.PrimaryKey;
            }
        }

        internal bool AutoNumber
        {
            get
            {
                return _Attribute.AutoNumber;
            }
        }

        internal string InsertSQL
        {
            get
            {
                return _Attribute.InsertSQL;
            }
        }

        internal bool LazyLoading
        {
            get
            {
                return _Attribute.LazyLoading;
            }
        }

        internal string DataFieldName
        {
            get
            {
                if (string.IsNullOrEmpty(_Attribute.FieldName))
                {
                    return RelatedProperty.Name;
                }
                else
                {
                    return _Attribute.FieldName;
                }
            }
        }

        internal string ParamName
        {
            get
            {
                return DataFieldName.ToLower().Replace(" ", "_");
            }
        }
    }

    /// <summary>
    /// Encapsulates all special field metadata.
    /// </summary>
    public sealed class SpecialFieldInfo : PropInfo
    {

        private SpecialFieldInfo() { }

        public static SpecialFieldInfo Create(System.Reflection.PropertyInfo theProperty)
        {
            SpecialFieldInfo sp = new SpecialFieldInfo();

            sp._RelatedProperty = theProperty;
            SpecialFieldAttribute[] atts = (SpecialFieldAttribute[])(sp.RelatedProperty.GetCustomAttributes(typeof(SpecialFieldAttribute), true));
            if (atts.Length == 0)
            {
                return null;
            }
            else if (atts.Length > 1)
            {
                throw new MissingSpecialFieldException(theProperty.DeclaringType, theProperty.Name);
            }
            sp._Attribute = atts[0];
            return sp;

        }

        private SpecialFieldAttribute _Attribute;

        public string Expression
        {
            get
            {
                return _Attribute.Expression;
            }
        }


        public string Alias
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
    /// Represents a table field mapping.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class FieldAttribute : Attribute
    {

        public FieldAttribute()
        {
        }

        public FieldAttribute(string fieldName)
        {
            _FieldName = fieldName;
        }


        public FieldAttribute(bool primaryKey, bool autoNumber)
        {
            _PrimaryKey = primaryKey;
            _AutoNumber = autoNumber;
        }

        public FieldAttribute(string fieldName, bool primaryKey, bool autoNumber)
            : this(fieldName)
        {
            _PrimaryKey = primaryKey;
            _AutoNumber = autoNumber;
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

        private string _InsertSQL;
        /// <summary>
        /// Used to set a sequence name used during an insert
        /// </summary>
        public string InsertSQL
        {
            get
            {
                return _InsertSQL;
            }
            set
            {
                _InsertSQL = value;
            }
        }
    }

    /// <summary>
    /// <para>Represents a foreign relationship. This attribute can be applied to a property that returns an instance of the foreign class, or a <see cref="Tenor.BLL.BLLCollection"/> of the foreign class.</para>
    /// </summary>
    /// <remarks>
    /// <para>Use this with <see cref="ForeignKeyFieldAttribute"/> instances.</para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ForeignKeyAttribute : Attribute
    {
        public ForeignKeyAttribute() {}

        /// <param name="manyToManyTable">The name of many-to-many table used to make the relation.</param>
        public ForeignKeyAttribute(string manyToManyTable)
        {
            this.ManyToManyTable = manyToManyTable;
        }

        /// <param name="manyToManyTable">The name of many-to-many table used to make the relation.</param>
        /// <param name="manyToManyTablePrefix">Contains the database prefix of that table, usually the schema.</param>
        public ForeignKeyAttribute(string manyToManyTable, string manyToManyTablePrefix)
        {
            this.ManyToManyTable = manyToManyTable;
            this.ManyToManyTablePrefix = manyToManyTablePrefix;
        }

        string manyToManyTableName;
        string manyToManyTablePrefix;

        /// <summary>
        /// The name of many-to-many table used to make the relation.
        /// </summary>
        public string ManyToManyTable
        {
            get { return manyToManyTableName; }
            set { manyToManyTableName = value; }
        }

        /// <summary>
        /// The name of many-to-many table prefix used to make the relation.
        /// </summary>
        public string ManyToManyTablePrefix
        {
            get { return manyToManyTablePrefix; }
            set { manyToManyTablePrefix = value; }
        }

        bool persistOnSave = true;

        /// <summary>
        /// Determines if this relation will be persisted upon a save operation. Defaults to true.
        /// </summary>
        public bool PersistManyToManyOnSave
        {
            get { return persistOnSave; }
            set { persistOnSave = value; }
        }
     }

    /// <summary>
    /// <para>Represents a foreign relationship pair.</para>
    /// </summary>
    /// <remarks>
    /// <para>If you have a composite relation, you can define more than one of this attribute to the same property.</para>
    /// <para>To define a Many-To-One relation, create a read-write property that returns an instance of the desired class, and set one or more of ForeignKeyFieldAttribute to that property to map local and foreign properties.</para>
    /// <para>To define a One-To-Many or Many-To-Many relation, create a read-only property that returns a collection of the desired class, and set one or more of ForeignKeyFieldAttribute to that property to map local and foreign properties.</para>
    /// <para>Many-To-Many relations needs a ForeignKeyAttribute with ManyToManyTable set with schema (when necessary) and table name, in addition of field mapping of that table.</para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class ForeignKeyFieldAttribute : Attribute
    {


        private ForeignKeyFieldAttribute()
        {
        }

        /// <param name="foreignPropertyName">A property declared on the foreign class. See <see cref="ForeignKeyAttribute"/> for details.</param>
        /// <param name="localPropertyName">A property declared on this class.</param>
        public ForeignKeyFieldAttribute(string foreignPropertyName, string localPropertyName)
        {
            _ForeignPropertyName = foreignPropertyName;
            _LocalPropertyName = localPropertyName;
        }


        /// <param name="foreignPropertyName">A property declared on the foreign class. See <see cref="ForeignKeyAttribute"/> for details.</param>
        /// <param name="localPropertyName">A property declared on this class.</param>
        /// <param name="foreignFieldOnManyToManyTable">The foreign property correspondence field on many-to-many table.</param>
        /// <param name="localFieldOnManyToManyTable">The local property correspondence field on many-to-many table.</param>
        public ForeignKeyFieldAttribute(string foreignPropertyName, string localPropertyName, string foreignFieldOnManyToManyTable, string localFieldOnManyToManyTable)
        {
            _ForeignPropertyName = foreignPropertyName;
            _LocalPropertyName = localPropertyName;
            this.foreignFieldOnManyToManyTable = foreignFieldOnManyToManyTable;
            this.localFieldOnManyToManyTable = localFieldOnManyToManyTable;
        }

        private string _ForeignPropertyName;

        /// <summary>
        /// A property declared on the foreign class. See <see cref="ForeignKeyFieldAttribute"/> for details.
        /// </summary>
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
        /// <summary>
        /// A property declared on this class.
        /// </summary>
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

        string localFieldOnManyToManyTable;

        /// <summary>
        /// The local property correspondence field on many-to-many table.
        /// </summary>
        public string LocalFieldOnManyToManyTable
        {
            get { return localFieldOnManyToManyTable; }
            set { localFieldOnManyToManyTable = value; }
        }
        string foreignFieldOnManyToManyTable;
        /// <summary>
        /// The foreign property correspondence field on many-to-many table.
        /// </summary>
        public string ForeignFieldOnManyToManyTable
        {
            get { return foreignFieldOnManyToManyTable; }
            set { foreignFieldOnManyToManyTable = value; }
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


        /// <param name="expression">A SQL expression that will be used to return a value to the applied property. This expression must be in the same language of your DBMS.</param>
        /// <remarks></remarks>
        public SpecialFieldAttribute(string expression)
        {
            this.Expression = expression;
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

    /// <summary>
    /// Represents the value that Tenor will use to persist the enum value. 
    /// Useful on legacy databases.
    /// </summary>
    [global::System.AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class EnumDatabaseValueAttribute : Attribute
    {
        readonly string value;

        public EnumDatabaseValueAttribute(string value)
        {
            this.value = value;
        }

        public string Value
        {
            get { return value; }
        }
    }
    #endregion

}
