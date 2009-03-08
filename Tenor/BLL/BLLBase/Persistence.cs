using System;
using System.Collections.Generic;
using System.Text;
using Tenor.Data;
using System.Data;
using System.Configuration;
using System.Data.Common;

namespace Tenor.BLL
{
    public abstract partial class BLLBase
    {


        /// <summary>
        /// Binds this entity instance to database data using primary keys current data.
        /// </summary>
        /// <remarks>Faz a consulta com LazyLoading</remarks>
        public void Bind()
        {
            Bind(true);
        }

        
        /// <summary>
        /// Binds this entity instance to database data using primary keys current data.
        /// </summary>
        /// <param name="LazyLoading">Defines weather lazy loading is enabled.</param>
        /// <exception cref="MissingFieldsException">Ocurrs when no property with a FieldAttribute is defined.</exception>
        /// <exception cref="System.Data.MissingPrimaryKeyException">Occurs when no FieldAttribute with primary key is defined.</exception>
        /// <remarks></remarks>
        protected void Bind(bool lazyLoading)
        {
            FieldInfo[] fields = GetFields(this.GetType(), true);


            if (fields.Length == 0)
            {
                throw (new System.Data.MissingPrimaryKeyException());
            }

            List<string> propertyNames = new List<string>();
            foreach (FieldInfo field in fields)
            {
                propertyNames.Add(field.RelatedProperty.Name);
            }

            Bind(lazyLoading, propertyNames.ToArray());
        }

        /// <summary>
        /// Binds this entity instance to database data using current data of <paramref name="filterMembers">Filter Members</paramref>.
        /// </summary>
        /// <param name="LazyLoading">Defines weather lazy loading is enabled.</param>
        /// <param name="FilterMembers">Property members used to filter data.</param>
        /// <remarks></remarks>
        protected void Bind(bool lazyLoading, string[] filterMembers)
        {
            Bind(lazyLoading, filterMembers, null);
        }



        /// <summary>
        /// Binds this entity instance to database data using current data of <paramref name="filterMembers">Filter Members</paramref>.
        /// </summary>
        /// <param name="LazyLoading">Defines weather lazy loading is enabled.</param>
        /// <param name="FilterMembers">Property members used to filter data.</param>
        /// <param name="DataRow">A System.Data.DataRow to bind this instance. When this parameter is not set, Tenor return data from the database.</param>
        protected virtual void Bind(bool LazyLoading, string[] FilterMembers, DataRow dataRow)
        {
            bool fromSearch = (dataRow != null);
            //dataRow is null when Bind is called from LoadForeing or directly from user code.
            //dataRow is not null when Bind is called from Search (see Search.cs)

            if (!fromSearch && ClassMetadata.Cacheable && LoadFromCache())
            {
                //LoadFromCache returns true, the Bind was done.
                //If not cacheable, never LoadFromCache (see Cache.cs)
                return;
            }


            TableInfo table = TableInfo.CreateTableInfo(this.GetType());
            ConnectionStringSettings connection = table.GetConnection();
            if (!fromSearch)
            {

                //Retrieve data

                SearchOptions so = new SearchOptions(this.GetType());



                // SELECT e FILTROS

                List<FieldInfo> filters = new List<FieldInfo>();
                foreach (string s in FilterMembers)
                {
                    FieldInfo field = FieldInfo.Create(this.GetType().GetProperty(s));
                    if (field == null)
                        throw new Tenor.Data.MissingFieldException(this.GetType(), s);
                    filters.Add(field);
                }


                foreach (FieldInfo field in filters)
                {
                    if (so.Conditions.Count > 0)
                    {
                        so.Conditions.Add(Tenor.Data.LogicalOperator.And);
                    }
                    so.Conditions.Add(field.RelatedProperty.Name, field.PropertyValue(this));
                }


                System.Data.DataTable dt = SearchWithDataTable(so, connection);

                if (dt.Rows.Count == 0)
                {
                    dt.Dispose();
                    throw (new RecordNotFoundException());
                }
                else if (dt.Rows.Count > 1)
                {
                    throw new ManyRecordsFoundException();
                }

                dataRow = dt.Rows[0];

            }




            FieldInfo[] fields = GetFields(this.GetType());
            SpecialFieldInfo[] spfields = GetSpecialFields(this.GetType());
            ForeignKeyInfo[] foreignkeys = GetForeignKeys(this.GetType());

            foreach (FieldInfo f in fields)
            {
                if (f.PrimaryKey || !f.LazyLoading)
                {
                    f.SetPropertyValue(this, dataRow[f.DataFieldName]);
                }
            }
            foreach (SpecialFieldInfo f in spfields)
            {
                f.SetPropertyValue(this, dataRow[f.Alias]);
            }


            dataRow = null;


            if (!LazyLoading)
            {
                foreach (ForeignKeyInfo f in foreignkeys)
                {
                    //Continuar em LazyLoading para evitar loops infinitos.
                    //TODO: Try to bind lazy properties at the same database round
                    this.LoadForeign(f.RelatedProperty.Name, true, null, connection);
                }
            }


            if (ClassMetadata.Cacheable)
            {
                SaveToCache();
            }
        }


    }
}
