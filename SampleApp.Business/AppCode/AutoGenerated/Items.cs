﻿
using System;
using System.Configuration;
using Tenor.Data;

namespace SampleApp.Business.Entities
{
    /// <summary>
    /// Represents the table Items.
    /// This is an item.
    /// </summary>
    [Serializable(), Table("Items", "")]
    public partial class Item : EntityBase
    {
        #region Properties
        // FOR ORACLE: InsertSQL = "ItemsSequence"
        // FOR POSTGRESQL: InsertSQL = "Items_ItemId_seq"

        private long _ItemId;
        /// <summary>
        /// Represents the field ItemId.
        /// 
        /// </summary>
        [Field(PrimaryKey = true, AutoNumber = true, InsertSQL = "Items_ItemId_seq")]
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


        private long? _CategoryId;
        /// <summary>
        /// Represents the field CategoryId.
        /// Used to hold category id.
        /// </summary>
        [Field()]
        public long? CategoryId
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
        [ForeignKeyField(PersonItem.Properties.ItemId, Properties.ItemId)]
        public EntityList<PersonItem> PersonItemList
        {
            get
            {
                return (EntityList<PersonItem>)GetPropertyValue();
            }
        }


        /// <summary>
        /// Represents the relationship FK_Items_Categories.
        /// </summary>
        [ForeignKeyField(Category.Properties.CategoryId, Properties.CategoryId)]
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



        /// <summary>
        /// Loads Items from the database with these keys.
        /// </summary>
        public Item(long pItemId)
            :
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

            return (Item[])(EntityBase.Search(sc, connection));
        }


        #endregion
    }
}