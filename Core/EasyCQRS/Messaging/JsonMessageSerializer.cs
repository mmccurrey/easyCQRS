using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EasyCQRS.Messaging
{
    public class JsonMessageSerializer : IMessageSerializer
    {
        private static JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = new Serialization.NonPublicPropertiesContractResolver()
        };

        public TMessage Deserialize<TMessage>(Type type, byte[] data) where TMessage : IMessage
        {
            var json = Encoding.UTF8.GetString(data);
            var message = (TMessage) JsonConvert.DeserializeObject(json, type, Settings);

            return message;
        }

        public byte[] Serialize<TMessage>(TMessage message) where TMessage : IMessage
        {
            var json = JsonConvert.SerializeObject(message, Formatting.Indented, Settings);
            var result = Encoding.UTF8.GetBytes(json);

            return result;
        }
    }
}
