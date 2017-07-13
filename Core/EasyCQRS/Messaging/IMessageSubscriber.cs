using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Messaging
{
    public interface IMessageSubscriber
    {
        IDisposable Subscribe<T>([CallerMemberName] string name = null) where T: class, IMessage;
    }
}
