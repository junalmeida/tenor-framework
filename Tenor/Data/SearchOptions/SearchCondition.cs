using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Configuration;
using Tenor.BLL;


namespace Tenor.Data
{
    /// <summary>
    /// Represents a filter condition.
    /// </summary>
    public class SearchCondition
    {
        public SearchCondition(string joinAlias, string propertyName, object value)
            :
            this(joinAlias, propertyName, value, CompareOperator.Equal)
        {
        }
        
        public SearchCondition(string joinAlias, string propertyName, object value, CompareOperator compareOperator)
            :
            this(joinAlias, propertyName, value, compareOperator, ((Type)null))
        {
        }
        
        public SearchCondition(string joinAlias, string propertyName, object value, CompareOperator compareOperator, Type castType)
        {
            _joinAlias = joinAlias;
            _propertyName = propertyName;


            if (_joinAlias != null)
            {
                if (_joinAlias.Contains("\"") || _joinAlias.Contains("\'") || _joinAlias.Contains("\r") || _joinAlias.Contains("\n") || _joinAlias.Contains(" "))
                {
                    throw (new ArgumentException("Invalid characters on the alias.", "joinAlias"));
                }
            }


            if (castType == null)
            {
                _value = value;
            }
            else
            {
                _value = Convert.ChangeType(value, castType);
            }

            /*_value = CType(value, casttype)*/
            _compareOperator = compareOperator;
            _castType = CastType;
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

        private object _value;
        public object Value
        {
            get
            {
                return _value;
            }
        }

        private Data.CompareOperator _compareOperator = Data.CompareOperator.Equal;
        public Data.CompareOperator CompareOperator
        {
            get
            {
                return _compareOperator;
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
    }

    /*
    internal class SearchConditionForManyToMany : SearchCondition
    {
        internal string localPropertyName;
        internal string foreignField;
        internal string localField;

        internal SearchConditionForManyToMany(string alias, string foreignPropertyName, string localPropertyName, string foreignField, string localField, object value)
            : base(alias, foreignPropertyName, value)
        {
            this.localPropertyName = localPropertyName;
            this.localField = localField;
            this.foreignField = foreignField;
        }
    }
     */
    internal class SearchConditionForManyToMany : SearchCondition
    {
        internal string localField;

        internal SearchConditionForManyToMany(string alias, string localField, object value)
            : base(alias, null, value)
        {
            this.localField = localField;
        }
    }

}