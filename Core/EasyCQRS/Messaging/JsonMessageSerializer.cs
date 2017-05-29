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
    internal class JsonMessageSerializer : IMessageSerializer
    {
        private static JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = new NonPublicPropertiesResolver()
        };

        public TMessage Deserialize<TMessage>(byte[] data) where TMessage : IMessage
        {
            var json = Encoding.UTF8.GetString(data);
            var message = JsonConvert.DeserializeObject<TMessage>(json);

            return message;
        }

        public byte[] Serialize<TMessage>(TMessage message) where TMessage : IMessage
        {
            var json = JsonConvert.SerializeObject(message, Formatting.Indented, Settings);
            var result = Encoding.UTF8.GetBytes(json);

            return result;
        }

        class NonPublicPropertiesResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var prop = base.CreateProperty(member, memberSerialization);
                var pi = member as System.Reflection.PropertyInfo;
                if (pi != null)
                {
                    prop.Readable = (pi.GetMethod != null);
                    prop.Writable = (pi.SetMethod != null);
                }
                return prop;
            }
        }
    }
}
