using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Common
{
    /// <summary>
    /// Socket帮助类(服务器端和客户端都用此)
    /// 消息头包括 4字节(int)内容长度+24字节真实ip(string)  共28字节
    /// </summary>
    public class SocketUtil : IDisposable
    {
        /// <summary>
        /// 消息头长度  包括4字节的内容长度  和24字节的真实ip
        /// </summary>
        private const int HeaderLength = 28;
        /// <summary>
        /// 连接的ip
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 连接的端口号
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// ip:port
        /// </summary>
        public string IpPort => Ip + ":" + Port;

        /// <summary>
        /// 客户端真实ip和port
        /// </summary>
        public string ClientRealIpPort => ClientRealIp + ":" + ClientRealPort;

        /// <summary>
        /// 客户端真实Ip
        /// </summary>
        public string ClientRealIp { get; set; }
        /// <summary>
        /// 客户端真实端口号
        /// </summary>
        public int ClientRealPort { get; set; }
        /// <summary>
        /// Socket主对象
        /// </summary>
        public Socket socketMain { get; set; }
        /// <summary>
        /// Socket客户端集合
        /// </summary>
        public Dictionary<string, SocketUtil> dicClient { get; set; }
        /// <summary>
        /// 接收消息的字节数组  默认长度是10Mb
        /// </summary>
        public byte[] buffer { get; set; }
        /// <summary>
        /// 缓冲字符长度默认是1024*1024*10  10Mb
        /// </summary>
        public int buffersize { get; set; } = 1024 * 1024 * 10;
        /// <summary>
        /// 真实数据的数组
        /// </summary>
        public List<byte> listByte { get; set; } = new List<byte>();
        /// <summary>
        /// 服务器收到消息执行方法
        /// byte[]  接收到的字节数组
        /// int 数组实际内容长度
        /// </summary>
        public Action<byte[], int, SocketUtil> OnMessage { get; set; }
        /// <summary>
        /// 服务器收到消息执行方法
        /// string  接收到的字符串
        /// </summary>
        public Action<string, SocketUtil> OnMessageString { get; set; }
        /// <summary>
        /// 接收消息发生异常
        /// </summary>
        public Action<Exception, SocketUtil> OnReceiveException { get; set; }
        /// <summary>
        /// 发送消息时的异常
        /// </summary>
        public Action<Exception, SocketUtil> OnSendException { get; set; }
        /// <summary>
        /// 服务器端接收到连接
        /// </summary>
        public Action<SocketUtil> OnAccept { get; set; }
        /// <summary>
        /// 关闭连接后执行
        /// </summary>
        public Action<SocketUtil> OnClose { get; set; }
        /// <summary>
        /// 发送消息后执行 int:消息的字节数
        /// </summary>
        public Action<int, SocketUtil> OnSent { get; set; }
        /// <summary>
        /// 接收数据真正的总长度
        /// </summary>
        private int numRealFileLength { get; set; } = 0;
        /// <summary>
        /// 是否是服务器端
        /// </summary>
        public bool IsServer { get; set; }
        /// <summary>
        /// 是否是异步接收数据，默认true
        /// </summary>
        public bool IsAsyncReceive { get; set; } = true;
        /// <summary>
        /// 构建socket帮助类对象，服务器与客户端都用这个
        /// </summary>
        /// <param name="ip">服务器：监听本机ip；客户端：服务器ip</param>
        /// <param name="port">服务器：监听本机端口；客户端：服务器端口</param>
        public SocketUtil(string ip, int port)
        {
            this.Ip = ip;
            this.Port = port;
        }
        /// <summary>
        /// 使用等待接收消息(仅限客户端使用)
        /// </summary>
        public bool UseWaitAccept { get; set; } = false;
        /// <summary>
        /// 构建服务器端Socket
        /// </summary>
        public void BuildServer()
        {
            IsServer = true;
            dicClient = new Dictionary<string, SocketUtil>();
            socketMain = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketMain.Bind(new IPEndPoint(IPAddress.Parse(this.Ip), this.Port));
            socketMain.Listen(0);
            socketMain.BeginAccept(new AsyncCallback(Accept), socketMain);
        }
        private void Accept(IAsyncResult ar)
        {
            //获取连接Socket 创建新的连接
            Socket myServer = ar.AsyncState as Socket;
            Socket client = myServer.EndAccept(ar);
            string ip_port = client.RemoteEndPoint.ToString();
            SocketUtil socketClient = new SocketUtil(ip_port.Split(':')[0], int.Parse(ip_port.Split(':')[1]));
            socketClient.socketMain = client;
            dicClient.Add(ip_port, socketClient);
            if (OnAccept != null)
                OnAccept(socketClient);
            socketClient.buffer = new byte[buffersize];
            if (IsAsyncReceive)
            {
                //异步接收
                socketClient.socketMain.BeginReceive(socketClient.buffer, 0, socketClient.buffersize, SocketFlags.None, new AsyncCallback(ReadCallback), socketClient);
            }
            else
            {
                //同步接收
                while (true)
                {
                    int count = socketClient.socketMain.Receive(socketClient.buffer, 0, socketClient.buffersize, SocketFlags.None);
                    ExecuteReceive(socketClient, count);
                }
                

            }
            myServer.BeginAccept(new AsyncCallback(Accept), myServer);//等待下一个连接
        }
        private void ReadCallback(IAsyncResult ar)
        {
            //获取并保存
            SocketUtil socketItem = ar.AsyncState as SocketUtil;
            try
            {
                if (!socketItem.socketMain.Connected)
                {
                    throw new Exception("连接已关闭");
                }
                int count = socketItem.socketMain.EndReceive(ar);
                if (count <= 0)
                {
                    throw new Exception("连接已关闭");
                }
                else
                {
                    ExecuteReceive(socketItem, count);
                }
                //接收完成 重新给出buffer接收
                socketItem.socketMain.BeginReceive(socketItem.buffer, 0, socketItem.buffersize, 0, new AsyncCallback(ReadCallback), socketItem);
            }
            catch (Exception e)
            {
                if (e.Message != "连接已关闭")
                {
                    if (OnReceiveException != null)
                        OnReceiveException(e, socketItem);
                }
                //Close(socketItem);
            }
        }
        /// <summary>
        /// 处理具体接收数据的方法
        /// </summary>
        /// <param name="socketItem"></param>
        /// <param name="count">接收数据字节长度</param>
        private bool ExecuteReceive(SocketUtil socketItem, int count)
        {
            int numLeftCount = 0;//接收多余的字节
            if (socketItem.numRealFileLength == 0)
            {
                socketItem.numRealFileLength = BitConverter.ToInt32(socketItem.buffer.Take(4).ToArray(), 0);
                if (socketItem.ClientRealIp.IsNullOrEmpty())
                {
                    string ip_client = Encoding.UTF8.GetString(socketItem.buffer.Skip(4).Take(24).ToArray()).TrimEnd('\0');//去掉空字节
                    socketItem.ClientRealIp = ip_client.Split(':')[0];
                    socketItem.ClientRealPort = int.Parse(ip_client.Split(':')[1]);
                }
                int numReceiveByte = count - HeaderLength;//本次应保存的字节
                if (numReceiveByte > socketItem.numRealFileLength)
                {
                    numLeftCount = numReceiveByte - socketItem.numRealFileLength;
                    numReceiveByte = socketItem.numRealFileLength;
                }
                socketItem.listByte.AddRange(socketItem.buffer.Skip(HeaderLength).Take(numReceiveByte));
            }
            else
            {
                int numReceiveByte = count;//本次应保存的字节
                if (socketItem.listByte.Count + numReceiveByte > socketItem.numRealFileLength)
                {
                    numReceiveByte = socketItem.numRealFileLength - socketItem.listByte.Count;
                    numLeftCount = count - numReceiveByte;
                }
                socketItem.listByte.AddRange(socketItem.buffer.Take(count));
            }
            //所有数据接收完成处理
            if (socketItem.numRealFileLength == socketItem.listByte.Count)
            {
                if(OnMessage!=null)
                    OnMessage(socketItem.listByte.ToArray(), socketItem.numRealFileLength, socketItem);
                if(OnMessageString!=null)
                {
                    string message = Encoding.UTF8.GetString(socketItem.listByte.ToArray());
                    OnMessageString(message, socketItem);
                }

                socketItem.listByte.Clear();
                if (numLeftCount > 0)
                {
                    byte[] temp = new byte[socketItem.buffersize];
                    Array.Copy(socketItem.buffer, socketItem.numRealFileLength + HeaderLength, temp, 0, numLeftCount);
                    socketItem.buffer = temp;
                    socketItem.numRealFileLength = 0;
                    ExecuteReceive(socketItem, numLeftCount);
                }
                else
                    socketItem.numRealFileLength = 0;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="socketItem"></param>
        public void Close(SocketUtil socketItem = null)
        {
            if (socketItem is null)
                socketItem = this;
            socketItem.socketMain.Close();
            if (dicClient != null && dicClient.ContainsKey(socketItem.IpPort))
                dicClient.Remove(socketItem.IpPort);
            if (OnClose != null)
                OnClose(socketItem);
        }
        /// <summary>
        /// 建立客户端
        /// </summary>
        public void BuildClient()
        {
            IsServer = false;
            buffer = new byte[buffersize];
            socketMain = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketMain.Connect(new IPEndPoint(IPAddress.Parse(this.Ip), this.Port));
            if(!UseWaitAccept)
                socketMain.BeginReceive(buffer, 0, buffersize, SocketFlags.None, ReadCallback, this);
        }
        /// <summary>
        /// 等待接收
        /// </summary>
        public void WaitAccept(Action<string, SocketUtil> _OnMessageString) 
        {
            OnMessageString = _OnMessageString;
            //同步接收
            while (true)
            {
                int count = socketMain.Receive(buffer, 0, buffersize, SocketFlags.None);
                if (ExecuteReceive(this, count))
                    break;
            }
        }
        /// <summary>
        /// 发送字符串，编码格式为utf-8
        /// </summary>
        /// <param name="data">发送的消息</param>
        /// <param name="socketItem">发送源socket对象，默认是当前socket</param>
        public void SendString(string data, SocketUtil socketItem = null)
        {
            Send(Encoding.UTF8.GetBytes(data),socketItem);
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="byteData">字节数组 自动添加下面信息 header:内容长度(int) 4字节；真实ip:port(string) 15字节</param>
        /// <param name="socketItem">发送源socket对象，默认是当前socket</param>
        public void Send(byte[] byteData, SocketUtil socketItem = null)
        {
            if (socketItem is null)
                socketItem = this;
            byte[] dataLenth = BitConverter.GetBytes(byteData.Length);//长度肯定为4
            byte[] ip_port = Encoding.UTF8.GetBytes(socketItem.socketMain.LocalEndPoint.ToString());//长度最大为24
            //最终发送的数据
            byte[] realSend = new byte[byteData.Length + HeaderLength];
            Array.Copy(dataLenth, 0, realSend, 0, dataLenth.Length);
            Array.Copy(ip_port, 0, realSend, 4, ip_port.Length);
            Array.Copy(byteData, 0, realSend, HeaderLength, byteData.Length);
            socketItem.socketMain.BeginSend(realSend, 0, realSend.Length, SocketFlags.None, new AsyncCallback(SendCallback), socketItem);
        }
        /// <summary>
        /// 发送数据后的方法
        /// </summary>
        /// <param name="ar"></param>
        private void SendCallback(IAsyncResult ar)
        {
            SocketUtil socketItem = (SocketUtil)ar.AsyncState;
            try
            {
                int bytesSent = socketItem.socketMain.EndSend(ar);
                if (OnSent != null)
                    OnSent(bytesSent, socketItem);
            }
            catch (Exception e)
            {
                if (OnSendException != null)
                    OnSendException(e, socketItem);
                //Close(socketItem);
            }
        }
        /// <summary>
        /// 释放连接
        /// </summary>
        public void Dispose()
        {
            if (this.socketMain.Connected)
                this.socketMain.Disconnect(false);
        }
    }

    /// <summary>
    /// Socket消息传递辅助类
    /// </summary>
    public class SocketMessage {
        /// <summary>
        /// 消息类型
        /// </summary>
        public string MessageType { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string MessageContent { get; set; }
    }

}
