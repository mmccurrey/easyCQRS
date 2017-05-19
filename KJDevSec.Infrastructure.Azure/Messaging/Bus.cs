using KJDevSec.Messaging;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KJDevSec.Azure.Messaging
{
    public class Bus : IBus
    {
        private readonly QueueClient commandsQueueClient;
        private readonly QueueClient eventsQueueClient;

        public Bus()
        {
            this.commandsQueueClient = GetQueueClient("machete-commands");
            this.eventsQueueClient = GetQueueClient("machete-events");
        }

        public Task SendCommandAsync<T>(T command) where T : Command
        {
            return SendMessage(command, commandsQueueClient);
        }

        public Task PublishEventsAsync(params Event[] events)
        {
            var tasks = new List<Task>();
            if(events != null)
            {
                foreach(var @event in events)
                {
                    tasks.Add(SendMessage(@event, eventsQueueClient));
                }
            }

            return Task.WhenAll(tasks);
        }

        async Task SendMessage(IMessage message, QueueClient queueClient)
        {
            var payload = Config.MessageSerializer.Serialize(message);
            var brokeredMessage = new Message(payload);

            brokeredMessage.UserProperties["Type"] = message.GetType().AssemblyQualifiedName;

            await queueClient.SendAsync(brokeredMessage);
        }

        QueueClient GetQueueClient(string queueName)
        {
            string connectionString = Config.GetSetting("Bus");

            // Initialize the connection to Service Bus Queue
            return new QueueClient(connectionString, queueName);
        }
    }
}
