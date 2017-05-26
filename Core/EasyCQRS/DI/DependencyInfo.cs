using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.DI
{
    class DependencyInfo
    {
        private DependencyInfo()
        {
            this.RegisteredTypes = new HashSet<DependencyTypeInfo>();
        }

        public DependencyInfo(Type type)
            : this()
        {
            if(type == null)
            {
                throw new ArgumentNullException("type");
            }

            SourceType = type;
        }

        public DependencyInfo(Type type, Type targetType)
            : this(type)
        {
            if(targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }

            RegisteredTypes.Add(new DependencyTypeInfo(targetType));
        }

        public DependencyInfo(Type type, Func<object> functor)
            : this(type)
        {
            if(functor == null)
            {
                throw new ArgumentNullException("functor");
            }

            Functor = functor;
        }

        public Type SourceType { get; private set; }
        public ICollection<DependencyTypeInfo> RegisteredTypes { get; private set; }
        public Func<object> Functor { get; private set; }

        public class DependencyTypeInfo
        {
            public DependencyTypeInfo(string name, Type targetType)
            {
                if(name == null)
                {
                    throw new ArgumentNullException("name");
                }

                if(targetType == null)
                {
                    throw new ArgumentNullException("targetType");
                }

                Name = name;
                TargetType = targetType;
            }

            public DependencyTypeInfo(Type type)
                : this(Guid.NewGuid().ToString(), type)
            {
            }

            public string Name { get; private set; }
            public Type TargetType { get; private set; }

            public override bool Equals(object obj)
            {
                var _obj = obj as DependencyTypeInfo;
                if (_obj == null)
                {
                    return false;
                }

                return Name.Equals(_obj.Name) && TargetType.Equals(_obj.TargetType);
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode();
            }

            public override string ToString()
            {
                return string.Format("[({0}) {1}]", Name, TargetType);
            }

            public static implicit operator DependencyTypeInfo(Type t)
            {
                return new DependencyTypeInfo(t);
            }            
        }
    }    
}
