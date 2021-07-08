using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Common.CustomException;

namespace Common
{
    /// <summary>
    /// 记录日志，输出到文件
    /// </summary>
    public static class LogUtil
    {
        #region 公有方法和属性
        #region 是否输出到控制台
        /// <summary>
        /// Info类型日志,输出到控制台，默认false
        /// </summary>
        public static bool PrintInfo = false;
        /// <summary>
        /// Debug类型日志,输出到控制台，默认false
        /// </summary>
        public static bool PrintDebug = false;
        /// <summary>
        /// Error类型日志,输出到控制台，默认false
        /// </summary>
        public static bool PrintError = false;
        /// <summary>
        /// Warning类型日志,输出到控制台，默认false
        /// </summary>
        public static bool PrintWarning = false;
        #endregion

        #region 是否输出到文件
        /// <summary>
        /// 是否输出Info日志，默认true
        /// </summary>
        public static bool WriteInfo = true;
        /// <summary>
        /// 是否输出Debug日志，默认true
        /// </summary>
        public static bool WriteDebug = true;
        /// <summary>
        /// 是否输出Error日志，默认true
        /// </summary>
        public static bool WriteError = true;
        /// <summary>
        /// 是否输出Warning日志，默认true
        /// </summary>
        public static bool WriteWarning = true;
        #endregion

        #region 添加Info日志
        /// <summary>
        /// 添加日志，保存为 Log_Info_日期
        /// </summary>
        /// <param name="strContent">日志内容</param>
        /// <param name="e">异常类传入则记录堆栈信息</param>
        public static void AddLog(string strContent, Exception e = null)
        {
            AddLog(strContent, "Log", e);
        }
        /// <summary>
        /// 添加日志，保存为 Log_Info_日期
        /// </summary>
        /// <param name="strContent">日志内容</param>
        /// <param name="strPerfixName">日志文件名前缀,默认Log</param>
        /// <param name="e">异常类传入则记录堆栈信息</param>
        public static void AddLog(string strContent, string strPerfixName, Exception e = null)
        {
            AddLog(strContent, strPerfixName, "", e);
        }
        /// <summary>
        /// 添加日志，保存为 Log_Info_日期
        /// 在Log下，还可以继续创建文件夹
        /// </summary>
        /// <param name="strContent">日志内容</param>
        /// <param name="strPerfixName">日志文件名前缀,默认Log</param>
        /// <param name="strDirName">文件夹名称，多级目录格式为:name1/name2/name3</param>
        /// <param name="e">异常类传入则记录堆栈信息</param>
        public static void AddLog(string strContent, string strPerfixName, string strDirName, Exception e = null)
        {
            AddLog(strContent, strPerfixName, strDirName, LogLevel.Info, e);
        }
        #endregion
        #region 添加Debug日志
        /// <summary>
        /// 添加日志，保存为 Log_Debug_日期
        /// </summary>
        /// <param name="strContent">日志内容</param>
        /// <param name="e">异常类传入则记录堆栈信息</param>
        public static void AddDebugLog(string strContent, Exception e = null)
        {
            AddDebugLog(strContent, "Log", e);
        }
        /// <summary>
        /// 添加日志，保存为 Log_Debug_日期
        /// </summary>
        /// <param name="strContent">日志内容</param>
        /// <param name="strPerfixName">日志文件名前缀,默认Log</param>
        /// <param name="e">异常类传入则记录堆栈信息</param>
        public static void AddDebugLog(string strContent, string strPerfixName, Exception e = null)
        {
            AddDebugLog(strContent, strPerfixName, "", e);
        }
        /// <summary>
        /// 添加日志，保存为 Log_Debug_日期
        /// 在Log下，还可以继续创建文件夹
        /// </summary>
        /// <param name="strContent">日志内容</param>
        /// <param name="strPerfixName">日志文件名前缀,默认Log</param>
        /// <param name="strDirName">文件夹名称，多级目录格式为:name1/name2/name3</param>
        /// <param name="e">异常类传入则记录堆栈信息</param>
        public static void AddDebugLog(string strContent, string strPerfixName, string strDirName, Exception e = null)
        {
            AddLog(strContent, strPerfixName, strDirName, LogLevel.Debug, e);
        }
        #endregion
        #region 添加Error日志
        /// <summary>
        /// 添加日志，保存为 Log_Error_日期
        /// </summary>
        /// <param name="strContent">日志内容</param>
        /// <param name="e">异常类传入则记录堆栈信息</param>
        public static void AddErrorLog(string strContent, Exception e = null)
        {
            AddErrorLog(strContent, "Log", e);
        }
        /// <summary>
        /// 添加日志，保存为 Log_Error_日期
        /// </summary>
        /// <param name="strContent">日志内容</param>
        /// <param name="strPerfixName">日志文件名前缀,默认Log</param>
        /// <param name="e">异常类传入则记录堆栈信息</param>
        public static void AddErrorLog(string strContent, string strPerfixName, Exception e = null)
        {
            AddErrorLog(strContent, strPerfixName, "", e);
        }
        /// <summary>
        /// 添加日志，保存为 Log_Error_日期
        /// 在Log下，还可以继续创建文件夹
        /// </summary>
        /// <param name="strContent">日志内容</param>
        /// <param name="strPerfixName">日志文件名前缀,默认Log</param>
        /// <param name="strDirName">文件夹名称，多级目录格式为:name1/name2/name3</param>
        /// <param name="e">异常类传入则记录堆栈信息</param>
        public static void AddErrorLog(string strContent, string strPerfixName, string strDirName, Exception e = null)
        {
            AddLog(strContent, strPerfixName, strDirName, LogLevel.Error, e);
        }
        #endregion
        #region 添加Warning日志
        /// <summary>
        /// 添加日志，保存为 Log_Warning_日期
        /// </summary>
        /// <param name="strContent">日志内容</param>
        /// <param name="e">异常类传入则记录堆栈信息</param>
        public static void AddWarningLog(string strContent, Exception e = null)
        {
            AddWarningLog(strContent, "Log", e);
        }
        /// <summary>
        /// 添加日志，保存为 Log_Warning_日期
        /// </summary>
        /// <param name="strContent">日志内容</param>
        /// <param name="strPerfixName">日志文件名前缀,默认Log</param>
        /// <param name="e">异常类传入则记录堆栈信息</param>
        public static void AddWarningLog(string strContent, string strPerfixName, Exception e = null)
        {
            AddWarningLog(strContent, strPerfixName, "", e);
        }
        /// <summary>
        /// 添加日志，保存为 Log_Warning_日期
        /// 在Log下，还可以继续创建文件夹
        /// </summary>
        /// <param name="strContent">日志内容</param>
        /// <param name="strPerfixName">日志文件名前缀,默认Log</param>
        /// <param name="strDirName">文件夹名称，多级目录格式为:name1/name2/name3</param>
        /// <param name="e">异常类传入则记录堆栈信息</param>
        public static void AddWarningLog(string strContent, string strPerfixName, string strDirName, Exception e = null)
        {
            AddLog(strContent, strPerfixName, strDirName, LogLevel.Warning, e);
        }
        #endregion

