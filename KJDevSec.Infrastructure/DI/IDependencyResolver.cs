using System;
using System.Collections.Generic;

namespace EasyCQRS.DI
{
    public interface IDependencyResolver
    {
        object Resolve(Type type);
        T Resolve<T>();
        IEnumerable<T> ResolveAll<T>();
        IEnumerable<object> ResolveAll(Type type);

        void Register<TInterface, TImplementation>() where TImplementation : TInterface;
        void Register<TInterface, TImplementation>(Func<TImplementation> constructor) where TImplementation : TInterface;
        void Register<TImplementation>(Func<TImplementation> constructor);
    }
}