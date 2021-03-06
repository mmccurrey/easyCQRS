﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Messaging
{
    public interface IHandler<T> where T: IMessage
    {
        Task HandleAsync(T message);
    }
}
