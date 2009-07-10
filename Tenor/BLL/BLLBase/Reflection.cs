using System;
using System.Collections.Generic;
using System.Text;
using Tenor.Data;
namespace Tenor.BLL
{
    public abstract partial class BLLBase
    {
 

        /// <summary>
        /// </summary>
        internal static FieldInfo[] GetFields(Type instanceType)
        {
            return GetFields(instanceType, null);
        }
        /// <summary>
        /// </summary>
        /// <param name="isPrimaryKey">True to get only primary keys, false to get only non-primary key fields, and null to get everything.</param>
        internal static FieldInfo[] GetFields(Type instanceType, bool? isPrimaryKey)
        {
            return GetFields(instanceType, isPrimaryKey, null);
        }


        /// <summary>
        /// </summary>
        /// <param name="isPrimaryKey">True to get only primary keys, false to get only non-primary key fields, and null to get everything.</param>
        internal static FieldInfo[] GetFields(Type InstanceType, bool? isPrimaryKey, string[] filter)
        {
            List<FieldInfo> returnValue = new List<FieldInfo>();
            foreach (System.Reflection.PropertyInfo i in InstanceType.GetProperties())
            {

                FieldInfo field = FieldInfo.Create(i); 
                if (field != null)
                {
                    if (!isPrimaryKey.HasValue || (field.PrimaryKey == isPrimaryKey.Value))
                    {
                        if (filter == null || filter.Length == 0 || Array.IndexOf<string>(filter, i.Name) > -1)
                        {
                            returnValue.Add(field);
                        }
                    }
                }
            }
            return returnValue.ToArray();
        }

        private static FieldInfo[] GetPrimaryKeys(Type instanceType)
        {
            return GetFields(instanceType, true);
        }

        /// <summary>
        /// </summary>
        internal static ForeignKeyInfo[] GetForeignKeys(Type instanceType)
        {
            List<ForeignKeyInfo> res = new List<ForeignKeyInfo>();
            foreach (System.Reflection.PropertyInfo i in instanceType.GetProperties())
            {
                ForeignKeyInfo foreign = ForeignKeyInfo.Create(i);
                if (foreign != null)
                {
                    res.Add(foreign);
                }
            }
            return res.ToArray();
        }

        private static SpecialFieldInfo[] GetSpecialFields(Type instanceType)
        {
            List<SpecialFieldInfo> res = new List<SpecialFieldInfo>();
            foreach (System.Reflection.PropertyInfo i in instanceType.GetProperties())
            {
                SpecialFieldInfo spInfo = SpecialFieldInfo.Create(i);
                if (spInfo != null)
                {
                    res.Add(spInfo);
                }
            }
            return res.ToArray();
        }


        private TableInfo classMetadata;
        private TableInfo ClassMetadata
        {
            get
            {
                if (classMetadata == null)
                    classMetadata = TableInfo.CreateTableInfo(this.GetType());
                return classMetadata;
            }
        }


    }
}
