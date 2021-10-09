using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClassLibrary_FQY.ClientSocket
{
    public class AsyncClient
    {
        //发送计数
        private int bytesSend = 0;


        //定义委托
        public delegate void OnReceiveData(byte[] buff);


        //定义事件
        public event OnReceiveData ReceiveSeverData;


        /// <summary>
        /// 开始异步连接服务器
        /// </summary>
        /// <param name="ip">服务器ip</param>
        /// <param name="port">服务器端口</param>
        public void StartClient(string ip, int port)
        {
            try
            {
                //为套接字建立远程端点
                IPAddress iPAddress = IPAddress.Parse(ip);
                IPEndPoint remoteEP = new IPEndPoint(iPAddress, port);

                // Create a TCP/IP socket.  
                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);

            }
            catch
            {


            }
        }
        /// <summary>
        /// 异步连接回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;
                // Complete the connection.  
                client.EndConnect(ar);
                if (ar.IsCompleted)
                {
                    //连接成功后开始接收数据
                    Recive(client);
                }
            }
            catch
            {


            }
        }
        /// <summary>
        /// 异步接收函数
        /// </summary>
        /// <param name="socket">已连接的socket</param>
        private void Recive(Socket socket)
        {
            try
            {
                // Create the state object
                StateObject state = new StateObject();
                StateObject.worksocket = socket;
                // Begin receiving the data from the remote device
                socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch
            {


            }
        }
        /// <summary>
        /// 异步接收回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket socket = StateObject.worksocket;
                // Read data from the remote device.  
                int bytesRead = socket.EndReceive(ar);

                if (bytesRead > 0)
                {
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    byte[] buf = new byte[bytesRead];
                    Array.Copy(state.buffer, buf, bytesRead);
                    if (ReceiveSeverData != null)
                    {
                        ReceiveSeverData(buf);
                    }
                }
                // Get the rest of the data
                socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch
            {


            }
        }
        /// <summary>
        /// 异步发送函数
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <returns>返回发送的数据个数</returns>
        public int SendAsync(byte[] data)
        {
            try
            {
                Socket socket = StateObject.worksocket;
                if (socket != null)
                {
                    //使用lambda表达式表示异步回调函数
                    socket.BeginSend(data, 0, data.Length, SocketFlags.None, ar =>
                    {
                        try
                        {
                            socket = (Socket)ar.AsyncState;
                            // Complete sending the data to the remote device.  
                            bytesSend = socket.EndSend(ar);
                        }
                        catch
                        {
                            bytesSend = -1;
                        }
                    }, socket);
                }

                return bytesSend;
            }
            catch
            {
                return -1;
            }

        }

    }
}
