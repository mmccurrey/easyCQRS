using EasyCQRS.Diagnostics;
using EasyCQRS.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Azure.Messaging
{
    internal class ServiceBus : IIntegrationEventBus
    {
        private readonly IMessageSerializer messageSerializer;
        private readonly ILogger logger;
        private readonly ITopicClient topicClient;
        private readonly InfrastructureContext db;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ServiceBus(
            IMessageSerializer messageSerializer,
            IHttpContextAccessor httpContextAccessor,
            IQueueClient queueClient,
            ITopicClient topicClient,
            ILogger logger,
            InfrastructureContext db)
        {
            this.messageSerializer = messageSerializer ?? throw new ArgumentNullException(nameof(messageSerializer));
            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(ServiceBus.httpContextAccessor));
            this.topicClient = topicClient ?? throw new ArgumentNullException(nameof(topicClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task PublishEventsAsync(params IntegrationEvent[] events)
        {
            if (events != null)
            {
                foreach (var @event in events)
                {
                    var entity = new IntegrationEventEntity
                    {
                        Id = @event.MessageId,
                        CorrelationId = httpContextAccessor.HttpContext?.TraceIdentifier,
                        ExecutedBy = httpContextAccessor.HttpContext?.User.GetId(),
                        Type = @event.GetType().AssemblyQualifiedName,
                        FullName = @event.GetType().FullName,
                        Payload = messageSerializer.Serialize(@event),
                        Date = DateTimeOffset.UtcNow
                    };

                    db.IntegrationEvents.Add(entity);

                    db.SaveChanges();
                    logger.Info("[ServiceBus->PublishEventsAsync] Sending integration event of type: {0}", @event.GetType().Name);
                    
                    var brokeredMessage = new Microsoft.Azure.ServiceBus.Message(entity.Payload);

                    brokeredMessage.UserProperties["CorrelationId"] = entity.CorrelationId;
                    brokeredMessage.UserProperties["Name"] = @event.GetType().Name;
                    brokeredMessage.UserProperties["FullName"] = @event.GetType().FullName;
                    brokeredMessage.UserProperties["Namespace"] = @event.GetType().Namespace;
                    brokeredMessage.UserProperties["Type"] = @event.GetType().AssemblyQualifiedName;
                    brokeredMessage.UserProperties["EventType"] = "Integration";

                    await topicClient.SendAsync(brokeredMessage);
                }
            }
        }
    }
}
