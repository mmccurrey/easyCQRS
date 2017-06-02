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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Azure.Messaging
{
    public class ServiceBusEventSubscriber : IEventSubscriber
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IServiceBusManagementClient serviceBusManagementClient;
        private readonly IMessageSerializer messageSerializer;
        private readonly IConfigurationManager configurationManager;
        private readonly ILogger logger;

        public ServiceBusEventSubscriber(
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

        public IDisposable Subscribe([CallerMemberName] string name = null)
        {
            var client = ServiceBusHelper.GetEventsSubscriptionClient(configurationManager, serviceBusManagementClient, name);
            var observable = Observable.Create<Event>(observer => {

                                                           client.RegisterMessageHandler((message, cancellationToken) =>
                                                           {
                                                               var @event = messageSerializer.Deserialize<Event>(message.Body);

                                                               observer.OnNext(@event);

                                                               return Task.FromResult(true);
                                                           });

                                                           return Disposable.Empty;
                                                      })
                                                      .Publish()
                                                      .RefCount();

            return observable.Subscribe(
                @event =>
                {
                    var handlerType = typeof(IHandler<>).MakeGenericType(@event.GetType());
                    var handlers = serviceProvider.GetServices(handlerType);

                    foreach(var handler in handlers)
                    {
                        try
                        {
                            dynamic dHandler = handler.AsDynamic();

                            dHandler.HandleAsync(@event);
                        }
                        catch (Exception ex)
                        {
                            logger.Warning(
                                string.Format(
                                    "Cannot process event of type: {0} with handler: {1}. Message: {2}", 
                                    @event.GetType().Name, 
                                    handler.GetType().Name,
                                    ex.Message
                                    ));
                        }
                    }
                },
                exception => logger.Error(exception.ToString()), //OnError
                () => logger.Info($"Service bus subscription {name} completed")); //OnComplete
        }
    }
}
