using System;
using System.Collections.Generic;
using System.Text;

namespace Tenor.Data.Dialects
{
    public static class DialectFactory
    {
        /// <summary>
        /// Creates an instance of a dialect based on current connection.
        /// </summary>
        internal static IDialect CreateDialect(System.Configuration.ConnectionStringSettings connection)
        {
            Dictionary<string, Type> dialects = GetAvailableDialectsAndTypes();
            if (dialects.ContainsKey(connection.ProviderName))
            {
                return (IDialect)Activator.CreateInstance(dialects[connection.ProviderName]);
            }
            else
            {
                throw new NotSupportedException("The provider '" + connection.ProviderName + "' is not supported yet. Please send a feature request.");
            }
        }

        public static string[] GetAvailableDialects()
        {
            return new List<string>(GetAvailableDialectsAndTypes().Keys).ToArray();
        }

        internal static Dictionary<string, Type> GetAvailableDialectsAndTypes()
        {
            Dictionary<string, Type> dialects = new Dictionary<string, Type>();
            dialects.Add("System.Data.SqlClient", typeof(TSql.TSql));
            return dialects;
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
            if (join.LocalTableInfo == null)
            {
                if (string.IsNullOrEmpty(join.parentAlias))
                {
                    join.LocalTableInfo = TableInfo.CreateTableInfo(baseClass);
                }
                else
                {
                    int pos = list.IndexOf(new Join(join.parentAlias));
                    if (pos == -1)
                        throw new InvalidOperationException("Cannot find the parent alias for '" + join.joinAlias + "'");
                    Join parent = list[pos];
                    if (parent.LocalTableInfo == null)
                        SetProperty(list, parent, baseClass);

                    join.LocalTableInfo = TableInfo.CreateTableInfo(parent.ForeignKey.ElementType);
                }
            }

            join.ForeignKey = ForeignKeyInfo.Create(join.LocalTableInfo.RelatedTable.GetProperty(join.propertyName));
            if (join.ForeignKey == null)
                throw new InvalidOperationException("Cannot find '" + join.propertyName + "' on '" + join.LocalTableInfo.RelatedTable.Name + "' class. You must define a ForeignKey on that class.");
            join.ForeignTableInfo = TableInfo.CreateTableInfo(join.ForeignKey.ElementType);
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