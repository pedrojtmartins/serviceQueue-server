using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace QueuServer
{
    class SocketConnection
    {
        public Socket socket { get; set; }
        public Thread thread { get; set; }

        public bool isTerminal { get; set; }
        public int terminalId { get; set; }

        public SocketConnection(Socket socket, Thread thread, bool isTerminal, int terminalId = -1)
        {
            this.socket = socket;
            this.thread = thread;
            this.isTerminal = isTerminal;
            this.terminalId = terminalId;
        }

        public void Terminate()
        {
            if (socket != null && socket.Connected)
                socket.Close();

            if (thread != null)
                thread.Abort();
        }

        public static int FindTerminal(List<SocketConnection> conns, Socket socket)
        {
            foreach(var conn in conns)
                if (conn.socket.Equals(socket))
                    return conn.terminalId;

            return -1;
        }
    }
}
