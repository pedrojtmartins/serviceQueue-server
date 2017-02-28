using QueueServer.Models;
using QueuServer.Databases.Edmx;
using QueuServer.Interfaces;
using QueuServer.Managers;
using QueuServer.Media;
using QueuServer.Models.Terminal;
using QueuServer.Printer;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public void Initialize()
        {
            string myIp = getMyIp();
            updateCllback.log(myIp);
            ipAddress = IPAddress.Parse(myIp);
            tcpListener = new TcpListener(ipAddress, _Constants.IP_PORT);
            tcpListener.Start();
            
            newClientsThread = new Thread(() => ListenToNewClients(tcpListener));
            newClientsThread.Start();
        }

        private void ListenToNewClients(TcpListener tcpListener)
        {
            if (tcpListener == null)
                return;

            try
            {
                while (true)
                {
                    Socket newSocket = tcpListener.AcceptSocket();
                    new Thread(() => IdentifyConnection(newSocket)).Start();
                }
            }catch(Exception e)
            {
                //return;
            }
        }

        private void IdentifyConnection(Socket socket)
        {
            updateCllback.log("waiting id");
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
                    updateCllback.log("id check");
                }
                else
                {
                    updateCllback.log("id null");
                }

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

                updateCllback.log("list client");
                var buffer = new byte[100];
                socket.ReceiveTimeout = int.MaxValue;

                try
                {
                    int size = socket.Receive(buffer);
                    updateCllback.log("client req");
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
                    updateCllback.log(e.ToString());
                    SocketConnection.RemoveConnection(connections, socket);
                    return;
                }
            }
        }

        internal void Close()
        {
            try
            {
                if (connections != null && connections.Count > 0)
                    foreach (var c in connections)
                        c.Terminate();

                tcpListener.Stop();
                newClientsThread.Abort();
            }
            catch (Exception e) { }
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
            ticket newTicket = dbManager.AddNewTicket((int)comm.ticketType);

            if (newTicket == null)
            {
                new Thread(() => SendData(originSocket, "0")).Start();
                return;
            }

            //Print ticket
            var sType = SocketRequestCommunication.getTicketTypeString(comm.ticketType);
            var sNum = SocketRequestCommunication.getTicketNumber(comm.ticketType, newTicket.number);
            new PrinterManager(sType, sNum).Print();

            //Send confirmation to android
            new Thread(() =>
            {
                Thread.Sleep(_Constants.TIME_WAIT_PRINTER); //give time for the printer to do it's work
                SendData(originSocket, "1");
            }).Start();
        }

        private void ComputeSocketCommunication_Client(Socket socket, SocketRequestCommunication comm)
        {
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

                    MediaPlayer.playNextTicket();
                }
                else
                    su.nextTicket = null;

                var serialized = SerializationManager<ServerUpdate>.Serialize(su);
                new Thread(() => SendData(socket, serialized)).Start();
            }
            else
            {
                throw new Exception();
            }
        }

        private SocketRequestCommunication DecodeSocketCommunication(byte[] buffer, int size)
        {

            var bArray = new byte[size];
            Array.Copy(buffer, bArray, size);

            String json = Encoding.ASCII.GetString(bArray);

            return SerializationManager<SocketRequestCommunication>.Desserialize(bArray);
        }

        //public void SendDataToClients(string data)
        //{
        //    var buffer = Encoding.ASCII.GetBytes(data);
        //    foreach (var conn in connections)
        //        if (conn.isTerminal)
        //            conn.socket.Send(buffer);
        //}

        public void SendData(Socket socket, String data)
        {
            var buffer = Encoding.ASCII.GetBytes(data);
            socket.Send(buffer);
        }

        private string getMyIp()
        {
            var host = Dns.GetHostName();
            var hostEntry = Dns.GetHostEntry(host);
            var adresses = hostEntry.AddressList;
            var address = adresses.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            if (address == null)
                return "";

            return address.ToString();
        }
    }
}
