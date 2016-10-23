using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace QueuServer.Managers
{
    class SerializationManager<T>
    {
        public static string Serialize(T obj)
        {
            try
            {
                using (MemoryStream stream1 = new MemoryStream())
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                    ser.WriteObject(stream1, obj);
                    stream1.Position = 0;

                    using (StreamReader sr = new StreamReader(stream1))
                        return sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return default(string);
        }

        public static T Desserialize(string json)
        {
            return Desserialize(Encoding.ASCII.GetBytes(json));
        }

        public static T Desserialize(byte[] obj)
        {
            try
            {
                if (obj == null)
                    return default(T);

                using (var ms = new MemoryStream(obj))
                {
                    var serializer = new DataContractJsonSerializer(typeof(T));
                    return (T)serializer.ReadObject(ms);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return default(T);
        }
    }
}
