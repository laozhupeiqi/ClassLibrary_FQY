using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ClassLibrary_FQY.ServerSocket
{
    public class SocketManager_Free
    {
        private int m_maxConnectNum;    //最大连接数  
        private int m_revBufferSize;    //最大接收字节数  
        private BufferManager m_bufferManager;
        private const int opsToAlloc = 2;
        private Socket listenSocket;            //监听Socket  
        private SocketAsyncEventArgsPool m_pool;
        private int m_clientCount;              //连接的客户端数量  
        private Semaphore m_maxNumberAcceptedClients;
        private List<AsyncUserToken> m_clients; //客户端列表  

        #region 定义委托  

        /// <summary>  
        /// 客户端连接数量变化时触发  
        /// </summary>  
        /// <param name="num">当前增加客户的个数(用户退出时为负数,增加时为正数,一般为1)</param>  
        /// <param name="token">增加用户的信息</param>  
        public delegate void OnClientNumberChange(int num, AsyncUserToken token);

        /// <summary>  
        /// 接收到客户端的数据  
        /// </summary>  
        /// <param name="token">客户端</param>  
        /// <param name="buff">客户端数据</param>  
        public delegate void OnReceiveData(AsyncUserToken token, byte[] buff);

        #endregion

        #region 定义事件  
        /// <summary>  
        /// 客户端连接数量变化事件  
        /// </summary>  
        public event OnClientNumberChange ClientNumberChange;

        /// <summary>  
        /// 接收到客户端的数据事件  
        /// </summary>  
        public event OnReceiveData ReceiveClientData;

        #endregion

        #region 定义属性  

        /// <summary>  
        /// 获取客户端列表  
        /// </summary>  
        public List<AsyncUserToken> ClientList { get { return m_clients; } }

        #endregion

        /// <summary>  
        /// 构造函数  
        /// </summary>  
        /// <param name="numConnections">最大连接数</param>  
        /// <param name="receiveBufferSize">缓存区大小</param>  
        public SocketManager_Free(int numConnections, int receiveBufferSize)
        {
            m_clientCount = 0;
            m_maxConnectNum = numConnections;
            m_revBufferSize = receiveBufferSize;
            // 分配缓冲区，以便最大数量的套接字可以同时向套接字发送一个未完成的读写操作   
            m_bufferManager = new BufferManager(receiveBufferSize * numConnections * opsToAlloc, receiveBufferSize);

            m_pool = new SocketAsyncEventArgsPool(numConnections);
            m_maxNumberAcceptedClients = new Semaphore(numConnections, numConnections);
        }

        /// <summary>  
        /// 初始化  
        /// </summary>  
        public void Init()
        {
            // 分配一个大字节缓冲区，所有I/O操作都使用一个缓冲区.  这有助于防止记忆碎片
            m_bufferManager.InitBuffer();
            m_clients = new List<AsyncUserToken>();
            // 预先分配SocketAsyncEventArgs对象池
            SocketAsyncEventArgs readWriteEventArg;

            for (int i = 0; i < m_maxConnectNum; i++)
            {
                //预先分配一组可重用的SocketAsyncEventArgs
                readWriteEventArg = new SocketAsyncEventArgs();
                readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                readWriteEventArg.UserToken = new AsyncUserToken();

                // 将缓冲池中的字节缓冲区分配给SocketAsyncEventArg对象
                m_bufferManager.SetBuffer(readWriteEventArg);
                // 将SocketAsyncEventArg添加到池中
                m_pool.Push(readWriteEventArg);
            }
        }

        /// <summary>  
        /// 启动服务  
        /// </summary>  
        /// <param name="localEndPoint">服务器将在其上侦听连接请求的终结点</param>  
        public bool Start(IPEndPoint localEndPoint)
        {
            try
            {
                m_clients.Clear();
                //创建侦听传入连接的套接字
                listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listenSocket.Bind(localEndPoint);
                //100个连接启动服务器
                listenSocket.Listen(m_maxConnectNum);
                // 监听插座上的post接收
                StartAccept(null);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 开始接受来自客户端的连接请求的操作
        /// </summary>
        /// <param name="acceptEventArg">在服务器的侦听套接字上发出接受操作时要使用的上下文对象</param>
        public void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
            }
            else
            {
                // 必须清除套接字，因为正在重用上下文对象
                acceptEventArg.AcceptSocket = null;
            }

            m_maxNumberAcceptedClients.WaitOne();
            if (!listenSocket.AcceptAsync(acceptEventArg))
            {
                ProcessAccept(acceptEventArg);
            }
        }

        private void AcceptEventArg_Completed1(object sender, SocketAsyncEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>  
        /// 停止服务  
        /// </summary>  
        public void Stop()
        {
            foreach (AsyncUserToken token in m_clients)
            {
                try
                {
                    token.mySocket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception) { }
            }
            try
            {
                listenSocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception) { }

            listenSocket.Close();
            int c_count = m_clients.Count;
            lock (m_clients) { m_clients.Clear(); }

            if (ClientNumberChange != null)
                ClientNumberChange(-c_count, null);
        }

        public void CloseClient(AsyncUserToken token)
        {
            try
            {
                token.mySocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 此方法是与关联的回调方法套接字.sync操作，并在接受操作完成时调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            try
            {
                Interlocked.Increment(ref m_clientCount);
                //获取接受的客户端连接的套接字，并将其放入ReadEventArg对象用户令牌中 
                SocketAsyncEventArgs readEventArgs = m_pool.Pop();
                AsyncUserToken userToken = (AsyncUserToken)readEventArgs.UserToken;
                userToken.mySocket = e.AcceptSocket;
                userToken.ConnectTime = DateTime.Now;
                userToken.Remote = e.AcceptSocket.RemoteEndPoint;
                userToken.ip_Client = ((IPEndPoint)(e.AcceptSocket.RemoteEndPoint)).Address;

                lock (m_clients) { m_clients.Add(userToken); }

                if (ClientNumberChange != null)
                    ClientNumberChange(1, userToken);
                if (!e.AcceptSocket.ReceiveAsync(readEventArgs))
                {
                    ProcessReceive(readEventArgs);
                }
            }
            catch
            {

            }

            // 接受下一个连接请求
            if (e.SocketError == SocketError.OperationAborted) return;
            StartAccept(e);
        }

        /// <summary>
        /// 每当套接字上的接收或发送操作完成时，就会调用此方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">与已完成的接收操作关联的SocketAsyncEventArg</param>
        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            // 确定刚刚完成的操作类型并调用关联的处理程序
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }
        }

        /// <summary>
        /// 此方法在异步接收操作完成时调用
        /// 如果远程主机关闭了连接，则套接字将关闭。
        /// 如果接收到数据，则将数据回显到客户端。
        /// </summary>
        /// <param name="e"></param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            try
            {
                // 检查远程主机是否关闭了连接
                AsyncUserToken token = (AsyncUserToken)e.UserToken;
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    //读取数据  
                    byte[] data = new byte[e.BytesTransferred];
                    Array.Copy(e.Buffer, e.Offset, data, 0, e.BytesTransferred);
                    lock (token.Buffer)
                    {
                        token.Buffer.AddRange(data);
                    }

                    do
                    {  
                        //包够长时,则提取出来,交给后面的程序去处理
                        byte[] rev = token.Buffer.GetRange(0, token.Buffer.Count).ToArray();
                        //从数据池中移除这组数据
                        lock (token.Buffer)
                        {
                            token.Buffer.RemoveRange(0, token.Buffer.Count);
                        }

                        //将数据包交给后台处理,这里你也可以新开个线程来处理.加快速度.  
                        if (ReceiveClientData != null)
                            ReceiveClientData(token, rev);
                        //这里API处理完后,并没有返回结果,当然结果是要返回的,却不是在这里, 这里的代码只管接收.  
                        //若要返回结果,可在API处理中调用此类对象的SendMessage方法,统一打包发送.不要被微软的示例给迷惑了.  
                    } while (token.Buffer.Count > 2);

                    //继续接收. 为什么要这么写,请看Socket.ReceiveAsync方法的说明  
                    if (!token.mySocket.ReceiveAsync(e))
                        this.ProcessReceive(e);
                }
                else
                {
                    CloseClientSocket(e);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 此方法在异步发送操作完成时调用。
        /// 该方法在套接字上发出另一个receive，以读取从客户端发送的任何附加数据
        /// </summary>
        /// <param name="e"></param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                // 已将数据回显到客户端
                AsyncUserToken token = (AsyncUserToken)e.UserToken;
                // 读取从客户端发送的下一个数据块
                bool willRaiseEvent = token.mySocket.ReceiveAsync(e);
                if (!willRaiseEvent)
                {
                    ProcessReceive(e);
                }
            }
            else
            {
                CloseClientSocket(e);
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="e"></param>
        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = e.UserToken as AsyncUserToken;

            lock (m_clients) { m_clients.Remove(token); }
            //如果有事件,则调用事件,发送客户端数量变化通知  
            if (ClientNumberChange != null)
                ClientNumberChange(-1, token);
            // close the socket associated with the client  
            try
            {
                token.mySocket.Shutdown(SocketShutdown.Send);
            }
            catch (Exception) { }
            token.mySocket.Close();
            // decrement the counter keeping track of the total number of clients connected to the server  
            Interlocked.Decrement(ref m_clientCount);
            m_maxNumberAcceptedClients.Release();
            // Free the SocketAsyncEventArg so they can be reused by another client  
            e.UserToken = new AsyncUserToken();
            m_pool.Push(e);
        }

        /// <summary>  
        /// 对数据进行打包,然后再发送  
        /// </summary>  
        /// <param name="token"></param>  
        /// <param name="message"></param>  
        /// <returns></returns>  
        public void SendMessage(AsyncUserToken token, byte[] message)
        {
            if (token == null || token.mySocket == null || !token.mySocket.Connected)
                return;
            try
            {
                //新建异步发送对象, 发送消息  
                SocketAsyncEventArgs sendArg = new SocketAsyncEventArgs();
                sendArg.UserToken = token;
                sendArg.SetBuffer(message, 0, message.Length);  //将数据放置进去.  
                token.mySocket.SendAsync(sendArg);
            }
            catch
            {

            }
        }


    }
}
