using QueueServer.Models;
using QueuServer.Databases.Edmx;
using QueuServer.Interfaces;
using QueuServer.Managers;
using QueuServer.Models.Terminal;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QueuServer
{
    class NetworkManager
    {
        IPAddress ipAddress;
        TcpListener tcpListener;

        Thread newClientsThread;
        List<SocketConnection> connections;

        IWindowUpdate updateCllback;

        public NetworkManager(IWindowUpdate updateCllback)
        {
            connections = new List<SocketConnection>();
            this.updateCllback = updateCllback;
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
                //Terminal disconnected
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
                socket.ReceiveTimeout = int.MaxValue;

                try
                {
                    int size = socket.Receive(buffer);
                    if (size > 0)
                        ComputeSocketCommunication(socket, buffer, size);
                    else
                    {
                        SocketConnection.RemoveConnection(connections, socket);
                        return;
                    }
                }
                catch (Exception e)
                {
                    SocketConnection.RemoveConnection(connections, socket);
                    return;
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
            var dbManager = DatabaseManager.getInstance();
            int res = dbManager.AddNewTicket((int)comm.ticketType);

            //new Thread(() => SendDataToClients(toSend)).Start();
        }

        private void ComputeSocketCommunication_Client(Socket socket, SocketRequestCommunication comm)
        {
            foreach (var con in connections)
            {
                if (!con.isTerminal)
                    continue;

                int terminalId = SocketConnection.FindTerminal(connections, socket);
                if (terminalId != -1)
                {
                    var dbManager = DatabaseManager.getInstance();

                    if (comm.ticketCompletedId != -1)
                        dbManager.SetTicketAsComplete(comm.ticketCompletedId, terminalId);

                    ServerUpdate su = new ServerUpdate();
                    ticket t = dbManager.GetNextTicket();
                    if (t != null)
                    {
                        if (dbManager.SetTicketForClient(t.id, terminalId) == -1)
                        {

                        }

                        su.nextTicket = new Models.TerminalTicket(t);
                        updateCllback.TicketsUpdated(su);
                    }
                    else
                        su.nextTicket = null;

                    var serialized = SerializationManager<ServerUpdate>.Serialize(su);
                    new Thread(() => SendDataToClients(serialized)).Start();
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        private SocketRequestCommunication DecodeSocketCommunication(byte[] buffer, int size)
        {

            var bArray = new byte[size];
            Array.Copy(buffer, bArray, size);

            String json = Encoding.ASCII.GetString(bArray);

            return SerializationManager<SocketRequestCommunication>.Desserialize(bArray);
        }

        public void SendDataToClients(string data)
        {
            var buffer = Encoding.ASCII.GetBytes(data);
            foreach (var conn in connections)
                if (conn.isTerminal)
                    conn.socket.Send(buffer);
        }
    }
}
