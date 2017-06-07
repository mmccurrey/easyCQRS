using EasyCQRS.Diagnostics;
using EasyCQRS.Messaging;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Azure.Messaging
{
    internal class TrackedCommandBus: ICommandBus
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IMessageSerializer messageSerializer;
        private readonly ILogger logger;
        private readonly InfrastructureContext db;

        public TrackedCommandBus(
            IServiceProvider serviceProvider,
            IMessageSerializer messageSerializer,
            ILogger logger,
            InfrastructureContext db)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException("serviceProvider");
            this.messageSerializer = messageSerializer ?? throw new ArgumentNullException("messageSerializer");
            this.logger = logger ?? throw new ArgumentNullException("logger");
            this.db = db ?? throw new ArgumentNullException("db");

        }

        public async Task SendCommandAsync<T>(T command) where T : Command
        {
            logger.Info("[CommandBus->SendCommandAsync] Sending command of type: {0}", typeof(T).Name);

            var entity = new CommandEntity
            {
                Id = command.CommandId,
                CorrelationId = command.CorrelationId,
                ExecutedBy = command.ExecutedBy,
                Type = command.GetType().AssemblyQualifiedName,
                FullName = command.GetType().FullName,
                ScheduledAt = DateTimeOffset.UtcNow,
                Payload = messageSerializer.Serialize(command)
            };

            db.Commands.Add(entity);        

            db.SaveChanges();

            try
            {
                var handlerType = typeof(IHandler<>).MakeGenericType(command.GetType());
                var handler = serviceProvider.GetService(handlerType);

                if (handler != null)
                {
                    await (Task) handler.AsDynamic().HandleAsync(command);
                }
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
    }
}
