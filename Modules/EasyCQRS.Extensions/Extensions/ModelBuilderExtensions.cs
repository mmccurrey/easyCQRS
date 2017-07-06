using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyCQRS
{
    public static class ModelBuilderExtensions
    {
        private static IEnumerable<Type> GetEntityConfigurations(this Assembly assembly, string filterNamespace = "")
        {
            return assembly
                .GetTypes()
                .Where(x =>
                {
                    var ti = x.GetTypeInfo();
                    return !ti.IsAbstract && ti.GetInterfaces().Any(y => y.GetTypeInfo().IsGenericType &&
                    y.GetGenericTypeDefinition() == typeof(IDbEntityConfiguration<>))
                    && (string.IsNullOrWhiteSpace(filterNamespace) || x.Namespace.ToLower().Trim() == filterNamespace.ToLower().Trim());
                })
                .ToList();
        }

        public static void AddConfiguration<TEntity>(
          this ModelBuilder modelBuilder,
          DbEntityConfiguration<TEntity> dbEntityConfiguration) where TEntity : class
        {
            modelBuilder.Entity<TEntity>(dbEntityConfiguration.Configure);
        }

        /// <summary>
        /// Specify a type in an assembly that has DbConfigurations you want to add
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="modelBuilder"></param>
        public static void AddAssemblyConfiguration<TType>(this ModelBuilder modelBuilder, string filterNamespace = "") where TType : class
        {
            var assembly = typeof(TType).GetTypeInfo().Assembly;
            var entityConfigs = assembly.GetEntityConfigurations(filterNamespace);
            foreach (var entityConfig in entityConfigs.Select(Activator.CreateInstance).Cast<IDbEntityConfiguration>())
            {
                entityConfig.Configure(modelBuilder);
            }
        }

        /// <summary>
        /// Specify a type in an assembly that has DbConfigurations you want to add
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="modelBuilder"></param>
        public static void AddAssemblyConfiguration(this ModelBuilder modelBuilder, Assembly assembly, string filterNamespace = "")
        {
            var entityConfigs = assembly.GetEntityConfigurations(filterNamespace);
            foreach (var entityConfig in entityConfigs.Select(Activator.CreateInstance).Cast<IDbEntityConfiguration>())
            {
                entityConfig.Configure(modelBuilder);
            }
        }
    }

    public interface IDbEntityConfiguration
    {
        void Configure(ModelBuilder modelBuilder);
    }

    public interface IDbEntityConfiguration<TEntity> : IDbEntityConfiguration where TEntity : class
    {
        void Configure(EntityTypeBuilder<TEntity> modelBuilder);
    }

    public abstract class DbEntityConfiguration<TEntity> : IDbEntityConfiguration<TEntity> where TEntity : class
    {
        public abstract void Configure(EntityTypeBuilder<TEntity> entity);
        public void Configure(ModelBuilder modelBuilder)
        {
            Configure(modelBuilder.Entity<TEntity>());
        }
    }
}