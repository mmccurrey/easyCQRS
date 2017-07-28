using System;
using System.Collections.Generic;
using System.Text;

namespace EasyCQRS
{
    public interface IEntity<TKey> where TKey: class
    {
        TKey Id { get; set; }
    }
}
