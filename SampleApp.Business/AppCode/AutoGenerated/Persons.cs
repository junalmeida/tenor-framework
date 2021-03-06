﻿using System;
using System.Configuration;
using System.IO;
using Tenor.Data;

namespace SampleApp.Business.Entities
{
    /// <summary>
    /// Represents the table Persons.
    /// Some description on table.
    /// </summary>
    [Serializable(), Table("Persons", "")]
    public partial class Person : EntityBase
    {

        #region Properties
        // FOR ORACLE: InsertSQL = "PersonsSequence"
        // FOR POSTGRESQL: InsertSQL = "Persons_PersonId_seq"

        private long _PersonId;
        /// <summary>
        /// Represents the field PersonId.
        /// 
        /// </summary>
        [Field(PrimaryKey = true, AutoNumber = true, InsertSQL = "Persons_PersonId_seq")]
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

        private bool active;

        [Field()]
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        [Field("Photo", LazyLoading = true)]
        public Stream Photo
        {
            get
            {
                return (Stream)GetPropertyValue();
            }
            set
            {
                image = null;
                SetPropertyValue(value);
            }
        }

        [Field("Photo2", LazyLoading = true)]
        public byte[] Photo2
        {
            get
            {
                return (byte[])GetPropertyValue();
            }
            set
            {
                image = null;
                SetPropertyValue(value);
            }
        }

        [Field("Photo3")]
        public byte[] Photo3
        {
            get;
            set;
        }

        private MaritalStatus? maritalStatus;

        [Field()]
        public MaritalStatus? MaritalStatus
        {
            get { return maritalStatus; }
            set { maritalStatus = value; }
        }

        private ContractType? contractType;

        /// <summary>
        /// Specify the contract type. 
        /// * For testing string enum.
        /// </summary>
        [Field()]
        public ContractType? ContractType
        {
            get { return contractType; }
            set { contractType = value; }
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
            public const string Active = "Active";
            public const string Photo = "Photo";
            public const string MaritalStatus = "MaritalStatus";
        }


        #endregion
        #region Foreign Keys


        /// <summary>
        /// Represents the relationship FK_Persons_Items_Persons.
        /// </summary>
        [ForeignKeyField(PersonItem.Properties.PersonId, Properties.PersonId)]
        public EntityList<PersonItem> PersonItemList
        {
            get
            {
                return (EntityList<PersonItem>)GetPropertyValue();
            }
        }


        /// <summary>
        /// Example of N-N relations.
        /// </summary>
        [ForeignKey(ManyToManyTable = "person_department")]
        [ForeignKeyField(Department.Properties.DepartmentId, Person.Properties.PersonId, "DepartmentId", "PersonId")]
        public EntityList<Department> DepartmentList
        {
            get
            {
                return (EntityList<Department>)GetPropertyValue();
            }
        }

        #endregion
        #region Constructors And Metadata


        public Person()
        { }



        /// <summary>
        /// Loads Persons from the database with these keys.
        /// </summary><%
        public Person(long pPersonId)
            :
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

            return (Person[])(EntityBase.Search(sc, connection));
        }


        #endregion
    }

}

