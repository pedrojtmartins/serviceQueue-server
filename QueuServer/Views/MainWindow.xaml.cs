using QueuServer.Interfaces;
using System.Windows;
using QueueServer.Models;
using System;
using QueuServer.Models;
using System.IO;
using System.Linq;

namespace QueuServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IWindowUpdate
    {
        private NetworkManager serverManager;
        private string currentVideo;

        public MainWindow()
        {
            InitializeComponent();

            InitialLoad();
            StartVideoPlayback();
        }

        public void TicketsUpdated(ServerUpdate su)
        {
            if (su == null || su.nextTicket == null)
                return;

            TerminalTicket ticket = su.nextTicket;

            this.Dispatcher.Invoke(() =>
            {
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
            });
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

        public void StartVideoPlayback()
        {
            String[] allfiles = FindVideos();
            if (allfiles == null || allfiles.Length == 0)
                return;

            currentVideo = allfiles[0];

            mediaPlayer.Source = new Uri(currentVideo, UriKind.Absolute);
            mediaPlayer.MediaEnded += (s, e) => NextVideo();
            mediaPlayer.Play();
        }

        private void NextVideo()
        {
            String[] allfiles = FindVideos();
            if (allfiles == null || allfiles.Length == 0)
                return;

            for (int i = 0; i < allfiles.Length; i++)
            {
                var file = allfiles[i];
                if (currentVideo.Equals(file))
                {
                    if (i == allfiles.Length - 1)
                        currentVideo = allfiles[0];
                    else
                        currentVideo = allfiles[i + 1];

                    break;
                }
            }

            mediaPlayer.Source = new Uri(currentVideo, UriKind.Absolute);
            mediaPlayer.Play();
        }

        private string[] FindVideos()
        {
            return Directory.EnumerateFiles(_Constants.VIDEOS_FOLDER)
                .Where(file => file.ToLower().EndsWith(".wmv") || file.ToLower().EndsWith(".mp4"))
                .ToArray();
        }
    }
}
