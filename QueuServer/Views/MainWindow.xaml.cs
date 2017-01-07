using QueuServer.Interfaces;
using System.Windows;
using QueueServer.Models;
using System;
using QueuServer.Models;

namespace QueuServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IWindowUpdate
    {
        private NetworkManager serverManager;

        public MainWindow()
        {
            InitializeComponent();

            InitialLoad();
        }

        public void TicketsUpdated(ServerUpdate su)
        {
            if (su == null || su.nextTicket == null)
                return;

            TerminalTicket ticket = su.nextTicket;
            if (ticket.type == 0)
            {
                Row1_Ticket.Text = "A" + ticket.number;
                Row1_Terminal.Text = "" + ticket.clientId;
            }
            else if (ticket.type == 1)
            {
                Row2_Ticket.Text = "B" + ticket.number;
                Row2_Terminal.Text = "" + ticket.clientId;
            }
            else if (ticket.type == 3)
            {
                Row3_Ticket.Text = "C" + ticket.number;
                Row3_Terminal.Text = "" + ticket.clientId;
            }
        }

        private void InitialLoad()
        {
            serverManager = new NetworkManager(this);
            serverManager.Initialize();


            //var comm = new SocketCommunication()
            //{
            //    who = SocketCommunication.Who.ANDROID,
            //    what = SocketCommunication.What.REQUEST_TICKET,
            //    ticketType = SocketCommunication.TicketType.NORMAL
            //};

            //Console.WriteLine(SocketCommunication.Serialize(comm));
        }
    }
}
