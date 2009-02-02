using System;
using System.Collections.Generic;
using System.Text;
using Tenor.Data;
using System.Collections;
using System.Configuration;
using System.Data.Common;
using System.Data;


namespace Tenor.BLL
{
    public abstract partial class BLLBase
    {
        private Dictionary<string, object> propertyData = new Dictionary<string, object>();



 
        /// <summary>
        /// Loads a foreign key property.
        /// </summary>
        /// <param name="propertyName">Property name whose set will be called to set data.</param>
        /// <param name="lazyLoading">Defines weather to enable the lazy loading.</param>
        /// <remarks></remarks>
        internal object LoadForeign(string propertyName, bool lazyLoading)
        {
            return LoadForeign(propertyName, lazyLoading, null, null);
        }


        /// <summary>
        /// Loads a foreign key property.
        /// </summary>
        /// <param name="propertyName">Property name whose set will be called to set data.</param>
        /// <param name="lazyLoading">Defines weather to enable the lazy loading.</param>
        /// <remarks></remarks>
        internal object LoadForeign(string property, bool LazyLoading, Type returnType, ConnectionStringSettings connection) 
        {
            if (!propertyData.ContainsKey(property))
            {
                System.Reflection.PropertyInfo FieldP = null;
                if (returnType != null)
                {
                    FieldP = this.GetType().GetProperty(property, returnType);
                }
                else
                {
                    FieldP = this.GetType().GetProperty(property);
                }
                ForeignKeyInfo Field = new ForeignKeyInfo(FieldP);


                //Dim filters As String = ""
                //Dim params As New List(Of Data.Parameter)

                TableInfo table = TableInfo.CreateTableInfo(Field.ElementType);
                if (connection == null)
                    connection = table.GetConnection();


                if (!Field.IsArray)
                {
                    if (table.Cacheable)
                    {
                        BLLBase instance = (BLLBase)Activator.CreateInstance(table.RelatedTable);
                        //We found a cacheble instance
                        for (int i = 0; i <= Field.ForeignFields.Length - 1; i++)
                        {
                            Field.ForeignFields[i].SetPropertyValue(instance, Field.LocalFields[i].PropertyValue(this));
                        }
                        instance.Bind(LazyLoading);
                        Field.SetPropertyValue(this, instance);
                        return instance;
                    }
                }


                SearchOptions sc = new SearchOptions(Field.ElementType);
                sc.LazyLoading = LazyLoading;



                for (int i = 0; i <= Field.ForeignFields.Length - 1; i++)
                {
                    if (i > 0)
                    {
                        sc.Conditions.Add(Tenor.Data.LogicalOperator.And);
                    }
                    // o  campo externo é igual ao campo local.
                    if (Field.LocalFields.Length - 1 < i)
                    {
                        throw (new MissingFieldsException());
                    }
                    sc.Conditions.Add(Field.ForeignFields[i].RelatedProperty.Name, Field.LocalFields[i].PropertyValue(this));

                }
                if (sc.Conditions.Count == 0)
                {
                    throw (new MissingFieldsException());
                }


                BLLBase[] instances = Search(sc, connection);

                if (this._IsLazyDisabled)
                {
                    foreach (BLLBase i in instances)
                    {
                        i._IsLazyDisabled = this._IsLazyDisabled;
                    }
                }


                if (Field.IsArray)
                {
                    if (Field.RelatedProperty.PropertyType.IsArray)
                    {
                        propertyData.Add(property, instances);
                    }
                    else
                    {
                        //It must have another way to create it, string is not cool.
                        Type listof = Type.GetType("Tenor.BLL.BLLCollection`1[[" + Field.ElementType.AssemblyQualifiedName + "]]");
                        System.Reflection.ConstructorInfo ctor = listof.GetConstructor(System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, new Type[] { typeof(BLLBase), typeof(string) }, null);
                        IList obj = (IList)ctor.Invoke(new object[] { (BLLBase)this, property });
                        obj.Clear();
                        foreach (BLLBase i in instances)
                        {
                            obj.Add(i);
                        }
                        propertyData.Add(property, obj);
                    }
                }
                else
                {
                    if (instances.Length == 0)
                    {
                        propertyData.Add(property, null);
                    }
                    else
                    {
                        if (instances.Length > 1)
                        {
                            throw new ManyRecordsFoundException();//"LoadingForeignKey-ManyToOne: More than one instance was returned");
                        }
                        propertyData.Add(property, instances[0]);
                    }
                }
            }
            return propertyData[property];
        }

