using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Messaging
{
    public abstract class Message: IMessage
    {
        public string MessageId { get; private set; }

        public Message(string messageId)
        {
            if (string.IsNullOrWhiteSpace(messageId))
            {
                throw new ArgumentNullException(nameof(messageId));
            }

            MessageId = messageId;
        }

        public Message(): 
            this(Guid.NewGuid().ToString())
        {
        }
    }
}
