using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary_FQY.ClientSocket
{
    /// <summary>
    /// 用于从远程设备接收数据的状态对象
    /// </summary>
    public class StateObject
    {
        // Client socket.
        public static Socket worksocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }
}