        #region 其他操作日志方法
        /// <summary>
        /// 启动定时删除日志，默认每天0天执行删除。
        /// 例如25日0点，保留3天，则保留22、23、24、25日，22日之前的删除
        /// </summary>
        /// <param name="days">保留日志天数</param>
        /// <param name="startHour">启动删除时间，小时，默认0</param>
        /// <param name="startMinute">启动删除时间，分钟，默认0</param>
        /// <param name="startSecond">启动删除时间，秒，默认0</param>
        public static void StartDelLogTiming(int days, int startHour = 0, int startMinute = 0, int startSecond = 0)
        {
            if (days <= 0)
                throw new Exception("定时删除日志的保留天数，必须大于0");
            CronUtil.AddCronJob("定时删日志", $"{startHour} {startMinute} {startSecond} * * ? *", () =>
            {
                DateTime dtDelBefore = DateTime.Now.Date.AddDays(0 - days);
                DelLog(AppDomain.CurrentDomain.BaseDirectory + "Log/", dtDelBefore);
            }, true);
        }
        /// <summary>
        /// 删除日志
        /// </summary>
        /// <param name="LogPath">日志路径</param>
        /// <param name="dtDelBefore">删除这个时间之前的所有日志(不包括这个时间)</param>
        public static void DelLog(string LogPath, DateTime dtDelBefore)
        {
            if (LogPath.IsNullOrEmpty() || File.Exists(LogPath))
                throw new BaseException("日志路径不正确");
            Action<string> DelLog = null;//删除日志的方法
            DelLog = (path) =>
            {
                if (Directory.Exists(path))
                {
                    foreach (var dir in Directory.GetDirectories(path))
                    {
                        DelLog(dir);
                    }
                    foreach (var fPath in Directory.GetFiles(path))
                    {
                        DelLog(fPath);
                    }
                }
                else if (File.Exists(path))
                {
                    //日期字符串
                    string strDate = Path.GetFileNameWithoutExtension(path);
                    strDate = strDate.Substring(strDate.LastIndexOf("_") + 1);
                    DateTime dtFile = strDate.ToDateTime("yyyyMMdd");
                    //删除设定天数之前的日志
                    if (dtFile < dtDelBefore)
                    {
                        File.Delete(path);
                        AddLog($"删除日志：{path}");
                    }
                }
            };
            DelLog(LogPath);
        }
        /// <summary>
        /// 获取日志文件全路径
        /// </summary>
        /// <param name="level">日志等级，不填写是所有等级</param>
        /// <param name="logDir">日志所在文件夹，不填写是所有文件夹</param>
        /// <param name="logPerfix">日志文件前缀，不填写是所有文件</param>
        /// <returns></returns>
        public static List<string> GetAllLogFileName(LogLevel? level = null, string logDir = null, string logPerfix = null)
        {
            var allFile = FileUtil.GetAllFiles(AppDomain.CurrentDomain.BaseDirectory + "Log");
            if (level == null && logDir.IsNullOrEmptyTrim() && logPerfix.IsNullOrEmptyTrim())
                return allFile;
            List<string> listRet = new List<string>(allFile.Count);
            foreach (string f in allFile)
            {
                if (level != null && !f.Contains($"_{level.Value.ToString()}_"))
                    continue;

                if (logDir.IsNotNullOrEmpty() && !f.Contains($"/{logDir}/"))
                    continue;
                if (logPerfix.IsNotNullOrEmpty() && !f.StartsWith(logPerfix))
                    continue;
                listRet.Add(f);
            }
            return listRet;
        }
        /// <summary>
        /// 根据日志路径，获取日志内容
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<LogRead> ReadLog(string filePath)
        {
            List<LogRead> listRet = new List<LogRead>();
            string strLog = FileUtil.ReadFileShare(filePath);
            if (strLog.IsNullOrEmpty())
                return listRet;
            var logList = strLog.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
            LogRead logRet = null;
            StringBuilder sb = new StringBuilder();
            logList.ForEach(s =>
            {
                if (s.StartsWith("---2"))//每个日志开头，都是日期，匹配到日期才new对象
                {
                    addToList();//将上次日志结果加入集合，要开始下次的日志了
                    logRet = new LogRead();
                    logRet.LogDate = s.Replace("---", "").ToDateTime("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    sb.AppendLine(s);
                }
            });

            //将日志对象加入列表的方法
            void addToList()
            {
                if (logRet != null)
                {
                    logRet.Content = sb.ToString();
                    sb.Clear();
                    listRet.Add(logRet);
                }
            }

            addToList();//将最后一个匹配的结果，加入集合
            return listRet;
        }
        #endregion


        #endregion

        #region 日志操作，私有方法和属性
        /// <summary>
        /// 添加日志根方法
        /// </summary>
        /// <param name="strContent">日志内容</param>
        /// <param name="strFileName">文件名</param>
        /// <param name="strDirName">文件夹名</param>
        /// <param name="level">日志等级</param>
        /// <param name="e">异常信息</param>
        private static void AddLog(string strContent, string strFileName, string strDirName, LogLevel level, Exception e)
        {
            if (e != null)
            {
                strContent = $"错误信息：{strContent}\r\n堆栈信息：{e.Message}\r\n{e.StackTrace}";
            }
            strFileName = strFileName.IsNullOrEmpty() ? "Log" : strFileName + "_" + level.ToString();
            if (NeedPrint(level))
                Console.WriteLine(strContent);
            if (NeedWriteFile(level))
            {
                var detail = dicLogMission.GetOrAdd(strDirName + strFileName, (key) => new LogDetail(strFileName, strDirName, level));
                detail.AddLogQueue(strContent);
            }
        }
        /// <summary>
        /// 根据日志等级，获取是否打印
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private static bool NeedPrint(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug: return PrintDebug;
                case LogLevel.Error: return PrintError;
                case LogLevel.Info: return PrintInfo;
                case LogLevel.Warning: return PrintWarning;
            }
            return true;
        }
        /// <summary>
        /// 根据日志等级，获取是否输出到文件
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private static bool NeedWriteFile(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug: return WriteDebug;
                case LogLevel.Error: return WriteError;
                case LogLevel.Info: return WriteInfo;
                case LogLevel.Warning: return WriteWarning;
            }
            return true;
        }
        /// <summary>
        /// 每个日志文件都是一个集合，每个文件都有自己的线程存日志
        /// </summary>
        private static ConcurrentDictionary<string, LogDetail> dicLogMission = new ConcurrentDictionary<string, LogDetail>();
        /// <summary>
        /// 日志操作详情类
        /// </summary>
        private class LogDetail
        {
            /// <summary>
            /// 日志文件名
            /// </summary>
            string FileName { get; set; }
            /// <summary>
            /// 日志所在文件夹
            /// </summary>
            string DirName { get; set; }
            /// <summary>
            /// 日志等级
            /// </summary>
            LogLevel Level { get; set; }

