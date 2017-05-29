using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS
{
    public static class ObjectExtensions
    {
        public static dynamic AsDynamic(this object o)
        {
            return ReflectionMagic.PrivateReflectionUsingDynamicExtensions.AsDynamic(o);
        }
    }
}
