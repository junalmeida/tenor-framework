using System;
using System.Collections.Generic;
using System.Text;
using Tenor.Data;
namespace Tenor.BLL
{
    public abstract partial class BLLBase
    {
 

        /// <summary>
        /// Pega uma lista de FieldInfos com todos os fields da instância passada
        /// </summary>
        /// <param name="InstanceType">O tipo do objeto que contém os campos</param>
        /// <returns>Uma lista de FieldInfos</returns>
        /// <remarks></remarks>
        internal static FieldInfo[] GetFields(Type instanceType)
        {
            return GetFields(instanceType, null);
        }
        /// <summary>
        /// Pega uma lista de FieldInfos com todos os fields da instância passada
        /// </summary>
        /// <param name="InstanceType">O tipo do objeto que contém os campos</param>
        /// <param name="IsPrimaryKey"></param>
        /// <returns>Uma lista de FieldInfos</returns>
        /// <remarks></remarks>
        internal static FieldInfo[] GetFields(Type instanceType, bool? isPrimaryKey)
        {
            return GetFields(instanceType, isPrimaryKey, null);
        }


        /// <summary>
        /// Pega uma lista de FieldInfos com todos os fields da instância passada
        /// </summary>
        /// <param name="InstanceType">O tipo do objeto que contém os campos</param>
        /// <param name="IsPrimaryKey"></param>
        /// <returns>Uma lista de FieldInfos</returns>
        /// <remarks></remarks>
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
        /// Pega uma lista de FieldInfos com todos os fields da instância passada
        /// </summary>
        /// <param name="InstanceType">O tipo da instancia</param>
        /// <returns>Uma lista de FieldInfos</returns>
        /// <remarks></remarks>
        internal static ForeignKeyInfo[] GetForeignKeys(Type InstanceType)
        {
            List<ForeignKeyInfo> res = new List<ForeignKeyInfo>();
            foreach (System.Reflection.PropertyInfo i in InstanceType.GetProperties())
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
