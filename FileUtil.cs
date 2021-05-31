using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    /// <summary>
    /// 文件工具
    /// </summary>
    public static class FileUtil
    {
        /// <summary>
        /// 获取文件md5值
        /// </summary>
        /// <param name="path"></param>
        /// <returns>md5字符串</returns>
        public static string GetMD5(string path)
        {
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retval = md5.ComputeHash(file);
            file.Close();

            StringBuilder sc = new StringBuilder();
            for (int i = 0; i < retval.Length; i++)
            {
                sc.Append(retval[i].ToString("x2"));
            }
            return sc.ToString();
        }
        /// <summary>
        /// 获取文件哈希值
        /// </summary>
        /// <param name="path"></param>
        /// <returns>hash码字符串</returns>
        public static string GetSHA1(string path)
        {
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] retval = sha1.ComputeHash(file);
            file.Close();

            StringBuilder sc = new StringBuilder();
            for (int i = 0; i < retval.Length; i++)
            {
                sc.Append(retval[i].ToString("x2"));
            }
            return sc.ToString();
        }
        /// <summary>
        /// 监视文件/文件夹改变，包括子文件夹的内容
        /// </summary>
        /// <param name="DirectoryPath">监视的文件夹</param>
        /// <param name="ChangeAction">文件修改时执行的方法(改变文件的全路径,文件改变事件类型)</param>
        /// <param name="Filter">文件过滤，可以是某个文件名a.txt，也可以是通配符*.txt，默认是*.*</param>
        /// <param name="RenameAction">重命名时执行的方法</param>
        /// <returns></returns>
        public static void WatchFileChanged(string DirectoryPath, Action<FileSystemEventArgs> ChangeAction=null, string Filter = "*.*", Action<RenamedEventArgs> RenameAction=null)
        {
            FileSystemWatcher watch = new FileSystemWatcher(DirectoryPath, Filter);
            watch.BeginInit();
            watch.EnableRaisingEvents = true;
            watch.Created += new FileSystemEventHandler((sender, e) => { if(ChangeAction!=null) ChangeAction(e); });
            watch.Changed += new FileSystemEventHandler((sender, e) => { if (ChangeAction != null) ChangeAction(e); });
            watch.Deleted += new FileSystemEventHandler((sender, e) => { if (ChangeAction != null) ChangeAction(e); });
            watch.Renamed += new RenamedEventHandler((sender, e) => { if (RenameAction != null) RenameAction(e); });
            watch.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess
                                  | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
            watch.IncludeSubdirectories = true;
            watch.EndInit();
        }

        /// <summary>
        /// 复制文件夹或文件
        /// </summary>
        /// <param name="SourcePath">源路径</param>
        /// <param name="DestinationPath">目的路径</param>
        /// <param name="overwriteexisting">是否覆盖</param>
        public static void Copy(string SourcePath, string DestinationPath, bool overwriteexisting)
        {
            bool isFile = !Directory.Exists(SourcePath);
            //复制文件
            if (isFile)
            {
                File.Copy(SourcePath, DestinationPath, overwriteexisting);
                return;
            }
            //文件夹名最后必须有左斜杠或右斜杠
            if (!SourcePath.EndsWith(@"\") && !SourcePath.EndsWith("/"))
                SourcePath += "/";
            if (!DestinationPath.EndsWith(@"\") && !DestinationPath.EndsWith("/"))
                DestinationPath += "/";
            //复制文件夹
            if (Directory.Exists(SourcePath))
            {
                if (Directory.Exists(DestinationPath) == false)
                    Directory.CreateDirectory(DestinationPath);

                foreach (string fls in Directory.GetFiles(SourcePath))
                {
                    FileInfo flinfo = new FileInfo(fls);
                    flinfo.CopyTo(DestinationPath + flinfo.Name, overwriteexisting);
                }
                foreach (string drs in Directory.GetDirectories(SourcePath))
                {
                    DirectoryInfo drinfo = new DirectoryInfo(drs);
                    Copy(drs, DestinationPath + drinfo.Name, overwriteexisting);
                }
            }
        }
        /// <summary>
        /// 非独占方式读取文件(文件需要有读权限)
        /// </summary>
        /// <param name="path">文件全路径</param>
        /// <param name="encoding">编码格式 默认是utf8格式</param>
        /// <returns>文件内容</returns>
        public static string ReadFileShare(string path, Encoding encoding = null)
        {
            string ret = "";
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (encoding is null)
                        encoding = Encoding.UTF8;
                    using (StreamReader sr = new StreamReader(fs, encoding))
                    {
                        ret = sr.ReadToEnd();
                    }
                }
            }
            return ret;
        }
        /// <summary>
        /// 获取文件夹中的文件全路径
        /// </summary>
        /// <param name="dir">文件夹路径</param>
        /// <param name="deep">深度查找，是否包括文件夹中的文件</param>
        /// <returns>文件绝对路径集合</returns>
        public static List<string> GetAllFiles(string dir, bool deep = true)
        {
            List<string> ret = new List<string>();
            return GetAllFilesDeep(dir, deep, ret);
        }
        /// <summary>
        /// 递归获取文件名
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="deep">深度查找，是否包括文件夹中的文件</param>
        /// <param name="ret">文件绝对路径集合</param>
        /// <returns>文件绝对路径集合</returns>
        private static List<string> GetAllFilesDeep(string path, bool deep, List<string> ret)
        {
            if (File.Exists(path))
                ret.Add(path);
            else if (Directory.Exists(path))
            {
                foreach (string f in Directory.GetFiles(path))
                {
                    GetAllFilesDeep(f, deep, ret);
                }
                foreach (string dir in Directory.GetDirectories(path))
                {
                    GetAllFilesDeep(dir, deep, ret);
                }
            }
            return ret;
        }

        #region 文件编码格式 https://www.cnblogs.com/cyberarmy/p/5652835.html
        /// <summary>
        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型
        /// </summary>
        /// <param name="FILE_NAME">文件路径</param>
        /// <returns>文件的编码类型</returns>
        public static System.Text.Encoding GetType(string FILE_NAME)
        {
            FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
            Encoding r = GetType(fs);
            fs.Close();
            return r;
        }

        /// <summary>
        /// 通过给定的文件流，判断文件的编码类型
        /// </summary>
        /// <param name="fs">文件流</param>
        /// <returns>文件的编码类型</returns>
        public static System.Text.Encoding GetType(FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
            Encoding reVal = Encoding.Default;

            BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
            int.TryParse(fs.Length.ToString(), out var i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            r.Close();
            return reVal;

        }

        /// <summary>
        /// 判断是否是不带 BOM 的 UTF8 格式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1; //计算当前正分析的字符应还有的字节数
            for (int i = 0; i < data.Length; i++)
            {
                var curByte = data[i]; //当前分析的字节.
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }
        #endregion
    }
}
