using System.Diagnostics;
using System;
using System.Collections;
using Microsoft.VisualBasic;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Configuration;


namespace Tenor.Data
{
    public class SortingCriteria
    {


        /// <summary>
        ///
        /// </summary>
        /// <param name="Table"></param>
        /// <param name="Property"></param>
        /// <remarks></remarks>
        public SortingCriteria(string joinAlias, string propertyName, Type castType) :
            this(joinAlias, propertyName, SortOrder.Ascending, castType)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="Table"></param>
        /// <param name="Property"></param>
        /// <remarks></remarks>
        public SortingCriteria(string joinAlias, string propertyName) :
            this(joinAlias, propertyName, SortOrder.Ascending)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="Table"></param>
        /// <param name="Property"></param>
        /// <param name="SortOrder"></param>
        /// <remarks></remarks>
        public SortingCriteria(string joinAlias, string propertyName, SortOrder sortOrder) :
            this(joinAlias, propertyName, sortOrder, null)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="Table"></param>
        /// <param name="Property"></param>
        /// <param name="SortOrder"></param>
        /// <param name="castType"></param>
        /// <remarks></remarks>
        public SortingCriteria(string joinAlias, string propertyName, SortOrder sortOrder, Type castType)
        {
            _joinAlias = joinAlias;

            if (_joinAlias != null)
            {
                if (_joinAlias.Contains("\"") || _joinAlias.Contains("\'") || _joinAlias.Contains("\r") || _joinAlias.Contains("\n") || _joinAlias.Contains(" "))
                {
                    throw (new ArgumentException("Invalid characters on the alias.", "joinAlias"));
                }
            }

            _propertyName = propertyName;
            _castType = castType;
            _sortOrder = sortOrder;
        }


        private string _joinAlias;
        public string JoinAlias
        {
            get
            {
                return _joinAlias;
            }
        }


        private string _propertyName;
        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
        }

        private Type _castType;
        public Type CastType
        {
            get
            {
                return _castType;
            }
        }

        private SortOrder _sortOrder;
        public SortOrder SortOrder
        {
            get
            {
                return _sortOrder;
            }
        }
    }
}