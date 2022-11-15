using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace GRP.DataAccess
{
    public static class InstanceMapper<T> where T : class
    {
        public static T Create(DataRow Data)
        {
            T retVal = null;
            string propName = string.Empty;

            if (Data != null)
            {
                retVal = CreateInstance();

                foreach (PropertyInfo pi in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    propName = pi.Name;
                    //if (!Data.Table.Columns.Contains(propName))
                    //    propName = pi.Name.ToUpper();
                    //if (!Data.Table.Columns.Contains(propName))
                    //    propName = pi.Name.ToLower();

                    if (Data.Table.Columns.Contains(propName))
                        pi.SetValue(retVal, Data[propName] == DBNull.Value ? null : Convert.ChangeType(Data[propName], pi.PropertyType), null);
                }
            }

            return retVal;
        }

        public static List<T> CreateList(DataRowCollection Data)
        {
            T obj = null;
            List<T> retVal = new List<T>();
            if (Data != null && Data.Count > 0)
            {
                foreach (DataRow row in Data)
                {
                    obj = Create(row);
                    retVal.Add(obj);
                }
            }
            return retVal;
        }

        public static List<T> CreateList(IEnumerable<DataRow> Data)
        {
            T obj = null;
            List<T> retVal = new List<T>();
            if (Data != null && Data.Count() > 0)
            {
                foreach (DataRow row in Data)
                {
                    obj = Create(row);
                    retVal.Add(obj);
                }
            }
            return retVal;
        }

        public static T Create(DataRow Data, Dictionary<string, string> PropMapping)
        {
            T retVal = null;
            string propName = string.Empty;

            if (Data != null)
            {
                retVal = CreateInstance();

                foreach (PropertyInfo pi in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    propName = PropMapping.ContainsKey(pi.Name) ? PropMapping[pi.Name] : string.Empty;

                    if (Data.Table.Columns.Contains(propName))
                        pi.SetValue(retVal, Data[propName] == DBNull.Value ? null : Convert.ChangeType(Data[propName], pi.PropertyType), null);
                }
            }

            return retVal;
        }

        public static List<T> CreateList(DataRowCollection Data, Dictionary<string, string> PropMapping)
        {
            T obj = null;
            List<T> retVal = new List<T>();
            if (Data != null && Data.Count > 0)
            {
                foreach (DataRow row in Data)
                {
                    obj = Create(row, PropMapping);
                    retVal.Add(obj);
                }
            }
            return retVal;
        }


        private static T CreateInstance()
        {
            Type objType = typeof(T);
            T retVal = Activator.CreateInstance(objType) as T;

            return retVal;
        }

        //private static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        //{
        //    PropertyInfo propInfo = null;
        //    do
        //    {
        //        propInfo = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        //        type = type.BaseType;
        //    }
        //    while (propInfo == null && type != null);
        //    return propInfo;
        //}
    }
}