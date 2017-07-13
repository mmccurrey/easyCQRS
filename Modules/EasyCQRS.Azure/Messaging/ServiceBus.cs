using EasyCQRS.Diagnostics;
using EasyCQRS.Messaging;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Azure.Messaging
{
    internal class ServiceBus : IEventBus, IIntegrationEventBus
    {
        private readonly IMessageSerializer messageSerializer;
        private readonly ILogger logger;
        private readonly IQueueClient queueClient;
        private readonly ITopicClient topicClient;
        private readonly InfrastructureContext db;

        public ServiceBus(
            IMessageSerializer messageSerializer,
            IQueueClient queueClient,
            ITopicClient topicClient,
            ILogger logger,
            InfrastructureContext db)
        {
            this.messageSerializer = messageSerializer ?? throw new ArgumentNullException("messageSerializer");
            this.queueClient = queueClient ?? throw new ArgumentNullException("queueClient");
            this.topicClient = topicClient ?? throw new ArgumentNullException("topicClient");
            this.logger = logger ?? throw new ArgumentNullException("logger");
            this.db = db ?? throw new ArgumentNullException("db");

        }


        public async Task PublishEventsAsync(params Event[] events)
        {
            if (events != null)
            {
                foreach (var @event in events)
                {
                    logger.Info("[ServiceBus->PublishEventsAsync] Sending event of type: {0}", @event.GetType().Name);

                    var payload = messageSerializer.Serialize(@event);
                    var brokeredMessage = new Microsoft.Azure.ServiceBus.Message(payload);

                    brokeredMessage.UserProperties["CorrelationId"] = @event.CorrelationId;
                    brokeredMessage.UserProperties["Name"] = @event.GetType().Name;
                    brokeredMessage.UserProperties["FullName"] = @event.GetType().FullName;
                    brokeredMessage.UserProperties["Namespace"] = @event.GetType().Namespace;
                    brokeredMessage.UserProperties["Type"] = @event.GetType().AssemblyQualifiedName;
                    brokeredMessage.UserProperties["Event"] = true;
                    brokeredMessage.UserProperties["IntegrationEvent"] = false;

                    await topicClient.SendAsync(brokeredMessage);
                }
            }
        }

        public async Task PublishEventsAsync(params IntegrationEvent[] events)
        {
            if (events != null)
            {
                foreach (var @event in events)
                {
                    var entity = new IntegrationEventEntity
                    {
                        Id = @event.EventId,
                        CorrelationId = @event.CorrelationId,
                        ExecutedBy = @event.ExecutedBy,
                        Type = @event.GetType().AssemblyQualifiedName,
                        FullName = @event.GetType().FullName,
                        Date = @event.Timestamp
                    };

                    db.IntegrationEvents.Add(entity);

                    db.SaveChanges();
                    logger.Info("[ServiceBus->PublishEventsAsync] Sending integration event of type: {0}", @event.GetType().Name);

                    var payload = messageSerializer.Serialize(@event);
                    var brokeredMessage = new Microsoft.Azure.ServiceBus.Message(payload);

                    brokeredMessage.UserProperties["CorrelationId"] = @event.CorrelationId;
                    brokeredMessage.UserProperties["Name"] = @event.GetType().Name;
                    brokeredMessage.UserProperties["FullName"] = @event.GetType().FullName;
                    brokeredMessage.UserProperties["Namespace"] = @event.GetType().Namespace;
                    brokeredMessage.UserProperties["Type"] = @event.GetType().AssemblyQualifiedName;
                    brokeredMessage.UserProperties["Event"] = false;
                    brokeredMessage.UserProperties["IntegrationEvent"] = true;

                    await topicClient.SendAsync(brokeredMessage);
                }
            }
        }
    }
}
