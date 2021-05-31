namespace System
{
    /// <summary>
    /// 日期扩展方法
    /// </summary>
    public static class DateTimeUtil
    {
        /// <summary>
        /// 格式化日期为yyyy-MM-dd
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToyyyyMMdd(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }
        /// <summary>
        /// 格式化日期为yyyy年MM月dd日
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToyyyyMMddCH(this DateTime dt)
        {
            return dt.ToString("yyyy年MM月dd日");
        }
        /// <summary>
        /// 格式化日期为yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToyyyyMMddHHmmss(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 格式化日期为yyyy年MM月dd日 HH:mm:ss
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToyyyyMMddHHmmssCH(this DateTime dt)
        {
            return dt.ToString("yyyy年MM月dd日 HH:mm:ss");
        }
        /// <summary>
        /// 获取当前unix时间戳(13位)
        /// </summary>
        /// <returns></returns>
        public static long GetTimestamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return ts.TotalMilliseconds.ToLong();
        }
        /// <summary>
        /// utc时间戳，转本地时间
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeByTimestamp(string timestamp)
        {
            DateTime dtTimestamp = TimeZoneInfo.ConvertTimeFromUtc(new System.DateTime(1970, 1, 1), TimeZoneInfo.Local);
            return dtTimestamp.AddMilliseconds(timestamp.ToLong());
        }
    }
}
