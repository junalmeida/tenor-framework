
using System.Collections.Generic;
using System.Configuration;
using System;

using Tenor.BLL;
using Tenor.Data;

namespace SampleApp.Business.Entities
{
    /// <summary>
    /// Represents the table Items.
    /// This is an item.
    /// </summary>
    [Serializable(), Table("Items", "dbo")]
    public partial class Item : BLLBase
    {
        #region Properties


        private int _ItemId;
        /// <summary>
        /// Represents the field ItemId.
        /// 
        /// </summary>
        [Field(PrimaryKey = true, AutoNumber = true)]
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


        private string _Description;
        /// <summary>
        /// Represents the field Description.
        /// The description of this item.
        /// </summary>
        [Field()]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }


        private int? _CategoryId;
        /// <summary>
        /// Represents the field CategoryId.
        /// Used to hold category id.
        /// </summary>
        [Field()]
        public int? CategoryId
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


        /// <summary>
        /// Keeps a list of constants with property names.
        /// </summary>
        public partial class Properties : object
        {
            private Properties() { }

            public const string ItemId = "ItemId";
            public const string Description = "Description";
            public const string CategoryId = "CategoryId";
        }


        #endregion
        #region Foreign Keys


        /// <summary>
        /// Represents the relationship FK_Persons_Items_Items.
        /// </summary>
        [ForeignKey(PersonItem.Properties.ItemId, Properties.ItemId)]
        public BLLCollection<PersonItem> PersonItemList
        {
            get
            {
                return (BLLCollection<PersonItem>)GetPropertyValue();
            }
        }


        /// <summary>
        /// Represents the relationship FK_Items_Categories.
        /// </summary>
        [ForeignKey(Category.Properties.CategoryId, Properties.CategoryId)]

        public Category Category
        {
            get
            {
                return (Category)GetPropertyValue();
            }
            set
            {
                SetPropertyValue(value);
            }
        }


        #endregion
        #region Constructors And Metadata


        public Item()
        { }


        public Item(bool lazyLoadingDisabled) :
            base(lazyLoadingDisabled)
        { }


        /// <summary>
        /// Loads Items from the database with these keys.
        /// </summary>
        public Item(int pItemId) :
            base()
        {
            this.ItemId = pItemId;

            Bind();
        }


        #endregion
        #region Search



        public static Item[] Search(ConditionCollection conditions, SortingCollection sorting)
        {
            return Search(conditions, sorting, false);
        }

        public static Item[] Search(ConditionCollection conditions, SortingCollection sorting, bool distinct)
        {
            return Search(conditions, sorting, distinct, 0);
        }

        public static Item[] Search(ConditionCollection conditions, SortingCollection sorting, bool distinct, int limit)
        {
            return Search(conditions, sorting, distinct, limit, null);
        }

        /// <summary>
        /// Performs a search within this class.
        /// </summary>
        public static Item[] Search(ConditionCollection conditions, SortingCollection sorting, bool distinct, int limit, ConnectionStringSettings connection)
        {
            SearchOptions sc = new SearchOptions(typeof(Item));
            if (conditions != null)
                sc.Conditions = conditions;
            if (sorting != null)
                sc.Sorting = sorting;

            sc.Distinct = distinct;
            sc.Top = limit;

            return (Item[])(BLLBase.Search(sc, connection));
        }


        #endregion
    }
}