using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Common;


namespace Tenor.Data
{

    public class TenorParameter /*: System.Data.Common.DbParameter*/
    {



        private string _name;
        private object _value;


        /// <summary>
        /// Creates an instance of a parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">Some value.</param>
        /// <remarks></remarks>
        public TenorParameter(string name, object value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            _name = name;
            _value = value;

        }

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
        internal DbParameter ToDbParameter(DbProviderFactory factory)
        {
            DbParameter dbparam = factory.CreateParameter();

            dbparam.ParameterName = ParameterName;
            
            if (Value is Enum)
                Value = Convert.ToInt64(Value);

            dbparam.Value = Value;

            return dbparam;
        }
    }
}