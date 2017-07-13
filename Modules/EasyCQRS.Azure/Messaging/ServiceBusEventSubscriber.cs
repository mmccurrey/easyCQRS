using EasyCQRS.Diagnostics;
using EasyCQRS.Messaging;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Azure.Messaging
{
    public class ServiceBusMessageSubscriber : IMessageSubscriber
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IServiceBusManagementClient serviceBusManagementClient;
        private readonly IMessageSerializer messageSerializer;
        private readonly IConfigurationManager configurationManager;
        private readonly ILogger logger;

        public ServiceBusMessageSubscriber(
            IServiceProvider serviceProvider,
            IServiceBusManagementClient serviceBusManagementClient,
            IMessageSerializer messageSerializer,
            IConfigurationManager configurationManager,
            ILogger logger)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException("config");
            this.serviceBusManagementClient = serviceBusManagementClient ?? throw new ArgumentNullException("serviceBusManagementClient");
            this.messageSerializer = messageSerializer ?? throw new ArgumentNullException("messageSerializer");
            this.configurationManager = configurationManager ?? throw new ArgumentNullException("settingsManager");
            this.logger = logger ?? throw new ArgumentNullException("logger");
        }

        public IDisposable Subscribe<TMessage>([CallerMemberName] string name = null) where TMessage: class, IMessage
        {
            var client = ServiceBusHelper.GetEventsSubscriptionClient(configurationManager, serviceBusManagementClient, name);
            var observable = Observable.Create<TMessage>(observer => {

                                                           client.RegisterMessageHandler((message, cancellationToken) =>
                                                           {
                                                               try
                                                               {
                                                                   var parentType = typeof(TMessage);
                                                                   var messageType = Type.GetType(message.UserProperties["Type"] as string);

                                                                   if (messageType != null && parentType.IsAssignableFrom(messageType))
                                                                   {
                                                                       var @event = messageSerializer.Deserialize<TMessage>(
                                                                           messageType, message.Body);

                                                                       observer.OnNext(@event);
                                                                   }
                                                               }
                                                               catch
                                                               {
                                                                   logger.Warning("Cannot deserialize message. ");
                                                               }

                                                               return Task.FromResult(true);
                                                           });

                                                           return Disposable.Empty;
                                                      })
                                                      .Publish()
                                                      .RefCount();

            return observable.Subscribe(
                message =>
                {
                    var handlers = GetHandlers(serviceProvider, message.GetType());
                    if (handlers != null)
                    {
                        foreach (var handler in handlers)
                        {
                            try
                            {
                                dynamic dHandler = handler.AsDynamic();

                                dHandler.HandleAsync(message);
                            }
                            catch (Exception ex)
                            {
                                logger.Warning(
                                    string.Format(
                                        "Cannot process message of type: {0} with handler: {1}. Message: {2}",
                                        message.GetType().Name,
                                        handler.GetType().Name,
                                        ex.Message
                                        ));
                            }
                        }
                    }
                },
                exception => logger.Error(exception.ToString()), //OnError
                () => logger.Info($"Service bus subscription {name} completed")); //OnComplete
        }

        private IEnumerable<object> GetHandlers(IServiceProvider serviceProvider, Type messageType)
        {
            var handlers = new List<object>();
            do
            {
                var handlerType = typeof(IHandler<>).MakeGenericType(messageType);
                var resolvedHandlers = serviceProvider.GetServices(handlerType);

                if(resolvedHandlers != null)
                {
                    handlers.AddRange(resolvedHandlers);
                }
            }
            while (messageType != null && messageType != typeof(EasyCQRS.Messaging.Message));

            return handlers;
        }
    }
}
