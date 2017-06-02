using EasyCQRS.EventSourcing;
using EasyCQRS.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyCQRS.Azure.EventSourcing
{
    internal class SagaStore: ISagaStore
    {
        private readonly ICommandBus bus;
        private readonly ISagaSerializer sagaSerializer;
        private readonly InfrastructureContext db;

        public SagaStore(
            ICommandBus bus,
            ISagaSerializer sagaSerializer,            
            InfrastructureContext db)
        {
            this.bus = bus ?? throw new ArgumentNullException("bus");
            this.sagaSerializer = sagaSerializer ?? throw new ArgumentNullException("sagaSerializer");
            this.db = db ?? throw new ArgumentNullException("db");
        }

        public async Task<T> FindAsync<T>(Guid id) where T : ISaga
        {
            var sagaType = typeof(T).FullName;
            var entity = await db.Sagas.FindAsync(id, sagaType);
            if (entity != null)
            {
                var saga = sagaSerializer.Deserialize<T>(entity.Payload);

                //Saga has pending commands?
                await PutCommandsToBus(entity, saga);

                //If isn't completed then return.
                if (!entity.Completed)
                {
                    return saga;
                }
            }

            return default(T);
        }
        
        public async Task SaveAsync<T>(T saga) where T : ISaga
        {
            var sagaType = saga.GetType().FullName;
            var entity = await db.Sagas.FindAsync(saga.Id, sagaType);
            if (entity == null)
            {
                entity = new SagaEntity
                {
                    Id = saga.Id,
                    CorrelationId = saga.CorrelationId,
                    Type = sagaType,
                    Completed = saga.Completed,
                    Payload = sagaSerializer.Serialize(saga)
                };

                db.Sagas.Add(entity);
            }
            else
            {
                entity.Payload = sagaSerializer.Serialize(saga);
            }

            db.SaveChanges();

            await PutCommandsToBus(entity, saga);
        }

        private async Task PutCommandsToBus(SagaEntity entity, ISaga saga)
        {
            if (entity != null)
            {
                var totalCommands = saga.PendingCommands.Count;
                var queue = new Queue<Command>(saga.PendingCommands);
                var processed = new List<Command>();

                while (queue.Count > 0)
                {
                    try
                    {
                        var command = queue.Peek();

                        await bus.SendCommandAsync(command);

                        saga.PendingCommands.Remove(queue.Dequeue());
                    }
                    catch
                    {
                        if (totalCommands != queue.Count())
                        {
                            entity.Payload = sagaSerializer.Serialize(saga);
                            db.SaveChanges();
                        }

                        throw;
                    }
                }

                entity.Payload = sagaSerializer.Serialize(saga);

                db.SaveChanges();
            }

            return;
        }
    }
}
