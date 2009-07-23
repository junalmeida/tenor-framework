using System.Collections.Generic;
using System.Configuration;
using System;

using Tenor.BLL;
using Tenor.Data;

namespace SampleApp.Business.Entities
{
    /// <summary>
    /// Represents the table Categories.
    /// This is a category
    /// </summary>
    [Serializable(), Table("Categories", "")]
    public partial class Category : BLLBase
    {
        // FOR ORACLE: InsertSQL = "CategoriesSequence"
        // FOR POSTGRESQL: InsertSQL = "Categories_CategoryId_seq"

        #region Properties

        // We had to put every autoincrement column as long, because SQLite only supports
        // INTEGER AUTOINCREMENT, and INTEGER is mapped as Int64.
        private long _CategoryId;
        /// <summary>
        /// Represents the field CategoryId.
        /// 
        /// </summary>
        [Field(PrimaryKey = true, AutoNumber = true, InsertSQL = "Categories_CategoryId_seq")]
        public long CategoryId
        {
            get
            {
                return _CategoryId;
            }
            set
            {
                _CategoryId = value;
            }
        }


        private string _Name;
        /// <summary>
        /// Represents the field Name.
        /// The name of the category.
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
        public partial class Properties : object
        {
            private Properties() { }

            public const string CategoryId = "CategoryId";
            public const string Name = "Name";
        }


        #endregion
        #region Foreign Keys


        /// <summary>
        /// Represents the relationship FK_Items_Categories.
        /// </summary>
        [ForeignKey(Item.Properties.CategoryId, Properties.CategoryId)]

        public BLLCollection<Item> ItemList
        {
            get
            {
                return (BLLCollection<Item>)GetPropertyValue();
            }
        }


        #endregion
        #region Constructors And Metadata


        public Category()
        { }




        /// <summary>
        /// Loads Categories from the database with these keys.
        /// </summary><%
        public Category(long pCategoryId) :
            base()
        {
            this.CategoryId = pCategoryId;

            Bind();
        }


        #endregion
        #region Search



        public static Category[] Search(ConditionCollection conditions, SortingCollection sorting)
        {
            return Search(conditions, sorting, false);
        }

        public static Category[] Search(ConditionCollection conditions, SortingCollection sorting, bool distinct)
        {
            return Search(conditions, sorting, distinct, 0);
        }

        public static Category[] Search(ConditionCollection conditions, SortingCollection sorting, bool distinct, int limit)
        {
            return Search(conditions, sorting, distinct, limit, null);
        }

        /// <summary>
        /// Performs a search within this class.
        /// </summary>
        public static Category[] Search(ConditionCollection conditions, SortingCollection sorting, bool distinct, int limit, ConnectionStringSettings connection)
        {
            SearchOptions sc = new SearchOptions(typeof(Category));
            if (conditions != null)
                sc.Conditions = conditions;
            if (sorting != null)
                sc.Sorting = sorting;

            sc.Distinct = distinct;
            sc.Top = limit;

            return (Category[])(BLLBase.Search(sc, connection));
        }


        #endregion
    }
}