using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Configuration;


namespace Tenor.Data
{
    /// <summary>
    /// Represents definitions on sorting results of a search.
    /// </summary>
    public class SortingCriteria
    {

        public SortingCriteria(string joinAlias, string propertyName, Type castType) :
            this(joinAlias, propertyName, SortOrder.Ascending, castType)
        {
        }



        public SortingCriteria(string joinAlias, string propertyName) :
            this(joinAlias, propertyName, SortOrder.Ascending)
        {
        }


        public SortingCriteria(string joinAlias, string propertyName, SortOrder sortOrder) :
            this(joinAlias, propertyName, sortOrder, null)
        {
        }


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