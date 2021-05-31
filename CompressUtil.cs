using System.IO;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Writers;

namespace Common
{
    /// <summary>
    /// 压缩帮助类
    /// </summary>
    public static class CompressUtil
    {
        /// <summary>
        /// 解压缩  rar  zip  tar
        /// </summary>
        /// <param name="SourceFileName">源压缩文件路径</param>
        /// <param name="DesDirName">解压文件夹路径</param>
        /// <param name="Overwite">覆盖原文件</param>
        public static void UnCompressFile(string SourceFileName, string DesDirName, bool Overwite = false)
        {
            using (var archive = ArchiveFactory.Open(SourceFileName))
            {
                foreach (var entry in archive.Entries)
                {
                    if (!entry.IsDirectory)
                    {
                        entry.WriteToDirectory(DesDirName, new ExtractionOptions() { ExtractFullPath = true, Overwrite = Overwite });
                    }
                }
            }
        }
        /// <summary>
        /// 压缩文件 压缩成zip
        /// </summary>
        /// <param name="SourcePath">源文件路径，文件或文件夹路径</param>
        /// <param name="DestZipName">压缩文件路径.zip</param>
        /// <param name="OnlyContent">只压缩文件夹里的内容(源文件为文件夹才有效)</param>
        public static void CompressFile(string SourcePath, string DestZipName, bool OnlyContent = false)
        {
            using (var archive = ZipArchive.Create())
            {
                CompressAll(archive, new string[] { SourcePath }, OnlyContent);
                using (FileStream fs = new FileStream(DestZipName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    archive.SaveTo(fs, new WriterOptions(CompressionType.Deflate) {  CompressionType = CompressionType.LZip });
                }
            }
        }
        /// <summary>
        /// 压缩文件 压缩成zip
        /// </summary>
        /// <param name="SourcePath">源文件路径们，文件或文件夹路径</param>
        /// <param name="DestZipName">压缩文件路径.zip</param>
        public static void CompressFile(string[] SourcePath, string DestZipName)
        {
            using (var archive = ZipArchive.Create())
            {
                CompressAll(archive, SourcePath);
                using (FileStream fs = new FileStream(DestZipName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    archive.SaveTo(fs, new WriterOptions(CompressionType.Deflate) { CompressionType = CompressionType.LZip });
                }
            }
        }
        /// <summary>
        /// 压缩文件成zip，返回流
        /// </summary>
        /// <param name="SourcePath">源文件路径们，文件或文件夹路径</param>
        /// <param name="OnlyContent">只压缩文件夹里的内容(源文件为文件夹才有效)</param>
        /// <returns>内存流MemoryStream</returns>
        public static Stream CompressFile(string SourcePath, bool OnlyContent = false)
        {
            Stream s = new MemoryStream();
            using (var archive = ZipArchive.Create())
            {
                CompressAll(archive, new string[] { SourcePath }, OnlyContent);
                archive.SaveTo(s, new WriterOptions(CompressionType.Deflate) { CompressionType = CompressionType.LZip });
                s.Position = 0;
            }
            return s;
        }
        /// <summary>
        /// 压缩文件成zip，返回流
        /// </summary>
        /// <param name="SourcePath">源文件路径们，文件或文件夹路径</param>
        /// <returns>内存流MemoryStream</returns>
        public static Stream CompressFile(string[] SourcePath)
        {
            Stream s = new MemoryStream();
            using (var archive = ZipArchive.Create())
            {
                CompressAll(archive, SourcePath);
                archive.SaveTo(s, new WriterOptions(CompressionType.Deflate) { CompressionType = CompressionType.LZip });
                s.Position = 0;
            }
            return s;
        }
        #region 压缩主要方法（私有）
        /// <summary>
        /// 压缩主方法
        /// </summary>
        /// <param name="archive"></param>
        /// <param name="SourcePath"></param>
        /// <param name="OnlyContent">只压缩文件夹里的内容(源文件为文件夹才有效)</param>
        private static void CompressAll(ZipArchive archive, string[] SourcePath, bool? OnlyContent = null)
        {
            string strRootPath;//压缩根目录路径
            foreach (string fileName in SourcePath)
            {
                if (File.Exists(fileName))
                {
                    strRootPath = "";
                    CompressFileDetail(archive, new FileInfo(fileName), strRootPath);
                }
                else if (Directory.Exists(fileName))
                {
                    if (OnlyContent != null && OnlyContent == true)
                        strRootPath = "";
                    else
                        strRootPath = Path.GetFileNameWithoutExtension(fileName.TrimEnd('\\', '/')) + "/";
                    CompressDirDetail(archive, new DirectoryInfo(fileName), strRootPath);
                }
            }
        }
        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="archive"></param>
        /// <param name="dir"></param>
        /// <param name="rootPath"></param>
        private static void CompressDirDetail(ZipArchive archive, DirectoryInfo dir, string rootPath)
        {
            foreach (var di in dir.GetDirectories())
            {
                if (Directory.Exists(di.FullName))
                {
                    CompressDirDetail(archive, di, rootPath + di.Name + "/");
                }
            }
            foreach (var f in dir.GetFiles())
            {
                CompressFileDetail(archive, f, rootPath);
            }
        }
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="archive"></param>
        /// <param name="fi"></param>
        /// <param name="rootPath"></param>
        private static void CompressFileDetail(ZipArchive archive, FileInfo fi, string rootPath)
        {
            archive.AddEntry(rootPath + fi.Name, fi);//添加文件夹中的文件
        }
        #endregion
    }
}
