using System.Windows;

namespace QueuServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NetworkManager serverManager;

        public MainWindow()
        {
            InitializeComponent();

            InitialLoad();
        }

        private void InitialLoad()
        {
            serverManager = new NetworkManager();
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
