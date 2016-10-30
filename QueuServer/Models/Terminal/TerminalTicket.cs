using QueuServer.Databases.Edmx;
using System;
using System.Runtime.Serialization;

namespace QueuServer.Models
{
    [DataContract]
    public class TerminalTicket
    {
        [DataMember(Name = "i")]
        public int id { get; set; }

        [DataMember(Name = "n")]
        public int number { get; set; }

        [DataMember(Name = "t")]
        public int type { get; set; }

        [DataMember(Name = "c", IsRequired = false)]
        public Nullable<int> clientId { get; set; }

        public TerminalTicket(ticket dbTicket)
        {
            this.id = dbTicket.id;
            this.number = dbTicket.number;
            this.type = dbTicket.type;
            this.clientId = dbTicket.clientId;
        }
    }
}
