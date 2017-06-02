using EasyCQRS.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Azure.Messaging
{
    public class CompositeCommandBus: ICommandBus
    {
        private readonly ICommandBus[] buses;

        public CompositeCommandBus(params ICommandBus[] buses)
        {
            this.buses = buses ?? throw new ArgumentNullException("buses");
        }

        public async Task SendCommandAsync<T>(T command) where T : Command
        {
            foreach(var bus in buses)
            {
                await bus.SendCommandAsync(command);
            }
        }
    }
}
