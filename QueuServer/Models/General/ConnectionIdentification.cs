using System.Runtime.Serialization;

namespace QueuServer.Models.Terminal
{
    [DataContract]
    class ConnectionIdentification
    {
        [DataMember(Name = "t")]
        public bool IsTerminal { get; set; }

        [DataMember(Name = "i")]
        public int TerminalId { get; set; }
    }
}
