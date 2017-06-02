using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Messaging
{
    public interface ICommandBus
    {
        Task SendCommandAsync<T>(T command) where T : Command;
    }
}
