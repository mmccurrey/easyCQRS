using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KJDevSec.Messaging
{
    public interface IBus
    {
        Task SendCommandAsync<T>(T command) where T : Command;
        Task SendEventAsync<T>(T @event) where T : Event;
    }
}
