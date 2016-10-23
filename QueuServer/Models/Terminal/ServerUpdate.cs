using QueuServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QueueServer.Models
{
    [DataContract]
    public class ServerUpdate
    {
        [DataMember(Name = "t")]
        public List<TerminalTicket> tickets { get; set; }
    }
}
