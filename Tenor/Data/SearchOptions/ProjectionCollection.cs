using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Tenor.Data
{
    /// <summary>
    /// This class provides a list of projections used on this query.
    /// </summary>
    public class ProjectionCollection : IEnumerable
    {


        #region " Constructors "
        /// <summary>
        /// Creates an instance of ProjectionCollection.
        /// </summary>
        internal ProjectionCollection()
        {
        }

        ///// <summary>
        ///// Creates an instance with one equality filter. 
        ///// </summary>
        ///// <param name="property">The property name of the base class.</param>
        //internal ProjectionCollection(string property)
        //{
        //    Add(property, null);
        //}

        ///// <summary>
        ///// Creates and instance with one projection.
        ///// </summary>
        ///// <param name="property">The property name of the class.</param>
        ///// <param name="alias">The alias of joint class.</param>
        //internal ProjectionCollection(string property, string alias)
        //{
        //    Add(property, alias);
        //}

        #endregion

        #region " Add "

        /// <summary>
        /// Adds a projection to this query.
        /// </summary>
        /// <param name="property">The property name of the base class.</param>
        /// <returns>This instance.</returns>
        public ProjectionCollection Add(string propertyName)
        {
            return Add(propertyName, null);
        }

        /// <summary>
        /// Adds a projection to this query.
        /// </summary>
        /// <param name="property">The property name of the class.</param>
        /// <param name="alias">The alias of joint class.</param>
        /// <returns>This instance.</returns>
        public ProjectionCollection Add(string propertyName, string alias)
        {
            return Add(new Projection(propertyName, alias));
        }




        /// <summary>
        /// Adds a projection to this query.
        /// </summary>
        /// <param name="projection">A Projection with field information.</param>
        /// <returns>This instance.</returns>
        public ProjectionCollection Add(Projection projection)
        {
            if (projection == null)
            {
                throw (new ArgumentNullException("projection"));
            }

            BaseAdd(projection);
            return this;
        }

        private void BaseAdd(Projection item)
        {
            Items.Add(item);
        }

        #endregion

        #region " Contains, IndexOf e Count "
        public bool Contains(Projection projection)
        {
            return Items.Contains(projection);
        }

        public int IndexOf(Projection projection)
        {
            return Items.IndexOf(projection);
        }

        public int Count
        {
            get { return Items.Count; }
        }

        #endregion

        List<Projection> Items = new List<Projection>();

        public Projection this[int index]
        {
            get
            {
                return Items[index];
            }
        }

        public IEnumerator GetEnumerator()
        {
            return Items.GetEnumerator();
        }

    }
}
