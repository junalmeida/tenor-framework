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

    internal class Join
    {
        public Join()
        {
        }
        public Join(string joinAlias)
        {
            this.joinAlias = joinAlias;
        }
        public string joinAlias;
        public string parentAlias;
        public string propertyName;
        public JoinMode joinMode;

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

        public Type Class;
        public ForeignKeyInfo ForeignKey;
    }
}
