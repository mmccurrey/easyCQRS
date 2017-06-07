using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Azure.Messaging
{
    class ServiceBusHelper
    {
        internal static IQueueClient GetCommandsQueueClient(IConfigurationManager configurationManager)
        {
            var connectionString = configurationManager.GetSetting("EasyCQRS.Infrastructure.ServiceBus.ConnectionString");
            var queueName = configurationManager.GetSetting("EasyCQRS.Infrastructure.ServiceBus.CommandsQueue");

            return new QueueClient(connectionString, queueName);
        }

        internal static ITopicClient GetEventsTopicClient(IConfigurationManager configurationManager, IServiceBusManagementClient serviceBusManagementClient)
        {
            var connectionString = configurationManager.GetSetting("EasyCQRS.Infrastructure.ServiceBus.ConnectionString");
            var resourceGroup = configurationManager.GetSetting("EasyCQRS.Infrastructure.ServiceBus.ResourceGroup");
            var ns = configurationManager.GetSetting("EasyCQRS.Infrastructure.ServiceBus.Namespace");
            var topicName = configurationManager.GetSetting("EasyCQRS.Infrastructure.ServiceBus.EventsTopic");

            serviceBusManagementClient.Topics.CreateOrUpdate(
                resourceGroup,
                ns,
                topicName,
                new Microsoft.Azure.Management.ServiceBus.Fluent.Models.TopicInner
                {
                    Location = "East US"
                });

            return new TopicClient(connectionString, topicName, RetryPolicy.Default);
        }

        internal static ISubscriptionClient GetEventsSubscriptionClient(IConfigurationManager configurationManager, IServiceBusManagementClient serviceBusManagementClient, string subscriptionName)
        {
            var connectionString = configurationManager.GetSetting("EasyCQRS.Infrastructure.ServiceBus.ConnectionString");
            var resourceGroup = configurationManager.GetSetting("EasyCQRS.Infrastructure.ServiceBus.ResourceGroup");
            var ns = configurationManager.GetSetting("EasyCQRS.Infrastructure.ServiceBus.Namespace");
            var topicName = configurationManager.GetSetting("EasyCQRS.Infrastructure.ServiceBus.EventsTopic");

            serviceBusManagementClient.Subscriptions.CreateOrUpdate(
                resourceGroup,
                ns,
                topicName,
                subscriptionName,
                new Microsoft.Azure.Management.ServiceBus.Fluent.Models.SubscriptionInner
                {
                    Location = "East US"
                });

            return new SubscriptionClient(connectionString, topicName, subscriptionName);
        }
    }
}
