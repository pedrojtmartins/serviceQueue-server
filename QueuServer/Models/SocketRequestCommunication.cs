using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

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
        public TicketType ticketType { get; set; }

        public static SocketRequestCommunication Deserialize(byte[] jsonBArray)
        {
            if (jsonBArray == null)
                return null;

            using (var ms = new MemoryStream(jsonBArray))
            {
                var serializer = new DataContractJsonSerializer(typeof(SocketRequestCommunication));
                return (SocketRequestCommunication)serializer.ReadObject(ms);
            }
        }

        public static String Serialize(SocketRequestCommunication obj)
        {
            using (MemoryStream stream1 = new MemoryStream())
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(SocketRequestCommunication));
                ser.WriteObject(stream1, obj);
                stream1.Position = 0;

                using (StreamReader sr = new StreamReader(stream1))
                    return sr.ReadToEnd();
            }
        }

    }


}
