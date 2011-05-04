using System.Collections.Generic;
using System.Configuration;
using System;
using Tenor.Data;

namespace SampleApp.Business.Entities
{
    /// <summary>
    /// Represents the table PersonDepartment.
    /// </summary>
    [Serializable(), Table("Person_Department", "")]
    public partial class PersonDepartment : EntityBase
    {

        #region Properties

        private long _DepartmentId;
        /// <summary>
        /// Represents the field DepartmentId.
        /// </summary>
        [Field(PrimaryKey = true)]
        public long DepartmentId
        {
            get
            {
                return _DepartmentId;
            }
            set
            {
                _DepartmentId = value;
            }
        }

        private long _PersonId;
        /// <summary>
        /// Represents the field PersonId.
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
        public partial class Properties
        {
            private Properties() { }
            public const string DepartmentId = "DepartmentId";
            public const string PersonId = "PersonId";
        }

        #endregion

        #region Foreign Keys

        /// <summary>
        /// Represents the relationship PersonDepartment_Person.
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
            }
        }

        /// <summary>
        /// Represents the relationship PersonDepartment_Department1.
        /// </summary>
        [ForeignKeyField(Department.Properties.DepartmentId, Properties.DepartmentId)]
        public Department Department
        {
            get
            {
                return (Department)GetPropertyValue();
            }
            set
            {
                SetPropertyValue(value);
            }
        }

        #endregion

        #region Constructors And Metadata

        public PersonDepartment()
        { }


        /// <summary>
        /// Loads PersonDepartment from the database with these keys.
        /// </summary>
        public PersonDepartment(long pDepartmentId, long pPersonId) :
            base()
        {
            this.DepartmentId = pDepartmentId;
            this.PersonId = pPersonId;
            Bind();
        }

        #endregion

        #region Search

        public static PersonDepartment[] Search(ConditionCollection conditions, SortingCollection sorting)
        {
            return Search(conditions, sorting, false);
        }

        public static PersonDepartment[] Search(ConditionCollection conditions, SortingCollection sorting, bool distinct)
        {
            return Search(conditions, sorting, distinct, 0);
        }

        public static PersonDepartment[] Search(ConditionCollection conditions, SortingCollection sorting, bool distinct, int limit)
        {
            return Search(conditions, sorting, distinct, limit, null);
        }

        /// <summary>
        /// Performs a search within this class.
        /// </summary>
        public static PersonDepartment[] Search(ConditionCollection conditions, SortingCollection sorting, bool distinct, int limit, ConnectionStringSettings connection)
        {
            SearchOptions sc = new SearchOptions(typeof(PersonDepartment));
            if (conditions != null)
                sc.Conditions = conditions;
            if (sorting != null)
                sc.Sorting = sorting;

            sc.Distinct = distinct;
            sc.Top = limit;

            return (PersonDepartment[])(EntityBase.Search(sc, connection));
        }

        #endregion

    }
}
