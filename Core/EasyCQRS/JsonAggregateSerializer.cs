using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS
{
    public class JsonAggregateSerializer : IAggregateSerializer
    {
        private static JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
            TypeNameHandling = TypeNameHandling.All,
            ContractResolver = new Serialization.NonPublicPropertiesContractResolver()
        };

        public TAggregateRoot Deserialize<TAggregateRoot>(byte[] data) where TAggregateRoot : AggregateRoot
        {
            var json = Encoding.UTF8.GetString(data);
            var aggregate = JsonConvert.DeserializeObject<TAggregateRoot>(json, Settings);

            return aggregate;
        }

        public byte[] Serialize<TAggregateRoot>(TAggregateRoot aggregateRoot) where TAggregateRoot : AggregateRoot
        {
            var json = JsonConvert.SerializeObject(aggregateRoot, Formatting.Indented, Settings);
            var result = Encoding.UTF8.GetBytes(json);

            return result;
        }
    }
}
