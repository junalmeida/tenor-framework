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


namespace Tenor.Data
{

    public class TenorParameter //: System.Data.Common.DbParameter
    {



        private string _name;
        private object _value;


        /// <summary>
        /// Cria uma instancia de parametro com Nome e Valor
        /// </summary>
        /// <param name="Name">Nome do parametro. Ex. CPF ou @CPF.</param>
        /// <param name="Value">Valor do parametro</param>
        /// <remarks></remarks>
        public TenorParameter(string name, object value)
        {
            _name = name;
            _value = value;
        }





        //internal string DbTypeName
        //{
        //    get
        //    {
        //        SqlParameter sql = root as SqlParameter;
        //        string res;
        //        if (sql != null)
        //        {
        //            res = sql.SqlDbType.ToString().ToLower();
        //            if (res.Contains("varchar"))
        //            {
        //                //Compatível somente com SQL 2005 +
        //                res += "(MAX)";
        //            }
        //        }
        //        else
        //        {
        //            res = root.DbType.ToString().ToLower();
        //        }


        //        return res;
        //    }
        //}


        //public override System.Data.DbType DbType
        //{
        //    get
        //    {
        //        return root.DbType;
        //    }
        //    set
        //    {
        //        root.DbType = value;
        //    }
        //}

        //public override System.Data.ParameterDirection Direction
        //{
        //    get
        //    {
        //        return root.Direction;
        //    }
        //    set
        //    {
        //        root.Direction = value;
        //    }
        //}

        //public override bool IsNullable
        //{
        //    get
        //    {
        //        return root.IsNullable;
        //    }
        //    set
        //    {
        //        root.IsNullable = value;
        //    }
        //}

        //public override void ResetDbType()
        //{
        //    root.ResetDbType();
        //}

        //public override int Size
        //{
        //    get
        //    {
        //        return root.Size;
        //    }
        //    set
        //    {
        //        root.Size = value;
        //    }
        //}

        //public override string SourceColumn
        //{
        //    get
        //    {
        //        return root.SourceColumn;
        //    }
        //    set
        //    {
        //        root.SourceColumn = value;
        //    }
        //}

        //public override bool SourceColumnNullMapping
        //{
        //    get
        //    {
        //        return root.SourceColumnNullMapping;
        //    }
        //    set
        //    {
        //        root.SourceColumnNullMapping = value;
        //    }
        //}

        //public override System.Data.DataRowVersion SourceVersion
        //{
        //    get
        //    {
        //        return root.SourceVersion;
        //    }
        //    set
        //    {
        //        root.SourceVersion = value;
        //    }
        //}



        public string ParameterName
        {
            get
            {
                return _name;
            }
        }

        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        /// <summary>
        /// Converts a TenorParameter to its equivalent.
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        internal DbParameter ToDbParameter(DbProviderFactory factory)
        {
            DbParameter dbparam = factory.CreateParameter();

            dbparam.ParameterName = ParameterName;
            dbparam.Value = Value;

            return dbparam;
        }
    }
}