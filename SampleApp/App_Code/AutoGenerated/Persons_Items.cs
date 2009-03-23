﻿
using System.Collections.Generic;
using System.Configuration;
using System;

using Tenor.BLL;
using Tenor.Data;

namespace SampleApp.Business.Entities
{
    /// <summary>
    /// Represents the table Persons_Items.
    /// Some descrition of this relation.
    /// </summary>
    [Serializable(), Table("PersonItem", "")]
    public partial class PersonItem : BLLBase
    {

        #region Properties


        private int _ItemId;
        /// <summary>
        /// Represents the field ItemId.
        /// 
        /// </summary>
        [Field(PrimaryKey = true)]
        public int ItemId
        {
            get
            {
                return _ItemId;
            }
            set
            {
                _ItemId = value;
            }
        }


        private int _PersonId;
        /// <summary>
        /// Represents the field PersonId.
        /// 
        /// </summary>
        [Field(PrimaryKey = true)]
        public int PersonId
        {
            get
            {
                return _PersonId;
            }
            set
            {
                _PersonId = value;
            }
        }


        /// <summary>
        /// Keeps a list of constants with property names.
        /// </summary>
        public partial class Properties : object
        {
            private Properties() { }

            public const string ItemId = "ItemId";


            public const string PersonId = "PersonId";


        }


        #endregion
        #region Foreign Keys


        /// <summary>
        /// Represents the relationship FK_Persons_Items_Items.
        /// </summary>
        [ForeignKey(Item.Properties.ItemId, Properties.ItemId)]
        public Item Item
        {
            get
            {
                return (Item)GetPropertyValue();
            }
            set
            {
                SetPropertyValue(value);
                if (value == null)
                    ItemId = 0;
                else
                    ItemId = value.ItemId;
            }
        }


        /// <summary>
        /// Represents the relationship FK_Persons_Items_Persons.
        /// </summary>
        [ForeignKey(Person.Properties.PersonId, Properties.PersonId)]
        public Person Person
        {
            get
            {
                return (Person)GetPropertyValue();
            }
            set
            {
                SetPropertyValue(value);
                if (value == null)
                    PersonId = 0;
                else
                    PersonId = value.PersonId;
            }
        }


        #endregion
        #region Constructors And Metadata


        public PersonItem()
        { }


        public PersonItem(bool lazyLoadingDisabled) :
            base(lazyLoadingDisabled)
        { }


        /// <summary>
        /// Loads Persons_Items from the database with these keys.
        /// </summary><%
        public PersonItem(int pItemId, int pPersonId) :
            base()
        {
            this.ItemId = pItemId;
            this.PersonId = pPersonId;

            Bind();
        }


        #endregion
        #region Search



        public static PersonItem[] Search(ConditionCollection conditions, SortingCollection sorting)
        {
            return Search(conditions, sorting, false);
        }

        public static PersonItem[] Search(ConditionCollection conditions, SortingCollection sorting, bool distinct)
        {
            return Search(conditions, sorting, distinct, 0);
        }

        public static PersonItem[] Search(ConditionCollection conditions, SortingCollection sorting, bool distinct, int limit)
        {
            return Search(conditions, sorting, distinct, limit, null);
        }

        /// <summary>
        /// Performs a search within this class.
        /// </summary>
        public static PersonItem[] Search(ConditionCollection conditions, SortingCollection sorting, bool distinct, int limit, ConnectionStringSettings connection)
        {
            SearchOptions sc = new SearchOptions(typeof(PersonItem));
            if (conditions != null)
                sc.Conditions = conditions;
            if (sorting != null)
                sc.Sorting = sorting;

            sc.Distinct = distinct;
            sc.Top = limit;

            return (PersonItem[])(BLLBase.Search(sc, connection));
        }


        #endregion
    }
}

