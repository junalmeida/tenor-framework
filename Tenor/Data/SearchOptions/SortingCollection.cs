using System.Diagnostics;
using System;
using System.Collections;
using Microsoft.VisualBasic;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;

namespace Tenor.Data
{
    public class SortingCollection : Collection<SortingCriteria>
    {


        public SortingCollection()
        {
        }

        public SortingCollection(string propertyName)
        {
            Add(propertyName);
        }

        public SortingCollection(string propertyName, SortOrder sortOrder)
        {
            Add(propertyName, sortOrder);
        }

        public SortingCollection(string joinAlias, string propertyName)
        {
            Add(joinAlias, propertyName);
        }

        public SortingCollection(string joinAlias, string propertyName, SortOrder sortOrder)
        {
            Add(joinAlias, propertyName, sortOrder);
        }

        public void Add(string propertyName)
        {
            SortingCriteria item = new SortingCriteria(null, propertyName);
            Add(item);
        }

        public void Add(string propertyName, Type castType)
        {
            SortingCriteria item = new SortingCriteria(null, propertyName, castType);
            Add(item);
        }

        public void Add(string propertyName, SortOrder sortOrder)
        {
            SortingCriteria item = new SortingCriteria(null, propertyName, sortOrder);
            Add(item);
        }

        public void Add(string propertyName, SortOrder sortOrder, Type castType)
        {
            SortingCriteria item = new SortingCriteria(null, propertyName, sortOrder, castType);
            Add(item);
        }

        public void Add(string joinAlias, string propertyName)
        {
            SortingCriteria item = new SortingCriteria(joinAlias, propertyName);
            Add(item);
        }

        public void Add(string joinAlias, string propertyName, Type castType)
        {
            SortingCriteria item = new SortingCriteria(joinAlias, propertyName, castType);
            Add(item);
        }

        public void Add(string joinAlias, string propertyName, SortOrder sortOrder)
        {
            SortingCriteria item = new SortingCriteria(joinAlias, propertyName, sortOrder);
            Add(item);
        }

        public void Add(string joinAlias, string propertyName, Tenor.Data.SortOrder sortOrder, Type castType)
        {
            SortingCriteria item = new SortingCriteria(joinAlias, propertyName, sortOrder, castType);
            Add(item);
        }

        public new void Add(SortingCriteria item)
        {
            base.Add(item);
        }
    }

}