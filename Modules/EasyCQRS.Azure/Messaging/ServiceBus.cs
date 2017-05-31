﻿using EasyCQRS.Diagnostics;
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
    class ServiceBus : IBus
    {
        private readonly ServiceBusManagementClient serviceBusManagementClient;
        private readonly IMessageSerializer messageSerializer;
        private readonly IConfigurationManager configurationManager;
        private readonly ILogger logger;

        private readonly QueueClient commandsQueueClient;
        private readonly TopicClient eventsTopicClient;
        private readonly InfrastructureContext db;

        public ServiceBus(            
            ServiceBusManagementClient serviceBusManagementClient,
            IMessageSerializer messageSerializer,
            IConfigurationManager configurationManager, 
            ILogger logger, 
            InfrastructureContext db)
        {
            this.serviceBusManagementClient = serviceBusManagementClient ?? throw new ArgumentNullException("serviceBusManagementClient");
            this.messageSerializer = messageSerializer ?? throw new ArgumentNullException("messageSerializer");
            this.configurationManager = configurationManager ?? throw new ArgumentNullException("settingsManager");
            this.logger = logger ?? throw new ArgumentNullException("logger");
            this.db = db ?? throw new ArgumentNullException("db");

            this.commandsQueueClient = ServiceBusHelper.GetCommandsQueueClient(configurationManager);
            this.eventsTopicClient = ServiceBusHelper.GetEventsTopicClient(configurationManager, serviceBusManagementClient);
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

            db.SaveChanges();

            try
            {
                var payload = messageSerializer.Serialize(command);
                var brokeredMessage = new Message(payload);

                brokeredMessage.UserProperties["CorrelationId"] = command.CorrelationId;
                brokeredMessage.UserProperties["Name"] = command.GetType().Name;
                brokeredMessage.UserProperties["FullName"] = command.GetType().FullName;
                brokeredMessage.UserProperties["Namespace"] = command.GetType().Namespace;
                brokeredMessage.UserProperties["Type"] = command.GetType().AssemblyQualifiedName;
                brokeredMessage.UserProperties["Command"] = true;                 

                await commandsQueueClient.SendAsync(brokeredMessage);

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

                    var payload = messageSerializer.Serialize(@event);
                    var brokeredMessage = new Message(payload);

                    brokeredMessage.UserProperties["CorrelationId"] = @event.CorrelationId;                   
                    brokeredMessage.UserProperties["Name"] = @event.GetType().Name;
                    brokeredMessage.UserProperties["FullName"] = @event.GetType().FullName;
                    brokeredMessage.UserProperties["Namespace"] = @event.GetType().Namespace;
                    brokeredMessage.UserProperties["Type"] = @event.GetType().AssemblyQualifiedName;
                    brokeredMessage.UserProperties["Event"] = true;

                    tasks.Add(eventsTopicClient.SendAsync(brokeredMessage));
                }
            }

            return Task.WhenAll(tasks);
        }

        
    }
}
