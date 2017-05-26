using System;
using Xunit;
using EasyCQRS.DI;
using System.Linq;

namespace EasyCQRS.Tests
{
    public class DefaultDependencyResolverTests
    {
        [Fact]
        public void DefaultDependencyResolver_ImplementsIDependencyResolverContract()
        {
            Assert.IsAssignableFrom<IDependencyResolver>(new DefaultDependencyResolver());
        }

        [Fact]
        public void DefaultDependencyResolver_ReturnsConcreteTypeFromInterface()
        {
            var sut = new DefaultDependencyResolver();

            sut.Register<IFake, Fake>();

            Assert.IsType(typeof(Fake), sut.Resolve<IFake>());
        }

        [Fact]
        public void DefaultDependencyResolver_ReturnsConcreteTypeFromCtor()
        {
            var sut = new DefaultDependencyResolver();

            sut.Register<IFake>(() => new Fake());

            Assert.IsType(typeof(Fake), sut.Resolve<IFake>());            
        }

        [Fact]
        public void DefaultDependencyResolver_ReturnsTypeFromCtor()
        {
            var sut = new DefaultDependencyResolver();

            sut.Register(() => new Fake());

            Assert.IsType(typeof(Fake), sut.Resolve<Fake>());
        }

        [Fact]
        public void DefaultDependencyResolver_ReturnsTypeWithoutPreviousRegistration()
        {
            var sut = new DefaultDependencyResolver();            

            Assert.IsType(typeof(Fake), sut.Resolve<Fake>());
        }

        [Fact]
        public void DefaultDependencyResolver_ReturnsTypeWithDependencies()
        {
            var sut = new DefaultDependencyResolver();

            sut.Register<IFakeDependency, FakeDependency>();
            sut.Register<IFake, Fake3>();

            Assert.IsType(typeof(Fake3), sut.Resolve<IFake>());
        }

        [Fact]
        public void DefaultDependencyResolver_ReturnsDifferentConcreteTypes()
        {
            var sut = new DefaultDependencyResolver();

            sut.Register<IFakeDependency, FakeDependency>();
            sut.Register<IFake, Fake>();
            sut.Register<IFake, Fake2>();
            sut.Register<IFake, Fake3>();

            var instances = sut.ResolveAll<IFake>();

            Assert.Equal(3, instances.Count());
            Assert.Contains(typeof(Fake), instances.Select(i => i.GetType()));
            Assert.Contains(typeof(Fake2), instances.Select(i => i.GetType()));
            Assert.Contains(typeof(Fake3), instances.Select(i => i.GetType()));            
        }

        [Fact]
        public void DefaultDependencyResolver_ReturnsLatestRegisteredConcreteType()
        {
            var sut = new DefaultDependencyResolver();

            sut.Register<IFakeDependency, FakeDependency>();
            sut.Register<IFake, Fake>();
            sut.Register<IFake, Fake2>();
            sut.Register<IFake, Fake3>();

            Assert.IsType(typeof(Fake3), sut.Resolve<IFake>());
        }

        interface IFakeDependency { }
        interface IFake { }
        class FakeDependency: IFakeDependency { }
        class Fake: IFake { }
        class Fake2: IFake { }
        class Fake3: IFake
        {
            public Fake3(IFakeDependency dependency)
            {
                if(dependency == null)
                {
                    throw new ArgumentNullException("dependency");
                }
            }
        }
    }
}
