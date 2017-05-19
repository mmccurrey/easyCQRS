using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EasyCQRS.Messaging
{
    class XmlMessageSerializer : IMessageSerializer
    {
        public TMessage Deserialize<TMessage>(byte[] data) where TMessage : IMessage
        {
            var serializer = new XmlSerializer(typeof(TMessage));
            using(var stream = new MemoryStream(data))
            {
                return (TMessage) serializer.Deserialize(stream);
            }
        }

        public byte[] Serialize<TMessage>(TMessage message) where TMessage : IMessage
        {
            byte[] result;
            var serializer = new XmlSerializer(typeof(TMessage));

            using (var stream = new MemoryStream())
            {

                serializer.Serialize(stream, message);

                stream.Flush();

                result = stream.ToArray();
            }

            return result;
        }
    }
}
