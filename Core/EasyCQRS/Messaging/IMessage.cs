using System;

namespace EasyCQRS.Messaging
{
    public interface IMessage
    {
        string MessageId { get; }
    }
}