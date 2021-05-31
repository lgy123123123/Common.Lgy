using System;
using Common.CustomException;
using Renci.SshNet;

namespace Common
{
    /// <summary>
    /// ssh执行帮助类
    /// </summary>
    public class SshUtil : IDisposable
    {
        /// <summary>
        /// ssh客户端对象
        /// </summary>
        public SshClient client { get; set; }
        /// <summary>
        /// 保持连接时长
        /// </summary>
        public TimeSpan KeepAliveInterval
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
        public SshUtil(string ip, int port, string username, string password)
        {
            client = new SshClient(ip, port, username, password);
        }
        /// <summary>
        /// 连接
        /// </summary>
        public void Connect()
        {
            if (client.IsConnected)
                return;
            else
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
        /// 执行命令，并返回结果
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="sourceProfile">命令前是否加上source /etc/profile，默认是false</param>
        /// <returns></returns>
        public string ExecuteCmd(string cmdText,bool sourceProfile=false)
        {
            Connect();
            cmdText = (sourceProfile ? "source /etc/profile;" : "") + cmdText;
            SshCommand cmd = client.RunCommand(cmdText);
            if (!cmd.Error.IsNullOrEmpty())
            {
                throw new BaseException(cmd.Error);
            }
            return cmd.Result;
        }
        /// <summary>
        /// 执行命令，并返回SshCommand，自行处理结果
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="sourceProfile">命令前是否加上source /etc/profile，默认是false</param>
        /// <returns></returns>
        public SshCommand ExecuteCmdRetCmd(string cmdText, bool sourceProfile = false)
        {
            Connect();
            cmdText = (sourceProfile ? "source /etc/profile;" : "") + cmdText;
            SshCommand cmd = client.RunCommand(cmdText);
            return cmd;
        }
    }
}
