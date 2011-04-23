
using System;
using System.Configuration;
using Tenor.Data;

namespace SampleApp.Business.Entities
{
    /// <summary>
    /// Represents the table Persons_Items.
    /// Some descrition of this relation.
    /// </summary>
    [Serializable(), Table("PersonItem", "")]
    public partial class PersonItem : EntityBase
    {

        #region Properties


        private long _ItemId;
        /// <summary>
        /// Represents the field ItemId.
        /// 
        /// </summary>
        [Field(PrimaryKey = true)]
        public long ItemId
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


        private long _PersonId;
        /// <summary>
        /// Represents the field PersonId.
        /// 
        /// </summary>
        [Field(PrimaryKey = true)]
        public long PersonId
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
        [ForeignKeyField(Item.Properties.ItemId, Properties.ItemId)]
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
        [ForeignKeyField(Person.Properties.PersonId, Properties.PersonId)]
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



        /// <summary>
        /// Loads Persons_Items from the database with these keys.
        /// </summary><%
        public PersonItem(long pItemId, long pPersonId)
            :
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

            return (PersonItem[])(EntityBase.Search(sc, connection));
        }


        #endregion
    }
}

