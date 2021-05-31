using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace Common
{
    /// <summary>
    /// sftp文件帮助类
    /// </summary>
    public class SftpUtil : IDisposable
    {
        /// <summary>
        /// sftp客户端对象
        /// </summary>
        public SftpClient client { get; set; }
        /// <summary>
        /// 保持连接时长
        /// </summary>
        public TimeSpan timeKeepAlive
        {
            get => client.KeepAliveInterval;
            set => client.KeepAliveInterval = value;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip">远程连接ip</param>
        /// <param name="port">端口号</param>
        /// <param name="username">登录名</param>
        /// <param name="password">密码</param>
        public SftpUtil(string ip, int port, string username, string password)
        {
            client = new SftpClient(ip, port, username, password);
        }
        /// <summary>
        /// 连接
        /// </summary>
        public void Connect()
        {
            if(!client.IsConnected)
                client.Connect();
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        public void DisConnect()
        {
            if (client.IsConnected)
                client.Disconnect();
        }
        /// <summary>
        /// 释放当前对象
        /// </summary>
        public void Dispose()
        {
            client.Dispose();
        }
        /// <summary>
        /// 获取文件夹或文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public SftpFile Get(string path)
        {
            Connect();
            return client.Get(path);
        }
        /// <summary>
        /// 获取远程文件夹列表
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public List<SftpFile> GetDirList(string path,Action<int> callback=null)
        {
            return client.ListDirectory(path, callback).ToList();
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="input">文件流</param>
        /// <param name="path">目的路径</param>
        /// <param name="canOverride">可否覆盖，默认true</param>
        /// <param name="callback">上传完回调函数</param>
        public void Upload(Stream input,string path,bool canOverride=true,Action<ulong> callback=null)
        {
            Connect();
            client.UploadFile(input, path, canOverride, callback);
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="sourcePath">源路径</param>
        /// <param name="destPath">目的路径</param>
        /// <param name="canOverride">可否覆盖，默认true</param>
        /// <param name="callback">上传完回调函数</param>
        public void Upload(string sourcePath, string destPath, bool canOverride = true, Action<ulong> callback = null)
        {
            Connect();
            using (FileStream fs = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                client.UploadFile(fs, destPath, canOverride, callback);
            }
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="output">获取的文件流</param>
        /// <param name="callback">回调函数</param>
        public void DownloadFile(string path,Stream output,Action<ulong> callback=null)
        {
            Connect();
            client.DownloadFile(path, output, callback);
        }
    }
}
