using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using Common;

namespace System
{
    /// <summary>
    /// 字符串工具
    /// </summary>
    public static class StringUtil
    {
        /// <summary>
        /// 字符串是否为空
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// 字符串trim后是否为空
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNullOrEmptyTrim(this string s)
        {
            return s == null || string.IsNullOrEmpty(s.Trim());
        }
        /// <summary>
        /// 字符串是否为不为空
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty(this string s)
        {
            return !string.IsNullOrEmpty(s);
        }
        /// <summary>
        /// 转换成base64的字符串
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encoding">编码格式，默认是utf8</param>
        /// <returns></returns>
        public static string ToBase64String(string s, Encoding encoding = null)
        {
            if (encoding is null)
                encoding = Encoding.UTF8;
            return Convert.ToBase64String(encoding.GetBytes(s));
        }
        /// <summary>
        /// 将base64字符串转成普通字符串
        /// </summary>
        /// <param name="base64str">base64字符串</param>
        /// <param name="encoding">编码格式，默认是utf8</param>
        /// <returns></returns>
        public static string ToStringFromBase64(string base64str, Encoding encoding = null)
        {
            if (encoding is null)
                encoding = Encoding.UTF8;
            return encoding.GetString(Convert.FromBase64String(base64str));
        }
        /// <summary>
        /// string.Format格式化字符串
        /// </summary>
        /// <param name="s"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Format(this string s, params object[] args)
        {
            return string.Format((IFormatProvider)CultureInfo.InvariantCulture, s, args);
        }
        /// <summary>
        /// 反序列化NewtonJson序列化的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public static T DeserializeNewtonJson<T>(this string s)
        {
            return JsonConvert.DeserializeObject<T>(s);
        }
        /// <summary>
        /// 反序列化NewtonJson序列化的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static T DeserializeNewtonJson<T>(this string s,JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject<T>(s,settings);
        }
        /// <summary>
        /// 字符串转日期
        /// </summary>
        /// <param name="s"></param>
        /// <param name="format">日期格式</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string s, string format = null)
        {
            if (format.IsNullOrEmpty())
                return DateTime.Parse(s);
            else
                return DateTime.ParseExact(s, format, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 字符串转换成byte数组
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encoding">默认是utf8格式</param>
        /// <returns></returns>
        public static byte[] ToByteArr(this string s,Encoding encoding=null)
        {
            if(encoding is null)
                encoding=Encoding.UTF8;
            return encoding.GetBytes(s);
        }
        /// <summary>
        /// byte数组转字符串
        /// </summary>
        /// <param name="b"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ByteToString(this byte[] b, Encoding encoding = null)
        {
            if (encoding is null)
                encoding = Encoding.UTF8;
            return encoding.GetString(b);
        }
        /// <summary>
        /// 字符串转枚举
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="s"></param>
        /// <param name="ignoreCase">是否忽略大小写，默认false</param>
        /// <returns></returns>
        public static TEnum ToEnum<TEnum>(this string s,bool ignoreCase=false) where TEnum:Enum
        {
            return (TEnum)Enum.Parse(typeof(TEnum), s, ignoreCase);
        }
        /// <summary>
        /// 删除微信昵称里的Emoji图片
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveEmoji(string str) {
            foreach (var a in str)
            {
                byte[] bts = Encoding.UTF32.GetBytes(a.ToString());

                if (bts[0].ToString() == "253" && bts[1].ToString() == "255")
                {
                    str = str.Replace(a.ToString(), "");
                }

            }
            return str;
        }
    }
}
