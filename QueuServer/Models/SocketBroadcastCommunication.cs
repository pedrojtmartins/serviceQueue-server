using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QueuServer.Models
{
    [DataContract]
    class SocketBroadcastCommunication
    {
        [DataMember(Name = "t")]
        public List<ticket> tickets { get; set; }

        public SocketBroadcastCommunication(List<ticket> tickets)
        {
            this.tickets = tickets;
        }
    }
}
