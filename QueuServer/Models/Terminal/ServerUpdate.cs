using QueuServer.Models;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace QueueServer.Models
{
    [DataContract]
    public class ServerUpdate
    {
        [DataMember(Name = "nt")]
        public TerminalTicket nextTicket { get; set; }

        [DataMember(Name = "t")]
        public List<TerminalTicket> tickets { get; set; }
    }
}
