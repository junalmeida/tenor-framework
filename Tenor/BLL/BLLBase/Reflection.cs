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
        internal static FieldInfo[] GetFields(Type InstanceType, bool? IsPrimaryKey, string[] Filter)
        {
            List<FieldInfo> returnValue = new List<FieldInfo>();
            foreach (System.Reflection.PropertyInfo i in InstanceType.GetProperties())
            {
                if (((FieldAttribute[])(i.GetCustomAttributes(typeof(FieldAttribute), true))).Length > 0)
                {
                    //Dim attribute As FieldAttribute = CType(i.GetCustomAttributes(GetType(FieldAttribute), True), FieldAttribute())(0)
                    FieldInfo campo = new FieldInfo(i); 

                    if (!IsPrimaryKey.HasValue || (campo.PrimaryKey == IsPrimaryKey.Value))
                    {
                        if (Filter == null || Filter.Length == 0 || Array.IndexOf<string>(Filter, i.Name) > -1)
                        {
                            returnValue.Add(campo);
                        }
                    }
                }
            }
            return returnValue.ToArray();
        }

        private static FieldInfo[] GetPrimaryKeys(Type InstanceType)
        {
            return GetFields(InstanceType, true);
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
                ForeignKeyAttribute[] foreignkeys = (ForeignKeyAttribute[])(i.GetCustomAttributes(typeof(ForeignKeyAttribute), true));
                if (foreignkeys.Length > 0)
                {
                    ForeignKeyInfo foreign = new ForeignKeyInfo(i);

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
                SpecialFieldAttribute[] sps = (SpecialFieldAttribute[])(i.GetCustomAttributes(typeof(SpecialFieldAttribute), true));
                if (sps.Length > 0)
                {
                    SpecialFieldInfo spInfo = new SpecialFieldInfo(i);
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
