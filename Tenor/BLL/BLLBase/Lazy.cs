/* Copyright (c) 2009 Marcos Almeida Jr, Rachel Carvalho and Vinicius Barbosa.
 *
 * See the file license.txt for copying permission.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Tenor.Data;
using System.Collections;
using System.Configuration;
using System.Data.Common;
using System.Data;
using Tenor.Data.Dialects;


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
        internal object LoadForeign(string propertyName)
        {
            return LoadForeign(propertyName, null, null);
        }

        /// <summary>
        /// Loads a foreign key property.
        /// </summary>
        /// <param name="propertyName">Property name whose set will be called to set data.</param>
        /// <param name="lazyLoading">Defines weather to enable the lazy loading.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="returnType">The return type of the property.</param>
        /// <remarks></remarks>
        internal object LoadForeign(string propertyName, Type returnType, ConnectionStringSettings connection)
        {
            System.Reflection.PropertyInfo fieldP = null;
            if (returnType != null)
            {
                fieldP = this.GetType().GetProperty(propertyName, returnType);
            }
            else
            {
                fieldP = this.GetType().GetProperty(propertyName);
            }
            return LoadForeign(fieldP, connection);
        }

        /// <summary>
        /// Loads a foreign key property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="lazyLoading">Defines weather to enable the lazy loading.</param>
        /// <param name="connection">The connection.</param>
        /// <remarks></remarks>
        internal object LoadForeign(System.Reflection.PropertyInfo property, ConnectionStringSettings connection)
        {
            if (!propertyData.ContainsKey(property.Name))
            {
                ForeignKeyInfo field = ForeignKeyInfo.Create(property);
                if (field == null)
                    throw new Tenor.Data.MissingFieldException(property.DeclaringType, property.Name);
                /*
                Dim filters As String = ""
                Dim params As New List(Of Data.Parameter)
                */
                TableInfo table = TableInfo.CreateTableInfo(field.ElementType);
                if (connection == null)
                    connection = table.GetConnection();


                if (!field.IsArray && table.Cacheable)
                {
                    //We found a cacheble instance, so we don't need to search.
                    BLLBase instance = (BLLBase)Activator.CreateInstance(table.RelatedTable);
                    for (int i = 0; i <= field.ForeignFields.Length - 1; i++)
                    {
                        field.ForeignFields[i].SetPropertyValue(instance, field.LocalFields[i].PropertyValue(this));
                    }
                    instance.Bind();
                    field.SetPropertyValue(this, instance);
                    return instance;
                }

                BLLBase[] instances;
                if (lazyEnabled)
                {

                    SearchOptions sc = new SearchOptions(field.ElementType);

                    if (field.IsManyToMany)
                    {
                        Join j = new Join(GeneralDialect.ManyToManyAlias);
                        j.ForeignKey = field;
                        j.LocalTableInfo = table;
                        sc.Conditions.includes.Add(j);

                        for (int i = 0; i <= field.ForeignFields.Length - 1; i++)
                        {
                            if (i > 0)
                            {
                                sc.Conditions.Add(Tenor.Data.LogicalOperator.And);
                            }
                            SearchConditionForManyToMany scmm = new SearchConditionForManyToMany(
                                GeneralDialect.ManyToManyAlias,
                                field.LocalManyToManyFields[i],
                                field.LocalFields[i].PropertyValue(this));

                            sc.Conditions.Add(scmm);
                        }
                    }
                    else
                    {
                        //lets find objects, one-to-many and many-to-one
                        //for each Foreign, join an AND operator to match foreign with local value.
                        for (int i = 0; i <= field.ForeignFields.Length - 1; i++)
                        {
                            if (i > 0)
                            {
                                sc.Conditions.Add(Tenor.Data.LogicalOperator.And);
                            }
                            sc.Conditions.Add(
                                /* the foreign property name */
                                field.ForeignFields[i].RelatedProperty.Name,
                                /* the local value */
                                field.LocalFields[i].PropertyValue(this));

                        }
                        if (sc.Conditions.Count == 0)
                        {
                            //this should never happen.
                            throw (new TenorException());
                        }
                    }

                    //lazy is enabled, go database, go!
                    instances = Search(sc, connection);
                }
                else
                {
                    //lazy is disabled, so, no data will be retrieved.
                    instances = new BLLBase[] { };
                }


                if (field.IsArray)
                {
                    if (field.RelatedProperty.PropertyType.IsArray)
                    {
                        propertyData.Add(property.Name, instances);
                    }
                    else
                    {
                        //It must have another way to create it, string is not cool.
                        Type listof = Type.GetType("Tenor.BLL.BLLCollection`1[[" + field.ElementType.AssemblyQualifiedName + "]]");
                        System.Reflection.ConstructorInfo ctor = listof.GetConstructor(System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, new Type[] { typeof(BLLBase), typeof(string) }, null);
                        IList obj = (IList)ctor.Invoke(new object[] { (BLLBase)this, property.Name });
                        obj.Clear();
                        foreach (BLLBase i in instances)
                        {
                            obj.Add(i);
                        }
                        propertyData.Add(property.Name, obj);
                    }
                }
                else
                {
                    if (instances.Length == 0)
                    {
                        propertyData.Add(property.Name, null);
                    }
                    else
                    {
                        if (instances.Length > 1)
                        {
                            throw new ManyRecordsFoundException();//"LoadingForeignKey-ManyToOne: More than one instance was returned");
                        }
                        propertyData.Add(property.Name, instances[0]);
                    }
                }
            }
            return propertyData[property.Name];
        }

        private string GetCallingProperty()
        {
            System.Diagnostics.StackTrace stack = new System.Diagnostics.StackTrace();
            //2 steps here 'cause this is called from another method in Tenor logic.
            System.Reflection.MethodBase method = stack.GetFrame(2).GetMethod();

            //TODO: Check if we can use a better way to get the GET section of a property.
            if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))
            {
                string propName = method.Name.Substring(4);
                System.Reflection.PropertyInfo prop = this.GetType().GetProperty(propName);
                if (prop == null)
                {
                    throw new TenorException("You can only call this method within a Property Get of the current instance.");
                }
                return propName;
            }
            else
            {
                throw new TenorException("You can only call this method within a Property Get section.");
            }
        }


        /// <summary>
        /// Gets the object of a lazy loaded property.
        /// This call can do a database access.
        /// </summary>
        /// <returns>An object with the loaded value.</returns>
        protected object GetPropertyValue()
        {
            string propertyName = GetCallingProperty();
            return GetPropertyValue(propertyName);
        }

        /// <summary>
        /// Gets the value of a lazy tagged property.
        /// This call can do a database access.
        /// </summary>
        internal virtual object GetPropertyValue(string propertyName)
        {
            //only loads if not loaded yet.
            //TODO: provide a way to reload from database.
            if (!propertyData.ContainsKey(propertyName))
            {
                //Getting class metadata.

                TableInfo table = TableInfo.CreateTableInfo(this.GetType());

                System.Reflection.PropertyInfo fieldP = this.GetType().GetProperty(propertyName);
                if (fieldP == null)
                {
                    throw new Tenor.Data.MissingFieldException(this.GetType(), propertyName);
                }

                ForeignKeyInfo fkInfo = null;
        
                fkInfo = ForeignKeyInfo.Create(fieldP);

                if (fkInfo != null)
                {
                    // this is an FK, so, route to fk loading
                    return LoadForeign(fieldP, null);
                }
                else
                {
                    //Continue to the lazy property (lazy fields like byte[]) loading
                    FieldInfo field = FieldInfo.Create(fieldP);
                    if (field == null)
                        throw new Tenor.Data.MissingFieldException(this.GetType(), fieldP.Name);

                    GeneralDialect dialect = DialectFactory.CreateDialect(table.GetConnection());

                    ConditionCollection conditions = new ConditionCollection();

                    foreach (FieldInfo f in GetFields(this.GetType(), true))
                    {
                        conditions.Add(f.RelatedProperty.Name, f.PropertyValue(this));
                    }
                    if (conditions.Count == 0)
                    {
                        throw (new Tenor.Data.MissingPrimaryKeyException(this.GetType()));
                    }

                    TenorParameter[] parameters = null;
                    string fieldsPart = dialect.CreateSelectSql(table.RelatedTable, null, new FieldInfo[] { field }, null);
                    string wherePart = dialect.CreateWhereSql(conditions, table.RelatedTable, null, out parameters);
                    string sql = dialect.CreateFullSql(table.RelatedTable,
                                                false, false,
                                                0, fieldsPart, null, null, wherePart);


                    Tenor.Data.DataTable rs = new Tenor.Data.DataTable(sql, parameters, table.GetConnection());

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
            }
            return propertyData[propertyName];
        }

        /// <summary>
        /// Sets the value of a lazy tagged property.
        /// </summary>
        /// <param name="value">The desired value from the property set section.</param>
        protected virtual void SetPropertyValue(object value)
        {
            string propertyName = GetCallingProperty();
            SetPropertyValue(propertyName, value);
        }

        internal virtual void SetPropertyValue(string propertyName, object value)
        {
            //Just to check
            System.Reflection.PropertyInfo fieldP = this.GetType().GetProperty(propertyName);
            if (fieldP == null)
            {
                throw (new ArgumentException("The property specified was not found.", "propertyName", null));
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

        /*
        private bool _IsLazyDisabled = false;
        /// <summary>
        /// Returns a value indicating whether or not this class is serializing.
        /// </summary>
        /// <returns>True or False</returns>
        protected virtual bool LazyLoadingDisabled
        {
            get
            {
                return _IsLazyDisabled;
            }
        }


        /// <summary>
        /// Disables the lazy loading of the desired instances.
        /// </summary>
        /// <param name="items">An array of entities.</param>
        public static void DisableLazyLoading(BLLBase[] items)
        {
            foreach (BLLBase i in items)
            {
                i.ChangeLazyLoadingStatus(true);
            }
        }

        /// <summary>
        /// Enables the lazy loading of the desired instances.
        /// </summary>
        /// <param name="items">An array of entities.</param>
        public static void EnableLazyLoading(BLLBase[] items)
        {
            foreach (BLLBase i in items)
            {
                i.ChangeLazyLoadingStatus(false);
            }
        }

        /// <summary>
        /// Changes the current lazy loading flag.
        /// </summary>
        /// <param name="status">True to enable the lazy loading feature.</param>
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
         */

        /// <summary>
        /// Lazy starts disabled.
        /// </summary>
        private bool lazyEnabled = false;
        /// <summary>
        /// Enables or disables lazy loading feature. 
        /// <para>New instances will have lazy disabled by default, but when <see cref="Bind"/> is called, lazy feature will be enabled.</para>
        /// <para>You can change lazy status whenever you want.</para>
        /// </summary>
        /// <param name="status">The status of lazy feature, if True, lazy will be enabled, otherwise, it will be disabled.</param>
        public void EnableLazyLoading(bool status)
        {
            lazyEnabled = status;
            //TODO: check if its valueable to remove null cache if properties were called with lazy disabled (when status == true).
        }


        /// <summary>
        /// Clear the lazy cache off all properties in order to retrieve database values again.
        /// </summary>
        public void ResetLazyProperties()
        {
            propertyData.Clear();
        }

        /// <summary>
        /// Clear the lazy cache for a specific property in order to retrieve database value again.
        /// </summary>
        /// <param name="propertyName">The name of the lazy property.</param>
        public void ResetLazyProperty(string propertyName)
        {
            if (propertyData.ContainsKey(propertyName))
                propertyData.Remove(propertyName);
        }
    }
}
