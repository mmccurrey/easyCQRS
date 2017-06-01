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

namespace EasyCQRS.EventSourcing
{
    internal class JsonSagaSerializer : ISagaSerializer
    {
        private static JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = new Serialization.NonPublicPropertiesContractResolver()
        };

        public TSaga Deserialize<TSaga>(byte[] data) where TSaga : ISaga
        {
            var json = Encoding.UTF8.GetString(data);
            var message = JsonConvert.DeserializeObject<TSaga>(json, Settings);

            return message;
        }

        public byte[] Serialize<TSaga>(TSaga message) where TSaga : ISaga
        {
            var json = JsonConvert.SerializeObject(message, Formatting.Indented, Settings);
            var result = Encoding.UTF8.GetBytes(json);

            return result;
        }
    }
}
