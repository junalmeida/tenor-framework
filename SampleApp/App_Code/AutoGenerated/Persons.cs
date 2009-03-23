using System;
using System.Configuration;
using Tenor.BLL;
using Tenor.Data;

namespace SampleApp.Business.Entities
{
    /// <summary>
    /// Represents the table Persons.
    /// Some description on table.
    /// </summary>
    [Serializable(), Table("Persons", "")]
    public partial class Person : BLLBase
    {

        #region Properties


        private int _PersonId;
        /// <summary>
        /// Represents the field PersonId.
        /// 
        /// </summary>
        [Field(PrimaryKey = true, AutoNumber = true)]
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


        private string _Name;
        /// <summary>
        /// Represents the field Name.
        /// 
        /// </summary>
        [Field()]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }


        private string _Email;
        /// <summary>
        /// Represents the field Email.
        /// 
        /// </summary>
        [Field()]
        public string Email
        {
            get
            {
                return _Email;
            }
            set
            {
                _Email = value;
            }
        }


        private DateTime? _Expires;
        /// <summary>
        /// Represents the field Expires.
        /// 
        /// </summary>
        [Field()]
        public DateTime? Expires
        {
            get
            {
                return _Expires;
            }
            set
            {
                _Expires = value;
            }
        }


        /// <summary>
        /// Keeps a list of constants with property names.
        /// </summary>
        public partial class Properties : object
        {
            private Properties() { }

            public const string PersonId = "PersonId";


            public const string Name = "Name";


            public const string Email = "Email";


            public const string Expires = "Expires";


        }


        #endregion
        #region Foreign Keys


        /// <summary>
        /// Represents the relationship FK_Persons_Items_Persons.
        /// </summary>
        [ForeignKey(PersonItem.Properties.PersonId, Properties.PersonId)]

        public BLLCollection<PersonItem> PersonItemList
        {
            get
            {
                return (BLLCollection<PersonItem>)GetPropertyValue();
            }
        }


        #endregion
        #region Constructors And Metadata


        public Person()
        { }


        public Person(bool lazyLoadingDisabled) :
            base(lazyLoadingDisabled)
        { }


        /// <summary>
        /// Loads Persons from the database with these keys.
        /// </summary><%
        public Person(int pPersonId) :
            base()
        {
            this.PersonId = pPersonId;

            Bind();
        }


        #endregion
        #region Search



        public static Person[] Search(ConditionCollection conditions, SortingCollection sorting)
        {
            return Search(conditions, sorting, false);
        }

        public static Person[] Search(ConditionCollection conditions, SortingCollection sorting, bool distinct)
        {
            return Search(conditions, sorting, distinct, 0);
        }

        public static Person[] Search(ConditionCollection conditions, SortingCollection sorting, bool distinct, int limit)
        {
            return Search(conditions, sorting, distinct, limit, null);
        }

        /// <summary>
        /// Performs a search within this class.
        /// </summary>
        public static Person[] Search(ConditionCollection conditions, SortingCollection sorting, bool distinct, int limit, ConnectionStringSettings connection)
        {
            SearchOptions sc = new SearchOptions(typeof(Person));
            if (conditions != null)
                sc.Conditions = conditions;
            if (sorting != null)
                sc.Sorting = sorting;

            sc.Distinct = distinct;
            sc.Top = limit;

            return (Person[])(BLLBase.Search(sc, connection));
        }


        #endregion
    }
}

