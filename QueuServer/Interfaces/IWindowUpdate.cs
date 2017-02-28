using QueueServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueuServer.Interfaces
{
    interface IWindowUpdate
    {
        void TicketsUpdated(ServerUpdate su);
        void log(string s);
    }
}
