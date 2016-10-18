using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QueuServer
{
    class ServerManager
    {
        List<SocketConnection> connections;

        IPAddress ipAddress;
        TcpListener tcpListener;

        Thread newClientsThread;

        public ServerManager()
        {
            connections = new List<SocketConnection>();
        }

        ~ServerManager()
        {
            tcpListener.Stop();
            newClientsThread.Abort();

            if (connections != null && connections.Count > 0)
                foreach (var conn in connections)
                    conn.Terminate();
        }

        public void Initialize()
        {
            ipAddress = IPAddress.Parse(Constants.IP_ADDRESS);
            tcpListener = new TcpListener(ipAddress, Constants.IP_PORT);
            tcpListener.Start();

            newClientsThread = new Thread(() => ListenToNewClients(tcpListener));
            newClientsThread.Start();
        }

        private void ListenToNewClients(TcpListener tcpListener)
        {
            if (tcpListener == null)
                return;

            while (true)
            {
                Socket newSocket = tcpListener.AcceptSocket();
                Thread newThread = new Thread(() => ListToClients(newSocket));
                newThread.Start();

                connections.Add(new SocketConnection(newSocket, newThread));
            }
        }

        private void ListToClients(Socket socket)
        {
            while (true)
            {
                //if (!socket.Connected)
                //{
                //    connectedSockets.Remove(socket);
                //    break;
                //}

                var buffer = new byte[100];
                int size = socket.Receive(buffer);
                if (size > 0)
                    ComputeSocketCommunication(socket, buffer, size);
                else
                {
                    //TODO remove socket
                    throw new Exception();
                }
            }
        }

        private void ComputeSocketCommunication(Socket socket, byte[] buffer, int size)
        {
            var comm = DecodeSocketCommunication(buffer, size);
            if (comm == null)
                throw new Exception(); //TODO handle invalid comm

            switch (comm.who)
            {
                case SocketRequestCommunication.Who.ANDROID:
                    ComputeSocketCommunication_Android(comm);
                    break;

                case SocketRequestCommunication.Who.CLIENT:
                    ComputeSocketCommunication_Client(comm);
                    break;
            }

        }

        private void ComputeSocketCommunication_Android(SocketRequestCommunication comm)
        {

        }

        private void ComputeSocketCommunication_Client(SocketRequestCommunication comm)
        {

        }

        private SocketRequestCommunication DecodeSocketCommunication(byte[] buffer, int size)
        {
            var bArray = new byte[size];
            Array.Copy(buffer, bArray, size);

            return SocketRequestCommunication.Deserialize(bArray);
        }
    }
}
