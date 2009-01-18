using System.Diagnostics;
using System;
using System.Collections;
using Microsoft.VisualBasic;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Tenor.Data
{

    public class ConditionCollection : Collection<object>
    {


        #region " Construtores "
        public ConditionCollection()
        {
        }

        public ConditionCollection(string @Property, object Value)
        {
            Add(@Property, Value);
        }

        public ConditionCollection(string @Property, object Value, Tenor.Data.CompareOperator CompareOperator)
        {
            Add(@Property, Value, CompareOperator);
        }

        public ConditionCollection(Type Table, string @Property, object Value)
        {
            Add(Table, @Property, Value);
        }

        public ConditionCollection(Type Table, string @Property, object Value, Tenor.Data.CompareOperator CompareOperator)
        {
            Add(Table, @Property, Value, CompareOperator);
        }
        #endregion

        #region " Add "
        /// <summary>
        /// Adiciona um operador lógico entre condições ou grupos de condições.
        /// </summary>
        /// <param name="LogicalOperator"></param>
        /// <remarks></remarks>
        public void Add(LogicalOperator LogicalOperator)
        {
            if (this.Items.Count == 0)
            {
                throw (new ArgumentException("Cannot insert a operator when the collection is empty.", "LogicalOperator", null));
            }
            else if (this[this.Items.Count - 1].GetType() == typeof(Data.LogicalOperator))
            {
                throw (new ArgumentException("Cannot insert a operator when the last item is another operator.", "LogicalOperator", null));
            }
            base.Add(LogicalOperator);
        }

        /// <summary>
        /// Adiciona uma condição de igualdade entre propriedade e valor.
        /// </summary>
        /// <param name="Property"></param>
        /// <param name="Value"></param>
        /// <remarks></remarks>
        public ConditionCollection Add(string @Property, object Value)
        {
            Add(new SearchCondition(null, @Property, Value, Tenor.Data.CompareOperator.Equal));
            return this;
        }

        /// <summary>
        /// Adiciona uma condição comparativa entre propriedade e valor.
        /// </summary>
        /// <param name="Property"></param>
        /// <param name="Value"></param>
        /// <param name="CompareOperator"></param>
        /// <remarks></remarks>
        public ConditionCollection Add(string @Property, object Value, Tenor.Data.CompareOperator CompareOperator)
        {
            Add(new SearchCondition(null, @Property, Value, CompareOperator));
            return this;
        }

        /// <summary>
        /// Adiciona uma condição comparativa entre propriedade e valor
        /// </summary>
        /// <param name="Property"></param>
        /// <param name="Value"></param>
        /// <param name="CompareOperator"></param>
        /// <param name="CastType"></param>
        /// <remarks></remarks>
        public ConditionCollection Add(string @Property, object Value, Tenor.Data.CompareOperator CompareOperator, Type CastType)
        {
            Add(new SearchCondition(null, @Property, Value, CompareOperator, CastType));
            return this;
        }

        public ConditionCollection Add(string @Property, object Value, Tenor.Data.CompareOperator CompareOperator, Type CastType, string TableAlias)
        {
            Add(new SearchCondition(null, @Property, Value, CompareOperator, CastType, TableAlias));
            return this;
        }

        public ConditionCollection Add(string @Property, object Value, Type CastType)
        {
            Add(new SearchCondition(null, @Property, Value, Tenor.Data.CompareOperator.Equal, CastType));
            return this;
        }

        public ConditionCollection Add(string @Property, object Value, Type CastType, string TableAlias)
        {
            Add(new SearchCondition(null, @Property, Value, Tenor.Data.CompareOperator.Equal, CastType, TableAlias));
            return this;
        }

        public ConditionCollection Add(Type Table, string @Property, object Value)
        {
            Add(new SearchCondition(Table, @Property, Value, Tenor.Data.CompareOperator.Equal));
            return this;
        }

        public ConditionCollection Add(Type Table, string @Property, object Value, string TableAlias)
        {
            Add(new SearchCondition(Table, @Property, Value, Tenor.Data.CompareOperator.Equal, TableAlias));
            return this;
        }

        public ConditionCollection Add(Type Table, string @Property, object Value, Tenor.Data.CompareOperator CompareOperator)
        {
            Add(new SearchCondition(Table, @Property, Value, CompareOperator));
            return this;
        }

        public ConditionCollection Add(Type Table, string @Property, object Value, Tenor.Data.CompareOperator CompareOperator, string TableAlias)
        {
            Add(new SearchCondition(Table, @Property, Value, CompareOperator, TableAlias));
            return this;
        }

        public ConditionCollection Add(Type Table, string @Property, object Value, Tenor.Data.CompareOperator CompareOperator, Type CastType)
        {
            Add(new SearchCondition(Table, @Property, Value, CompareOperator, CastType));
            return this;
        }

        public ConditionCollection Add(Type Table, string @Property, object Value, Tenor.Data.CompareOperator CompareOperator, Type CastType, string TableAlias)
        {
            Add(new SearchCondition(Table, @Property, Value, CompareOperator, CastType, TableAlias));
            return this;
        }

        public ConditionCollection Add(Type Table, string @Property, object Value, Type CastType)
        {
            Add(new SearchCondition(Table, @Property, Value, Tenor.Data.CompareOperator.Equal, CastType));
            return this;
        }

        public ConditionCollection Add(Type Table, string @Property, object Value, Type CastType, string TableAlias)
        {
            Add(new SearchCondition(Table, @Property, Value, Tenor.Data.CompareOperator.Equal, CastType, TableAlias));
            return this;
        }


        /// <summary>
        /// Adiciona uma condição
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <remarks></remarks>
        public ConditionCollection Add(SearchCondition SearchCondition)
        {
            if (SearchCondition == null)
            {
                throw (new ArgumentException("Cannot insert a Null object."));
            }
            else if (Count > 0 && this[Count - 1].GetType() != typeof(Data.LogicalOperator))
            {
                throw (new ArgumentException("Cannot insert a SearchCondition when the last item is not an operator.", "SearchCondition", null));
            }

            base.Add(SearchCondition);
            return this;
        }


        /// <summary>
        /// Adiciona um grupo de condições.
        /// </summary>
        /// <param name="SearchConditions"></param>
        /// <remarks></remarks>
        public ConditionCollection Add(ConditionCollection SearchConditions)
        {
            if (SearchConditions == null || SearchConditions.Count == 0)
            {
                throw (new ArgumentException("Cannot insert Null or empty collection."));
            }
            else if (this.Items.Count > 0 && (this[this.Items.Count - 1].GetType() == typeof(SearchCondition) || this[this.Items.Count - 1].GetType() == typeof(ConditionCollection)))
            {
                throw (new ArgumentException("Cannot insert a SearchCondition when the last item is not an operator.", "SearchConditions", null));
            }
            else if (!(SearchConditions[SearchConditions.Count - 1].GetType() == typeof(SearchCondition) || SearchConditions[SearchConditions.Count - 1].GetType() == typeof(ConditionCollection)))
            {
                throw (new ArgumentException("Invalid ConditionCollecion. Collections cannot end with an operator.", "SearchConditions", null));
            }
            base.Add(SearchConditions);

            return this;
        }

        #endregion

        #region " AND "
        /// <summary>
        /// Adiciona uma condição de igualdade entre propriedade e valor.
        /// </summary>
        /// <param name="Property"></param>
        /// <param name="Value"></param>
        /// <remarks></remarks>
        public ConditionCollection @And(string @Property, object Value)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(@Property, Value);
        }

        /// <summary>
        /// Adiciona uma condição comparativa entre propriedade e valor.
        /// </summary>
        /// <param name="Property"></param>
        /// <param name="Value"></param>
        /// <param name="CompareOperator"></param>
        /// <remarks></remarks>
        public ConditionCollection @And(string @Property, object Value, Tenor.Data.CompareOperator CompareOperator)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(@Property, Value, CompareOperator);

        }

        /// <summary>
        /// Adiciona uma condição comparativa entre propriedade e valor
        /// </summary>
        /// <param name="Property"></param>
        /// <param name="Value"></param>
        /// <param name="CompareOperator"></param>
        /// <param name="CastType"></param>
        /// <remarks></remarks>
        public ConditionCollection @And(string @Property, object Value, Tenor.Data.CompareOperator CompareOperator, Type CastType)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(@Property, Value, CompareOperator, CastType);
        }

        public ConditionCollection @And(string @Property, object Value, Tenor.Data.CompareOperator CompareOperator, Type CastType, string TableAlias)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(@Property, Value, CompareOperator, CastType, TableAlias);
        }

        public ConditionCollection @And(string @Property, object Value, Type CastType)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(@Property, Value, CastType);
        }

        public ConditionCollection @And(string @Property, object Value, Type CastType, string TableAlias)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(@Property, Value, CastType, TableAlias);
        }

        public ConditionCollection @And(Type Table, string @Property, object Value)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(Table, @Property, Value);
        }

        public ConditionCollection @And(Type Table, string @Property, object Value, string TableAlias)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            this.Add(Table, @Property, Value, TableAlias);
            return this;
        }

        public ConditionCollection @And(Type Table, string @Property, object Value, Tenor.Data.CompareOperator CompareOperator)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(Table, @Property, Value, CompareOperator);
        }

        public ConditionCollection @And(Type Table, string @Property, object Value, Tenor.Data.CompareOperator CompareOperator, string TableAlias)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(Table, @Property, Value, CompareOperator, TableAlias);
        }

        public ConditionCollection @And(Type Table, string @Property, object Value, Tenor.Data.CompareOperator CompareOperator, Type CastType)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(Table, @Property, Value, CompareOperator, CastType);
        }

        public ConditionCollection @And(Type Table, string @Property, object Value, Tenor.Data.CompareOperator CompareOperator, Type CastType, string TableAlias)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(Table, @Property, Value, CompareOperator, CastType, TableAlias);
        }

        public ConditionCollection @And(Type Table, string @Property, object Value, Type CastType)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(Table, @Property, Value, CastType);
        }

        public ConditionCollection @And(Type Table, string @Property, object Value, Type CastType, string TableAlias)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(Table, @Property, Value, CastType, TableAlias);
        }

        public ConditionCollection @And(SearchCondition SearchCondition)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(SearchCondition);
        }

        public ConditionCollection @And(ConditionCollection SearchConditions)
        {
            this.Add(Tenor.Data.LogicalOperator.And);
            return this.Add(SearchConditions);
        }

        #endregion

        #region " Or "


        /// <summary>
        /// Adiciona uma condição de igualdade entre propriedade e valor.
        /// </summary>
        /// <param name="Property"></param>
        /// <param name="Value"></param>
        /// <remarks></remarks>
        public ConditionCollection @Or(string @Property, object Value)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(@Property, Value);
        }

        /// <summary>
        /// Adiciona uma condição comparativa entre propriedade e valor.
        /// </summary>
        /// <param name="Property"></param>
        /// <param name="Value"></param>
        /// <param name="CompareOperator"></param>
        /// <remarks></remarks>
        public ConditionCollection @Or(string @Property, object Value, Tenor.Data.CompareOperator CompareOperator)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(@Property, Value, CompareOperator);
        }

        /// <summary>
        /// Adiciona uma condição comparativa entre propriedade e valor
        /// </summary>
        /// <param name="Property"></param>
        /// <param name="Value"></param>
        /// <param name="CompareOperator"></param>
        /// <param name="CastType"></param>
        /// <remarks></remarks>
        public ConditionCollection @Or(string @Property, object Value, Tenor.Data.CompareOperator CompareOperator, Type CastType)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(@Property, Value, CompareOperator, CastType);
        }

        public ConditionCollection @Or(string @Property, object Value, Tenor.Data.CompareOperator CompareOperator, Type CastType, string TableAlias)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(@Property, Value, CompareOperator, CastType, TableAlias);
        }

        public ConditionCollection @Or(string @Property, object Value, Type CastType)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(@Property, Value, CastType);
        }

        public ConditionCollection @Or(string @Property, object Value, Type CastType, string TableAlias)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(@Property, Value, CastType, TableAlias);
        }

        public ConditionCollection @Or(Type Table, string @Property, object Value)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(Table, @Property, Value);
        }

        public ConditionCollection @Or(Type Table, string @Property, object Value, string TableAlias)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(Table, @Property, Value, TableAlias);
        }

        public ConditionCollection @Or(Type Table, string @Property, object Value, Tenor.Data.CompareOperator CompareOperator)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(Table, @Property, Value, CompareOperator);
        }

        public ConditionCollection @Or(Type Table, string @Property, object Value, Tenor.Data.CompareOperator CompareOperator, string TableAlias)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(Table, @Property, Value, CompareOperator, TableAlias);
        }

        public ConditionCollection @Or(Type Table, string @Property, object Value, Tenor.Data.CompareOperator CompareOperator, Type CastType)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(Table, @Property, Value, CompareOperator, CastType);
        }

        public ConditionCollection @Or(Type Table, string @Property, object Value, Tenor.Data.CompareOperator CompareOperator, Type CastType, string TableAlias)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(Table, @Property, Value, CompareOperator, CastType, TableAlias);
        }

        public ConditionCollection @Or(Type Table, string @Property, object Value, Type CastType)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(Table, @Property, Value, CastType);
        }

        public ConditionCollection @Or(Type Table, string @Property, object Value, Type CastType, string TableAlias)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(Table, @Property, Value, CastType, TableAlias);
        }

        public ConditionCollection @Or(SearchCondition SearchCondition)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(SearchCondition);
        }

        public ConditionCollection @Or(ConditionCollection SearchConditions)
        {
            this.Add(Tenor.Data.LogicalOperator.Or);
            return this.Add(SearchConditions);
        }
        #endregion

        #region " Contains e IndexOf "
        public bool Contains(ConditionCollection SearchConditions)
        {
            return base.Contains(SearchConditions);
        }

        public bool Contains(SearchCondition SearchCondition)
        {
            return base.Contains(SearchCondition);
        }

        public bool Contains(Data.LogicalOperator LogicalOperator)
        {
            return base.Contains(LogicalOperator);
        }

        public int IndexOf(ConditionCollection SearchConditions)
        {
            return base.IndexOf(SearchConditions);
        }

        public int IndexOf(SearchCondition SearchCondition)
        {
            return base.IndexOf(SearchCondition);
        }

        public int IndexOf(Data.LogicalOperator LogicalOperator)
        {
            return base.IndexOf(LogicalOperator);
        }

        #endregion

        #region " Include Table "
        internal Dictionary<string, Type> Includes = new Dictionary<string, Type>();

        /// <summary>
        /// Inclue uma classe nos relacionamentos
        /// </summary>
        /// <param name="Table">Tipo da classe desejada. Deve herdar de BLLBase direta ou indiretamente.</param>
        /// <remarks></remarks>
        public void Include(Type Table)
        {
            if (Table == null)
            {
                throw (new ArgumentNullException("Table"));
            }
            Include(Table, Table.Name);
        }

        /// <summary>
        /// Inclue uma classe nos relacionamentos
        /// </summary>
        /// <param name="Table">Tipo da classe desejada. Deve herdar de BLLBase direta ou indiretamente.</param>
        /// <param name="Alias">Alias da classe</param>
        /// <remarks></remarks>
        public void Include(Type Table, string @Alias)
        {
            if (Table == null)
            {
                throw (new ArgumentNullException("Table"));
            }
            Includes.Add(@Alias, Table);
        }
        #endregion


        public new object this[int index]
        {
            get
            {
                return base[index];
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public void Insert()
        {
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public void Remove()
        {
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public void RemoveAt()
        {
        }
    }
}
