using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using Common.CustomException;

namespace System
{
    /// <summary>
    /// DataTable帮助类
    /// </summary>
    public static class DataTableUtil
    {
        /// <summary>
        /// DataTable转模型，只赋值公共属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <param name="ignoreCase">忽略大小写</param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dt, bool ignoreCase = false) where T : new()
        {
            List<T> listRet = new List<T>();
            if (dt != null && dt.Rows.Count > 0)
            {
                Type modelType = typeof(T);
                //模型属性
                var props = modelType.GetProperties();
                //寻找需要赋值的属性 key:table列名  value:模型属性
                Dictionary<string, PropertyInfo> dicCols = new Dictionary<string, PropertyInfo>();
                foreach (PropertyInfo prop in props)
                {
                    if (prop.CanWrite)//可写入的属性
                    {
                        if (ignoreCase)//忽略大小写
                        {
                            foreach (DataColumn column in dt.Columns)
                            {
                                if (column.ColumnName.Equals(prop.Name, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    dicCols.Add(column.ColumnName, prop);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (dt.Columns.Contains(prop.Name))
                                dicCols.Add(prop.Name, prop);
                        }
                    }
                }

                foreach (DataRow row in dt.Rows)
                {
                    T model = new T();
                    foreach (KeyValuePair<string, PropertyInfo> kv in dicCols)
                    {
                        var strRowColumn = kv.Key;
                        var propModel = kv.Value;
                        if (row[strRowColumn] != DBNull.Value)
                            propModel.SetValue(model, row[strRowColumn]);
                    }
                    listRet.Add(model);
                }
            }
            return listRet;
        }
        /// <summary>
        /// DataTable转模型，自己给模型赋值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <param name="customeOperateFunc">自定义操作赋值，参数：int行号，DataRow行值，DataColumnCollection列名</param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dt, Func<int, DataRow, DataColumnCollection, T> customeOperateFunc) where T : new()
        {
            List<T> listRet = new List<T>();
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    listRet.Add(customeOperateFunc(i, dt.Rows[i], dt.Columns));
                }
            }
            return listRet;
        }

        /// <summary>
        /// DataTable转Dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <param name="colKeyName">主键列名</param>
        /// <param name="ignoreCase">是否忽略列名大小写</param>
        /// <returns></returns>
        public static Dictionary<string,T> ToDictionary<T>(this DataTable dt,string colKeyName, bool ignoreCase = false) where T : new()
        {
            Dictionary<string,T> dicRet = new Dictionary<string, T>();
            if (dt != null && dt.Rows.Count > 0)
            {
                Type modelType = typeof(T);
                //模型属性
                var props = modelType.GetProperties();
                //寻找需要赋值的属性 key:table列名  value:模型属性
                Dictionary<string, PropertyInfo> dicCols = new Dictionary<string, PropertyInfo>();
                foreach (PropertyInfo prop in props)
                {
                    if (prop.CanWrite)//可写入的属性
                    {
                        if (ignoreCase)//忽略大小写时，效率低
                        {
                            foreach (DataColumn column in dt.Columns)
                            {
                                if (column.ColumnName.Equals(prop.Name, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    dicCols.Add(column.ColumnName, prop);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (dt.Columns.Contains(prop.Name))
                                dicCols.Add(prop.Name, prop);
                        }
                    }
                }

                if (!dt.Columns.Contains(colKeyName))
                    throw new BaseException("ToDictionary时未找到主键列");

                foreach (DataRow row in dt.Rows)
                {
                    T model = new T();
                    foreach (KeyValuePair<string, PropertyInfo> kv in dicCols)
                    {
                        var strRowColumn = kv.Key;
                        var propModel = kv.Value;
                        if (row[strRowColumn] != DBNull.Value)
                            propModel.SetValue(model, row[strRowColumn]);
                    }

                    dicRet.Add(row[colKeyName].ToString(),model);
                }
            }
            return dicRet;
        }
        /// <summary>
        /// 转换成DataTable
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static DataTable ToDataTable(this DataRow[] rows)
        {
            if(rows==null||rows.Length==0) return new DataTable();
            DataTable dtTemp = rows[0].Table.Clone();
            foreach (var row in rows)
            {
                dtTemp.ImportRow(row);
            }
            return dtTemp;
        }
    }
}
