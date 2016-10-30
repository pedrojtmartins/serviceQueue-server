using QueuServer.Managers;
using QueuServer.Models.Terminal;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace QueuServer
{
    class NetworkManager
    {
        IPAddress ipAddress;
        TcpListener tcpListener;

        Thread newClientsThread;
        List<SocketConnection> connections;

        public NetworkManager()
        {
            connections = new List<SocketConnection>();
        }

        ~NetworkManager()
        {
            tcpListener.Stop();
            newClientsThread.Abort();

            if (connections != null && connections.Count > 0)
                foreach (var conn in connections)
                    conn.Terminate();
        }

        public void Initialize()
        {
            ipAddress = IPAddress.Parse(_Constants.IP_ADDRESS);
            tcpListener = new TcpListener(ipAddress, _Constants.IP_PORT);
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
                new Thread(() => IdentifyConnection(newSocket)).Start();

            }
        }

        private void IdentifyConnection(Socket socket)
        {
            var buffer = new byte[100];
            int size = socket.Receive(buffer);
            if (size > 0)
            {
                var id = IdentifyConnection(buffer, size);
                if (id != null)
                {
                    Thread newThread = new Thread(() => ListToClients(socket));
                    connections.Add(new SocketConnection(socket, newThread, id.IsTerminal, id.TerminalId));
                    newThread.Start();
                }
                else
                    throw new Exception();

            }
            else
                throw new Exception();
        }

        private ConnectionIdentification IdentifyConnection(byte[] buffer, int size)
        {
            var data = new byte[size];
            Array.Copy(buffer, data, size);

            return SerializationManager<ConnectionIdentification>.Desserialize(data);
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
                    ComputeSocketCommunication_Android(socket, comm);
                    break;

                case SocketRequestCommunication.Who.CLIENT:
                    ComputeSocketCommunication_Client(socket, comm);
                    break;
            }
        }

        private void ComputeSocketCommunication_Android(Socket originSocket, SocketRequestCommunication comm)
        {
            foreach (var con in connections)
            {
                if (!con.isTerminal || con.socket.Equals(originSocket))
                    continue;

                SendDataToClient(con.socket, new byte[] { 1, 2, 5 });
            }
        }

        private void ComputeSocketCommunication_Client(Socket socket, SocketRequestCommunication comm)
        {
            foreach (var con in connections)
            {
                if (!con.isTerminal)
                    continue;

                SendDataToClient(con.socket, new byte[] { 1, 2, 5 });
            }
        }

        private void UpdateServerWindow()
        {
        }

        private SocketRequestCommunication DecodeSocketCommunication(byte[] buffer, int size)
        {
            var bArray = new byte[size];
            Array.Copy(buffer, bArray, size);

            return SerializationManager<SocketRequestCommunication>.Desserialize(bArray);
        }

        public void SendDataToClient(Socket socket, byte[] data)
        {
            socket.Send(data);
        }
    }
}
