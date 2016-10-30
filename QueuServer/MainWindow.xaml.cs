using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
