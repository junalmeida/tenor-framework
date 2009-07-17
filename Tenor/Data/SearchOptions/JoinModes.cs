using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Tenor.Data
{
    /// <summary>
    /// Keeps a list of join modes.
    /// </summary>
    public enum JoinMode
    {
        /// <summary>
        /// Creates an inner join with the specified alias.
        /// </summary>
        InnerJoin,
        /// <summary>
        /// Creates a left join with the specified alias.
        /// </summary>
        LeftJoin,
        /// <summary>
        /// Creates a right join with the specified alias.
        /// </summary>
        RightJoin
    }

    /// <summary>
    /// Represents join information.
    /// </summary>
    internal class Join
    {
        public Join() { }
        public Join(string joinAlias)
        {
            /*, string parentAlias, string propertyName, JoinMode joinMode*/
            this.joinAlias = joinAlias;
            /*
            this.parentAlias = parentAlias;
            this.propertyName = propertyName;
            this.joinMode = joinMode;
             */
        }

        private string joinAlias;

        public string JoinAlias
        {
            get { return joinAlias; }
        }

        private string parentAlias;

        public string ParentAlias
        {
            get { return parentAlias; }
            set { parentAlias = value; }
        }


        private JoinMode joinMode;

        public JoinMode JoinMode
        {
            get { return joinMode; }
            set { joinMode = value; }
        }

        private string propertyName;

        public string PropertyName
        {
            get { return propertyName; }
            set { propertyName = value; }
        }

        internal TableInfo LocalTableInfo;
        internal TableInfo ForeignTableInfo;
        internal ForeignKeyInfo ForeignKey;

        public override bool Equals(object obj)
        {
            Join castObj = obj as Join;
            if (castObj == null)
                return false;

            return object.Equals(this.joinAlias, castObj.joinAlias);
        }

        public override int GetHashCode()
        {
            int hash = 57;
            hash = 27 * hash * joinAlias.GetHashCode();
            //hash = 27 * hash * parentAlias.GetHashCode();
            //hash = 27 * hash * propertyName.GetHashCode();
            //hash = 27 * hash * joinMode.GetHashCode();
            return hash;
        }
    }
}
