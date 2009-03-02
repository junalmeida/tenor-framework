using System;
using System.Collections.Generic;
using System.Text;

namespace Tenor.Data.Dialects
{
    internal class DialectFactory
    {
        /// <summary>
        /// Creates an instance of a dialect based on current connection.
        /// </summary>
        internal static IDialect CreateDialect(System.Configuration.ConnectionStringSettings connection)
        {
            switch (connection.ProviderName)
            {
                case "System.Data.SqlClient":
                    return new TSql.TSql();
                default:
                    throw new NotSupportedException("The provider '" + connection.ProviderName + "' is not supported yet. Please send a feature request.");
            }

        }


        internal static Join[] GetPlainJoins(ConditionCollection conditions, Type baseClass)
        {
            List<Join> list = new List<Join>();
            GetPlainJoins(conditions, list);

            foreach (Join join in list)
            {
                if (join.ForeignKey == null)
                {
                    SetProperty(list, join, baseClass);
                }
            }
            return list.ToArray();
        }

        private static void SetProperty(List<Join> list, Join join, Type baseClass)
        {
            if (join.Class == null)
            {
                if (string.IsNullOrEmpty(join.parentAlias))
                {
                    join.Class = baseClass;
                }
                else
                {
                    int pos = list.IndexOf(new Join(join.parentAlias));
                    if (pos == -1)
                        throw new InvalidOperationException("Cannot find the parent alias for '" + join.joinAlias + "'");
                    Join parent = list[pos];
                    if (parent.Class == null)
                        SetProperty(list, parent, baseClass);

                    join.Class = parent.ForeignKey.ElementType;
                }
            }

            join.ForeignKey = ForeignKeyInfo.Create(join.Class.GetProperty(join.propertyName));
            if (join.ForeignKey == null)
                throw new InvalidOperationException("Cannot find '" + join.propertyName + "' on '" + join.Class.Name + "' class. You must define a ForeignKey on that class.");
        }


        private static void GetPlainJoins(ConditionCollection conditions, List<Join> list)
        {
            foreach (Join join in conditions.includes)
            {
                if (list.Contains(join))
                    throw new InvalidOperationException("The join '" + join.joinAlias + "' is already on the list.");
                else
                    list.Add(join);
            }
            foreach (object item in conditions)
            {
                if (item.GetType() == typeof(ConditionCollection))
                {
                    GetPlainJoins((ConditionCollection)item, list);
                }
            }
        }


    }
}