        private string GetCallingProperty()
        {
            System.Diagnostics.StackTrace stack = new System.Diagnostics.StackTrace();
            System.Reflection.MethodBase method = stack.GetFrame(2).GetMethod();
            if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))
            {
                string propName = method.Name.Substring(4);
                System.Reflection.PropertyInfo prop = this.GetType().GetProperty(propName);
                if (prop == null)
                {
                    throw new InvalidOperationException("You can only call this method within a Property Get of the current instance.");
                }
                return propName;
            }
            else
            {
                throw new InvalidOperationException("You can only call this method within a Property Get.");
            }
        }


        /// <summary>
        /// Load the lazyloading field.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected object GetLazyProperty()
        {
            string propertyName = GetCallingProperty();
            return GetLazyProperty(propertyName);
        }

        /// <summary>
        /// Carrega as informações de um campo marcado para lazy loading
        /// </summary>
        /// <remarks></remarks>
        internal virtual object GetLazyProperty(string propertyName)
        {
            if (!propertyData.ContainsKey(propertyName))
            {
                System.Reflection.PropertyInfo fieldP = this.GetType().GetProperty(propertyName);
                if (fieldP == null)
                {
                    throw (new ArgumentException("The property specified was not found.", "Property", null));
                }

                ForeignKeyInfo fkInfo = null;
                try
                {
                    //TODO: Change this to something that don't throws exceptions
                    fkInfo = new ForeignKeyInfo(fieldP);
                }
                catch { }
                if (fkInfo != null)
                {
                    return LoadForeign(propertyName, true);
                }

                //Continue to the lazy property loading
                FieldInfo field = new FieldInfo(fieldP);
                string sql = "";
                sql += " SELECT " + "\r\n";
                sql += " {0}" + "\r\n";
                sql += " FROM {1} " + "\r\n";
                sql += " WHERE {2}";


                TableInfo table = TableInfo.CreateTableInfo(this.GetType());
                ConnectionStringSettings connection = table.GetConnection();
                DbCommandBuilder builder = Helper.GetCommandBuilder(connection);

                string fields = builder.QuoteIdentifier(field.DataFieldName);
                string from = table.GetSchemaAndTable();

                List<TenorParameter> parameters = new List<TenorParameter>();
                string filter = string.Empty;

                foreach (FieldInfo f in GetFields(this.GetType(), true))
                {
                    //Get primary keys
                    string paramName = f.DataFieldName.ToLower().Replace(" ", "_");
                    TenorParameter param = new TenorParameter(paramName, f.PropertyValue(this));

                    filter += " AND " + builder.QuoteIdentifier(f.DataFieldName) + (" = " + Helper.GetParameterPrefix(connection) + paramName);
                    parameters.Add(param);
                }
                if (string.IsNullOrEmpty(filter))
                {
                    throw (new MissingPrimaryKeyException());
                }
                filter = filter.Substring(5);

                Tenor.Data.DataTable rs = new Tenor.Data.DataTable(string.Format(
                    sql,
                    fields,
                    from,
                    filter), parameters.ToArray(), connection);

                rs.Bind();

                if (rs.Rows.Count == 0)
                {
                    throw (new RecordNotFoundException());
                }
                else if (rs.Rows.Count > 1)
                {
                    throw new ManyRecordsFoundException();
                }
                else
                {
                    propertyData[propertyName] = rs.Rows[0][field.DataFieldName];
                }

            }
            return propertyData[propertyName];
        }

        protected virtual void SetLazyProperty(object value)
        {
            string propertyName = GetCallingProperty();
            SetLazyProperty(propertyName, value);
        }

        internal virtual void SetLazyProperty(string propertyName, object value)
        {
            //Just to check
            System.Reflection.PropertyInfo fieldP = this.GetType().GetProperty(propertyName);
            if (fieldP == null)
            {
                throw (new ArgumentException("The property specified was not found.", "Property", null));
            }

            if (propertyData.ContainsKey(propertyName))
            {
                propertyData[propertyName] = value;
            }
            else
            {
                propertyData.Add(propertyName, value);
            }
        }


        protected bool _IsLazyDisabled = false;
        /// <summary>
        /// Returns a value indicating whether or not this class is serializing.
        /// </summary>
        /// <returns>True or False</returns>
        /// <remarks></remarks>
        protected virtual bool LazyLoadingDisabled
        {
            get
            {
                return _IsLazyDisabled;
            }
        }


        /// <summary>
        /// Desativa o LazyLoading para as intâncias passadas.
        /// </summary>
        /// <param name="items"></param>
        /// <remarks></remarks>
        public static void DisableLazyLoading(BLLBase[] items)
        {
            foreach (BLLBase i in items)
            {
                i.ChangeLazyLoadingStatus(true);
            }
        }

        /// <summary>
        /// Ativa o LazyLoading para as intâncias passadas.
        /// </summary>
        /// <param name="items"></param>
        /// <remarks></remarks>
        public static void EnableLazyLoading(BLLBase[] items)
        {
            foreach (BLLBase i in items)
            {
                i.ChangeLazyLoadingStatus(false);
            }
        }


        private void ChangeLazyLoadingStatus(bool status)
        {
            this._IsLazyDisabled = status;

            foreach (ForeignKeyInfo fk in BLLBase.GetForeignKeys(this.GetType()))
            {
                if (fk.IsArray)
                {
                    ICollection col = (ICollection)(fk.PropertyValue(this, false));
                    if (col != null)
                    {
                        foreach (BLLBase item in col)
                        {
                            item.ChangeLazyLoadingStatus(status);
                        }
                    }
                }
                else
                {
                    BLLBase obj = (BLLBase)(fk.PropertyValue(this, false));
                    if (obj != null)
                    {
                        obj.ChangeLazyLoadingStatus(status);
                    }
                }
            }
        }
 
    }
}
