using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ClassLibrary_FQY.ServerSocket
{
    public class AsyncUserToken
    {
        /// <summary>
        /// 客户端IP地址
        /// </summary>
        public IPAddress ip_Client { get; set; }
        /// <summary>
        /// 远程地址
        /// </summary>
        public EndPoint Remote { get; set; }
        /// <summary>
        /// 通信SOKET
        /// </summary>
        public Socket mySocket { get; set; }
        /// <summary>
        /// 连接时间
        /// </summary>
        public DateTime ConnectTime { get; set; }
        /// <summary>
        /// 数据缓存区
        /// </summary>
        public List<byte> Buffer { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public AsyncUserToken()
        {
            Buffer = new List<byte>();
        }
    }
}
