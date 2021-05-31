using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Common
{
    /// <summary>
    /// 枚举扩展类
    /// </summary>
    public static class EnumUtil
    {
        /// <summary>
        /// 获取枚举对象
        /// </summary>
        /// <typeparam name="T">枚举对象</typeparam>
        /// <param name="eName">枚举名称</param>
        /// <param name="isIgnoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static T ToEnum<T>(this string eName, bool isIgnoreCase = false) where T : Enum
        {
            try
            {
                return (T)Enum.Parse(typeof(T), eName, isIgnoreCase);
            }
            catch
            {
                // ignored
            }
            return default(T);
        }
        /// <summary>
        /// 获取所有枚举字符串列表
        /// </summary>
        /// <param name="e">枚举对象</param>
        /// <returns></returns>
        public static List<string> GetEnumNameList(this Enum e)
        {
            return e.GetType().GetEnumNameList();
        }
        /// <summary>
        /// 获取所有枚举字符串列表
        /// </summary>
        /// <param name="eType">枚举类型</param>
        /// <returns></returns>
        public static List<string> GetEnumNameList(this Type eType)
        {
            return eType.GetEnumNames().ToList();
        }
        /// <summary>
        /// 获取枚举所有数值
        /// </summary>
        /// <param name="eType">枚举类型</param>
        /// <returns></returns>
        public static List<int> GetEnumValueList(this Type eType)
        {
            var list = new List<int>();
            foreach (var value in eType.GetEnumValues())
            {
                list.Add(value.ToInt());
            }
            return list;
        }
        /// <summary>
        /// 获取枚举所有数值
        /// </summary>
        /// <param name="e">枚举对象</param>
        /// <returns></returns>
        public static List<int> GetEnumValueList(this Enum e)
        {
            return e.GetType().GetEnumValueList();
        }

        /// <summary>
        /// 获取枚举项的Description属性描述
        /// </summary>
        /// <param name="e"></param>
        /// <param name="isDesc2">是否是Description2Attribute</param>
        /// <returns>Description属性描述</returns>
        public static string GetEnumDescription(this Enum e, bool isDesc2 = false)
        {
            FieldInfo field = e.GetType().GetField(e.ToString());

            if (isDesc2)
            {
                Description2Attribute attribute = Attribute.GetCustomAttribute(field, typeof(Description2Attribute)) as Description2Attribute;
                return attribute is null ? e.ToString() : attribute.Description;
            }
            else
            {
                DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                return attribute is null ? e.ToString() : attribute.Description;
            }
        }

        /// <summary>
        /// 获取枚举的所有Description属性描述
        /// </summary>
        /// <param name="eType">枚举类型</param>
        /// <param name="isDesc2">是否是Description2Attribute</param>
        /// <returns>Description属性描述List</returns>
        public static List<string> GetEnumDescriptionList(this Type eType, bool isDesc2 = false)
        {
            List<string> listDesc = new List<string>();
            foreach (string name in eType.GetEnumNames())
            {
                FieldInfo field = eType.GetField(name);
                if (isDesc2)
                {
                    Description2Attribute attribute = Attribute.GetCustomAttribute(field, typeof(Description2Attribute)) as Description2Attribute;
                    listDesc.Add(attribute == null ? name : attribute.Description);
                }
                else
                {
                    DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    listDesc.Add(attribute == null ? name : attribute.Description);
                }
            }
            return listDesc;
        }
        /// <summary>
        /// 获取枚举的所有Description属性描述
        /// </summary>
        /// <param name="e">枚举对象</param>
        /// <returns>Description属性描述List</returns>
        public static List<string> GetEnumDescriptionList(this Enum e)
        {
            return e.GetType().GetEnumDescriptionList();
        }

        /// <summary>
        /// 获取枚举的所有Description属性描述和枚举名称
        /// key:枚举名称  value:Description
        /// </summary>
        /// <param name="eType">枚举类型</param>
        /// <param name="isDesc2">是否是Description2Attribute</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetEnumDescriptionDic(this Type eType, bool isDesc2 = false)
        {
            Dictionary<string, string> dicDesc = new Dictionary<string, string>();
            foreach (string name in eType.GetEnumNames())
            {
                FieldInfo field = eType.GetField(name);
                string desc = "";
                if (isDesc2)
                {
                    Description2Attribute attribute = Attribute.GetCustomAttribute(field, typeof(Description2Attribute)) as Description2Attribute;
                    desc = attribute == null ? name : attribute.Description;
                }
                else
                {
                    DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    desc = attribute == null ? name : attribute.Description;
                }
                dicDesc.Add(name, desc);
            }
            return dicDesc;
        }
        /// <summary>
        /// 获取枚举的所有Description属性描述和枚举名称
        /// key:枚举名称  value:Description
        /// </summary>
        /// <param name="e">枚举对象</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetEnumDescriptionDic(this Enum e)
        {
            return e.GetType().GetEnumDescriptionDic();
        }
        /// <summary>
        /// 获取枚举的所有Description属性描述和枚举名称
        /// key:枚举名称  value:枚举数字
        /// </summary>
        /// <param name="eType">枚举类型</param>
        /// <returns></returns>
        public static Dictionary<string, int> GetEnumNameValueDic(this Type eType)
        {
            Dictionary<string, int> dicDesc = new Dictionary<string, int>();
            foreach (int eCode in Enum.GetValues(eType))
            {
                dicDesc.Add(Enum.GetName(eType, eCode), eCode);
            }
            return dicDesc;
        }
        /// <summary>
        /// 获取枚举的所有Description属性描述和枚举名称
        /// key:枚举名称  value:枚举数字
        /// </summary>
        /// <param name="e">枚举对象</param>
        /// <returns></returns>
        public static Dictionary<string, int> GetEnumNameValueDic(this Enum e)
        {
            return e.GetType().GetEnumNameValueDic();
        }

        public static Dictionary<string, string> GetEnumDescription<T>()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            FieldInfo[] fields = typeof(T).GetFields();
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    object[] attr = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    string description = attr.Length == 0 ? field.Name : ((DescriptionAttribute)attr[0]).Description;
                    dic.Add(field.Name, description);
                }
            }
            return dic;
        }

        public static List<KeyValuePair<string, string>> GetEnumDescriptionList<T>()
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            FieldInfo[] fields = typeof(T).GetFields();
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    object[] attr = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    string description = attr.Length == 0 ? field.Name : ((DescriptionAttribute)attr[0]).Description;
                    result.Add(new KeyValuePair<string, string>(field.Name, description));
                }
            }
            return result;
        }
    }

    /// <summary>
    /// 枚举的第二个属性，方便使用，有需要的话，再增加第三个
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class Description2Attribute : Attribute
    {
        public Description2Attribute() : this(string.Empty)
        { }
        public Description2Attribute(string description)
        {
            this.DescriptionValue = description;
        }
        public virtual string Description => this.DescriptionValue;
        protected string DescriptionValue { get; set; }
    }
}
