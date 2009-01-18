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

        public SortingCollection(string @Property)
        {
            Add(@Property);
        }

        public SortingCollection(string @Property, Tenor.Data.SortOrder SortOrder)
        {
            Add(@Property, SortOrder);
        }

        public SortingCollection(Type Table, string @Property)
        {
            Add(Table, @Property);
        }

        public SortingCollection(Type Table, string @Property, Tenor.Data.SortOrder SortOrder)
        {
            Add(Table, @Property, SortOrder);
        }

        public void Add(string @Property)
        {
            SortingCriteria item = new SortingCriteria(null, @Property);
            Add(item);
        }

        public void Add(string @Property, Type CastType)
        {
            SortingCriteria item = new SortingCriteria(null, @Property, CastType);
            Add(item);
        }

        public void Add(string @Property, Tenor.Data.SortOrder SortOrder)
        {
            SortingCriteria item = new SortingCriteria(null, @Property, SortOrder);
            Add(item);
        }

        public void Add(string @Property, Tenor.Data.SortOrder SortOrder, Type CastType)
        {
            SortingCriteria item = new SortingCriteria(null, @Property, SortOrder, CastType);
            Add(item);
        }

        public void Add(Type Table, string @Property)
        {
            SortingCriteria item = new SortingCriteria(Table, @Property);
            Add(item);
        }

        public void Add(Type Table, string @Property, Type CastType)
        {
            SortingCriteria item = new SortingCriteria(Table, @Property, CastType);
            Add(item);
        }

        public void Add(Type Table, string @Property, Tenor.Data.SortOrder SortOrder)
        {
            SortingCriteria item = new SortingCriteria(Table, @Property, SortOrder);
            Add(item);
        }

        public void Add(Type Table, string @Property, Tenor.Data.SortOrder SortOrder, Type CastType)
        {
            SortingCriteria item = new SortingCriteria(Table, @Property, SortOrder, CastType);
            Add(item);
        }

        public new void Add(SortingCriteria item)
        {
            base.Add(item);
        }
    }

}