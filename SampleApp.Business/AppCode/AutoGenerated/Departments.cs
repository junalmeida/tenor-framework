﻿using System;
using System.Configuration;
using Tenor.Data;

namespace SampleApp.Business.Entities
{
    /// <summary>
    /// Represents the table Demartments.
    /// Some description on table.
    /// </summary>
    [Serializable(), Table("Departments", "")]
    public partial class Department : EntityBase
    {

        #region Properties
        // FOR ORACLE: InsertSQL = "DepartmentsSequence"
        // FOR POSTGRESQL: InsertSQL = "Departments_DepartmentId_seq"

        private long _DepartmentId;
        /// <summary>
        /// Represents the field DepartmentId.
        /// 
        /// </summary>
        [Field(PrimaryKey = true, AutoNumber = true, InsertSQL = "Departments_DepartmentId_seq")]
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

        /// <summary>
        /// Keeps a list of constants with property names.
        /// </summary>
        public static partial class Properties : object
        {
            public const string DepartmentId = "DepartmentId";
            public const string Name = "Name";
        }


        /// <summary>
        /// Represents the relationship 
        /// </summary>
        [ForeignKeyField(PersonDepartment.Properties.DepartmentId, Properties.DepartmentId)]
        public EntityList<PersonDepartment> PersonDepartmentList
        {
            get
            {
                return (EntityList<PersonDepartment>)GetPropertyValue();
            }
        }
        #endregion

        #region Constructors And Metadata


        public Department()
        { }



        /// <summary>
        /// Loads Department from the database with these keys.
        /// </summary><%
        public Department(long pDepartmentId)
            :
            base()
        {
            this.DepartmentId = pDepartmentId;

            Bind();
        }


        #endregion
        #region Search



        public static Department[] Search(ConditionCollection conditions, SortingCollection sorting)
        {
            return Search(conditions, sorting, false);
        }

        public static Department[] Search(ConditionCollection conditions, SortingCollection sorting, bool distinct)
        {
            return Search(conditions, sorting, distinct, 0);
        }

        public static Department[] Search(ConditionCollection conditions, SortingCollection sorting, bool distinct, int limit)
        {
            return Search(conditions, sorting, distinct, limit, null);
        }

        /// <summary>
        /// Performs a search within this class.
        /// </summary>
        public static Department[] Search(ConditionCollection conditions, SortingCollection sorting, bool distinct, int limit, ConnectionStringSettings connection)
        {
            SearchOptions sc = new SearchOptions(typeof(Department));
            if (conditions != null)
                sc.Conditions = conditions;
            if (sorting != null)
                sc.Sorting = sorting;

            sc.Distinct = distinct;
            sc.Top = limit;

            return (Department[])(EntityBase.Search(sc, connection));
        }


        #endregion
    }

}

