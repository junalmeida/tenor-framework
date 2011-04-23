using System;
using System.Configuration;
using Tenor.Data;

namespace Teste.TESTE
{
    /// <summary>
    /// Represents the table Product.
    /// </summary>
    [Serializable(), Table("Product", "TESTE")]
    public partial class Product : EntityBase
    {

        #region Properties

        private decimal _ID;
        /// <summary>
        /// Represents the field ID.
        /// </summary>
        [Field(PrimaryKey = true, AutoNumber = true, InsertSQL = "ProductSequence")]
        public decimal ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }

        private string _Nome;
        /// <summary>
        /// Represents the field Nome.
        /// </summary>
        [Field()]
        public string Nome
        {
            get
            {
                return _Nome;
            }
            set
            {
                _Nome = value;
            }
        }

        /// <summary>
        /// Keeps a list of constants with property names.
        /// </summary>
        public partial class Properties : object
        {
            private Properties() { }
            public const string ID = "ID";
            public const string Nome = "Nome";
        }

        #endregion

        #region Foreign Keys

        #endregion

        #region Constructors And Metadata

        public Product()
        { }

        /// <summary>
        /// Loads Product from the database with these keys.
        /// </summary>
        public Product(decimal pID) :
            base()
        {
            this.ID = pID;
            Bind();
        }

        #endregion

        #region Search

        public static Product[] Search(ConditionCollection conditions, SortingCollection sorting)
        {
            return Search(conditions, sorting, false);
        }

        public static Product[] Search(ConditionCollection conditions, SortingCollection sorting, bool distinct)
        {
            return Search(conditions, sorting, distinct, 0);
        }

        public static Product[] Search(ConditionCollection conditions, SortingCollection sorting, bool distinct, int limit)
        {
            return Search(conditions, sorting, distinct, limit, null);
        }

        /// <summary>
        /// Performs a search within this class.
        /// </summary>
        public static Product[] Search(ConditionCollection conditions, SortingCollection sorting, bool distinct, int limit, ConnectionStringSettings connection)
        {
            SearchOptions sc = new SearchOptions(typeof(Product));
            if (conditions != null)
                sc.Conditions = conditions;
            if (sorting != null)
                sc.Sorting = sorting;

            sc.Distinct = distinct;
            sc.Top = limit;

            return (Product[])(EntityBase.Search(sc, connection));
        }

        #endregion

    }
}
