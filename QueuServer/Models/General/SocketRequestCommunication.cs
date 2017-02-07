using System.Runtime.Serialization;

namespace QueuServer
{
    [DataContract]
    class SocketRequestCommunication
    {
        public enum Who { ANDROID, CLIENT };

        public enum What { REQUEST_TICKET, HANDLE_TICKET };
        public enum TicketType { NORMAL, PRIORITY }

        [DataMember(Name = "o")]
        public Who who { get; set; }

        [DataMember(Name = "a")]
        public What what { get; set; }

        [DataMember(Name = "t", IsRequired = false)]
        public int iTicketType { get; set; }

        public TicketType ticketType { get { return iTicketType == 0 ? TicketType.NORMAL : TicketType.PRIORITY; } }

        [DataMember(Name = "i", IsRequired = false)]
        public int ticketCompletedId { get; set; }
    }
}