            public LogDetail(string strFileName, string strDirName, LogLevel level)
            {
                FileName = strFileName;
                DirName = strDirName;
                Level = level;
            }
            /// <summary>
            /// 日志队列
            /// </summary>
            private ConcurrentQueue<LogModel> qList { get; set; } = new ConcurrentQueue<LogModel>();
            /// <summary>
            /// 是否已经停止写日志
            /// </summary>
            bool IsStop = true;
            /// <summary>
            /// 添加到日志队列
            /// </summary>
            /// <param name="strContent"></param>
            public void AddLogQueue(string strContent)
            {
                qList.Enqueue(new LogModel { LogContent = strContent, HappenTime = DateTime.Now });
                //每次添加完，都进行日志输出
                WriteLog();
            }
            /// <summary>
            /// 开始写日志
            /// </summary>
            private void WriteLog()
            {
                if (IsStop)
                {
                    IsStop = false;
                    if (qList.Count > 0)
                    {
                        new Thread(() =>
                        {
                            if (DirName.IsNotNullOrEmpty() && !DirName.EndsWith("/"))
                                DirName += "/";
                            var strFold = AppDomain.CurrentDomain.BaseDirectory + "Log/" + DirName;
                            if (!Directory.Exists(strFold)) Directory.CreateDirectory(strFold);
                            var strFile = strFold + FileName + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                            using (var fs = new FileStream(strFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                            {
                                using (var sw = new StreamWriter(fs))
                                {
                                    while (!qList.IsEmpty)
                                    {
                                        var isHaveData = qList.TryDequeue(out var log);
                                        if (isHaveData)
                                            sw.Write($"---{log.HappenTime:yyyy-MM-dd HH:mm:ss}---\r\n{log.LogContent}\r\n");
                                    }
                                }
                            }
                            IsStop = true;
                        })
                        { IsBackground = false }.Start();//前台线程，防止进程结束，日志没记录
                    }
                }
            }
            /// <summary>
            /// 日志模型
            /// </summary>
            private struct LogModel
            {
                /// <summary>
                /// 日志内容
                /// </summary>
                public string LogContent { get; set; }
                /// <summary>
                /// 消息发生时间
                /// </summary>
                public DateTime HappenTime { get; set; }
            }
        }
        #endregion
    }
    /// <summary>
    /// 日志等级
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 普通输出信息
        /// </summary>
        Info,
        /// <summary>
        /// 调试信息
        /// </summary>
        Debug,
        /// <summary>
        /// 异常信息
        /// </summary>
        Error,
        /// <summary>
        /// 警告
        /// </summary>
        Warning
    }
    /// <summary>
    /// 读取日志类
    /// </summary>
    public class LogRead
    {
        /// <summary>
        /// 日志时间
        /// </summary>
        public DateTime LogDate { get; set; }
        /// <summary>
        /// 日志内容
        /// </summary>
        public string Content { get; set; }
    }
}