using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;

namespace Tenor.Data
{
    /// <summary>
    /// Represents a collection of sort definitions.
    /// </summary>
    /// <example>
    /// <para>You can use this class to define sort options.</para>
    /// <code>
    /// SortingCollection sorts = new SortingCollection();
    /// sorts.Add("Name", SortOrder.Descending);
    /// </code>
    /// </example>
    public class SortingCollection : Collection<SortingCriteria>
    {
        /// <summary></summary>
        public SortingCollection()
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="SortingCollection"/> with one <see cref="SortingCriteria"/> set as Ascending.
        /// </summary>
        /// <param name="propertyName">A string with the property name of the base class.</param>
        public SortingCollection(string propertyName)
        {
            Add(propertyName);
        }

        /// <summary>
        /// Creates an instance of <see cref="SortingCollection"/> with one <see cref="SortingCriteria"/>.
        /// </summary>
        /// <param name="propertyName">A string with the property name of the base class.</param>
        /// <param name="sortOrder">One of the <see cref="SortOrder"/> values.</param>
        public SortingCollection(string propertyName, SortOrder sortOrder)
        {
            Add(propertyName, sortOrder);
        }

        /// <summary>
        /// Creates an instance of <see cref="SortingCollection"/> with one <see cref="SortingCriteria"/> set as Ascending.
        /// </summary>
        /// <param name="joinAlias">A string with the alias of a join, already defined on your query.</param>
        /// <param name="propertyName">A string with the property name of the joined class.</param>
        public SortingCollection(string joinAlias, string propertyName)
        {
            Add(joinAlias, propertyName);
        }

        /// <summary>
        /// Creates an instance of <see cref="SortingCollection"/> with one <see cref="SortingCriteria"/>.
        /// </summary>
        /// <param name="joinAlias">A string with the alias of a join, already defined on your query.</param>
        /// <param name="propertyName">A string with the property name of the joined class.</param>
        /// <param name="sortOrder">One of the <see cref="SortOrder"/> values.</param>
        public SortingCollection(string joinAlias, string propertyName, SortOrder sortOrder)
        {
            Add(joinAlias, propertyName, sortOrder);
        }

        /// <summary>
        /// Adds a <see cref="SortingCriteria"/> for a property set as Ascending. 
        /// </summary>
        /// <param name="propertyName">A string with the property name of the base class.</param>
        public void Add(string propertyName)
        {
            SortingCriteria item = new SortingCriteria(null, propertyName);
            Add(item);
        }

        /// <summary>
        /// Adds a <see cref="SortingCriteria"/> for a property set as Ascending. 
        /// </summary>
        /// <param name="propertyName">A string with the property name of the base class.</param>
        /// <param name="castType">A type to cast the field. For example, you can sort a set of integer values on a string field.</param>
        public void Add(string propertyName, Type castType)
        {
            SortingCriteria item = new SortingCriteria(null, propertyName, castType);
            Add(item);
        }

        /// <summary>
        /// Adds a <see cref="SortingCriteria"/> for a property.
        /// </summary>
        /// <param name="propertyName">A string with the property name of the base class.</param>
        /// <param name="sortOrder">One of the <see cref="SortOrder"/> values.</param>
        public void Add(string propertyName, SortOrder sortOrder)
        {
            SortingCriteria item = new SortingCriteria(null, propertyName, sortOrder);
            Add(item);
        }

        /// <summary>
        /// Adds a <see cref="SortingCriteria"/> for a property.
        /// </summary>
        /// <param name="propertyName">A string with the property name of the base class.</param>
        /// <param name="sortOrder">One of the <see cref="SortOrder"/> values.</param>
        /// <param name="castType">A type to cast the field. For example, you can sort a set of integer values on a string field.</param>
        public void Add(string propertyName, SortOrder sortOrder, Type castType)
        {
            SortingCriteria item = new SortingCriteria(null, propertyName, sortOrder, castType);
            Add(item);
        }

        /// <summary>
        /// Adds a <see cref="SortingCriteria"/> for a property set as Ascending.
        /// </summary>
        /// <param name="joinAlias">A string with the alias of a join, already defined on your query.</param>
        /// <param name="propertyName">A string with the property name of the joined class.</param>
        public void Add(string joinAlias, string propertyName)
        {
            SortingCriteria item = new SortingCriteria(joinAlias, propertyName);
            Add(item);
        }

        /// <summary>
        /// Adds a <see cref="SortingCriteria"/> for a property set as Ascending.
        /// </summary>
        /// <param name="joinAlias">A string with the alias of a join, already defined on your query.</param>
        /// <param name="propertyName">A string with the property name of the joined class.</param>
        /// <param name="castType">A type to cast the field. For example, you can sort a set of integer values on a string field.</param>
        public void Add(string joinAlias, string propertyName, Type castType)
        {
            SortingCriteria item = new SortingCriteria(joinAlias, propertyName, castType);
            Add(item);
        }

        /// <summary>
        /// Adds a <see cref="SortingCriteria"/> for a property.
        /// </summary>
        /// <param name="joinAlias">A string with the alias of a join, already defined on your query.</param>
        /// <param name="propertyName">A string with the property name of the joined class.</param>
        /// <param name="sortOrder">One of the <see cref="SortOrder"/> values.</param>
        public void Add(string joinAlias, string propertyName, SortOrder sortOrder)
        {
            SortingCriteria item = new SortingCriteria(joinAlias, propertyName, sortOrder);
            Add(item);
        }

        /// <summary>
        /// Adds a <see cref="SortingCriteria"/> for a property.
        /// </summary>
        /// <param name="joinAlias">A string with the alias of a join, already defined on your query.</param>
        /// <param name="propertyName">A string with the property name of the joined class.</param>
        /// <param name="sortOrder">One of the <see cref="SortOrder"/> values.</param>
        /// <param name="castType">A type to cast the field. For example, you can sort a set of integer values on a string field.</param>
        public void Add(string joinAlias, string propertyName, Tenor.Data.SortOrder sortOrder, Type castType)
        {
            SortingCriteria item = new SortingCriteria(joinAlias, propertyName, sortOrder, castType);
            Add(item);
        }
    }
}