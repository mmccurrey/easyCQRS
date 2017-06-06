using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Messaging
{
    public interface IMessageSerializer
    {
        byte[] Serialize<TMessage>(TMessage message) where TMessage : IMessage;

        TMessage Deserialize<TMessage>(Type type, byte[] data) where TMessage : IMessage;        
    }
}
