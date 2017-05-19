using EasyCQRS.Diagnostics;
using EasyCQRS.Messaging;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Azure.Messaging
{
    class ServiceBus : IBus
    {
        private readonly IMessageSerializer messageSerializer;
        private readonly ISettingManager settingsManager;
        private readonly ILogger logger;

        private readonly QueueClient commandsQueueClient;
        private readonly QueueClient eventsQueueClient;
        private readonly InfrastructureContext db;

        public ServiceBus(            
            IMessageSerializer messageSerializer,
            ISettingManager settingsManager, 
            ILogger logger,
            InfrastructureContext db)
        {
            this.messageSerializer = messageSerializer ?? throw new ArgumentNullException("messageSerializer");
            this.settingsManager = settingsManager ?? throw new ArgumentNullException("settingsManager");
            this.logger = logger ?? throw new ArgumentNullException("logger");

            this.db = db ?? throw new ArgumentNullException("db");
            this.commandsQueueClient = GetQueueClient("machete-commands");
            this.eventsQueueClient = GetQueueClient("machete-events");
        }

        public async Task SendCommandAsync<T>(T command) where T : Command
        {
            logger.Info("[ServiceBus->SendCommandAsync] Sending command of type: {0}", typeof(T).Name);

            var entity = new CommandEntity
            {
                Id = command.CommandId,
                CorrelationId = command.CorrelationId,
                ExecutedBy = command.ExecutedBy,
                Type = command.GetType().FullName,
                ScheduledAt = DateTimeOffset.UtcNow,                
                Payload = messageSerializer.Serialize(command)                
            };

            db.Commands.Add(entity);

            await db.SaveChangesAsync();

            try
            {               
                await SendMessage(command, commandsQueueClient);

                entity.Success = true;
            }
            catch (Exception e)
            {
                entity.ErrorDescription = e.ToString();

                throw e;
            }
            finally
            {
                entity.Executed = true;
                entity.ExecutedAt = DateTimeOffset.UtcNow;
                db.SaveChanges();
            }
        }

        public Task PublishEventsAsync(params Event[] events)
        {
            var tasks = new List<Task>();
            if(events != null)
            {
                foreach(var @event in events)
                {
                    logger.Info("[ServiceBus->PublishEventsAsync] Sending event of type: {0}", @event.GetType().Name);
                    tasks.Add(SendMessage(@event, eventsQueueClient));
                }
            }

            return Task.WhenAll(tasks);
        }

        async Task SendMessage(IMessage message, QueueClient queueClient)
        {
            var payload = messageSerializer.Serialize(message);
            var brokeredMessage = new Message(payload);

            brokeredMessage.UserProperties["Type"] = message.GetType().AssemblyQualifiedName;

            await queueClient.SendAsync(brokeredMessage);
        }

        QueueClient GetQueueClient(string queueName)
        {
            string connectionString = settingsManager.GetSetting("Bus");

            // Initialize the connection to Service Bus Queue
            return new QueueClient(connectionString, queueName);
        }
    }
}
