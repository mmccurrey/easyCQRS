using Autofac;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KJDevSec.DI
{    
    class AutofactDependencyResolver : IDependencyResolver
    {
        private readonly ContainerBuilder builder;
        private IContainer container;

        public AutofactDependencyResolver(ContainerBuilder container)
        {
            this.builder = container;
            this.container = builder.Build();
        }        

        public object Resolve(Type type)
        {
            return container.Resolve(type);
        }

        public T Resolve<T>()
        {
            return (T) this.Resolve(typeof(T));
        }


        public IEnumerable<T> ResolveAll<T>()
        {
            return container.Resolve<IEnumerable<T>>();
        }

        public IEnumerable<object> ResolveAll(Type type)
        {
            var t = typeof(IEnumerable<>);
            var gt = t.MakeGenericType(type);

            return Convert<object>((IEnumerable)container.Resolve(gt));
        }


        public void Register<TInterface, TImplementation>() where TImplementation : TInterface
        {
            builder.RegisterType<TImplementation>().As<TInterface>();

            UpdateContainer();
        }

        public void Register<TInterface, TImplementation>(Func<TImplementation> constructor) where TImplementation : TInterface
        {
            builder.Register<TImplementation>((c) => constructor()).As<TInterface>();

            UpdateContainer();
        }
        
        public void Register<TImplementation>(Func<TImplementation> constructor)
        {
            builder.Register<TImplementation>((c) => constructor());

            UpdateContainer();
        }

        void UpdateContainer()
        {
            //container = builder.Build();
            #pragma warning disable 612, 618
            builder.Update(container);
            #pragma warning restore 612, 618
        }

        IEnumerable<T> Convert<T>(IEnumerable source)
        {
            foreach (var item in source)
            {
                yield return (T)item;
            }
        }
    }
}
