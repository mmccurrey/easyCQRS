using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.DI
{
    class DefaultDependencyResolver: IDependencyResolver
    {
        private ICollection<DependencyInfo> dependencies = new HashSet<DependencyInfo>();

        public void Register(Type sourceType, Type targetType)
        {
            if (!sourceType.GetTypeInfo().IsAssignableFrom(targetType))
            {
                throw new InvalidOperationException(
                    string.Format(
                    "Cannot register dependency. Mismatch of types. {0} != {1}", sourceType, targetType));
            }
            
            var registration = dependencies
                                .Where(d => d.SourceType == sourceType)
                                .FirstOrDefault();

            if (registration == null)
            {
                registration = new DependencyInfo(sourceType);
                dependencies.Add(registration);
            }

            var registeredType = registration.RegisteredTypes
                                             .Where(t => t.TargetType == targetType)
                                             .FirstOrDefault();

            if (registeredType != null) //Remove previous registration
            {
                registration.RegisteredTypes.Remove(registeredType);
            }

            registration.RegisteredTypes.Add(targetType);
        }

        public void Register<TInterface, TImplementation>() where TImplementation : TInterface
        {
            Register(typeof(TInterface), typeof(TImplementation));
        }

        public void Register<TInterface, TImplementation>(Func<TImplementation> constructor) where TImplementation : TInterface
        {
            var sourceType = typeof(TInterface);
            var targetType = typeof(TImplementation);

            var registration = dependencies
                                .Where(d => d.SourceType == sourceType)
                                .FirstOrDefault();

            if (registration != null) //Remove registration
            {
                dependencies.Remove(registration);
            }

            registration = new DependencyInfo(sourceType, functor: () => constructor());
            dependencies.Add(registration);            
        }

        public void Register<TImplementation>(Func<TImplementation> constructor)
        {
            Register<TImplementation, TImplementation>(constructor);
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type sourceType)
        {
            var targetType = sourceType;
            var registration = dependencies
                                .Where(d => d.SourceType == sourceType)
                                .FirstOrDefault();

            // Find the registration
            if (registration != null)
            {
                if(registration.Functor != null)
                {
                    return registration.Functor();
                }

                targetType = registration.RegisteredTypes.Last().TargetType;
            }                        

            

            // Try to construct the object
            // Step-1: find the constructor (ideally first constructor if multiple constructos present for the type)
            var ctor = targetType.GetTypeInfo().GetConstructors().First();

            // Step-2: find the parameters for the constructor and try to resolve those
            var paramsInfo = ctor.GetParameters().ToList();
            List<object> resolvedParams = new List<object>();
            foreach (ParameterInfo param in paramsInfo)
            {
                Type t = param.ParameterType;
                object res = Resolve(t);
                resolvedParams.Add(res);
            }

            // Step-3: using reflection invoke constructor to create the object
            object retObject = ctor.Invoke(resolvedParams.ToArray());

            return retObject;
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return ResolveAll(typeof(T)).Select(t => (T)t);
        }

        public IEnumerable<object> ResolveAll(Type sourceType)
        {
            var objects = new List<object>();            
            var registration = dependencies
                                .Where(d => d.SourceType == sourceType)
                                .FirstOrDefault();

            // Find the registration
            if (registration != null)
            {
                foreach (var registeredType in registration.RegisteredTypes)
                {
                    var resolvedType = Resolve(registeredType.TargetType);

                    if (resolvedType == null)
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                "Cannot resolve type: {0} from {1}", registeredType.TargetType, sourceType));
                    }

                    objects.Add(resolvedType);
                }
            }
            else
            {
                var resolvedType = Resolve(sourceType);
                if (resolvedType == null)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "Cannot resolve type: {0}", sourceType));
                }

                objects.Add(resolvedType);
            }

            return objects;
        }
    }

}
