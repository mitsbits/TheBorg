using Borg.Infra.EF6;
using Borg.Infra.EF6.Discovery;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Borg
{
    internal static class MapEntityExtensions
    {
        public static bool IsMapEntity(this Type entity)
        {
            if (entity.IsInterface || entity.IsAbstract) return false;
            if (entity.GetInterfaces().Any(i => i == typeof(IMapEntity))) return true;
            return Attribute.GetCustomAttribute(entity, typeof(MapEntityAttribute), false) != null;
        }

        public static bool IsSequenceEntity(this Type entity)
        {
            return entity.IsMapEntity() && entity.GetInterfaces().Any(i => i == typeof(ISequenceEntity));
        }
    }

    internal static class EntityConfigurationWrapperExtensions
    {
        public static void SetKeys(this DbModelBuilder builder, Type type, string[] keyNames, object entityInvocation = null)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (keyNames == null) throw new ArgumentNullException(nameof(keyNames));
            if (!keyNames.Any()) throw new ArgumentNullException(nameof(keyNames));
            if (!type.IsMapEntity()) throw new ArgumentOutOfRangeException(nameof(type));
            if (entityInvocation == null)
            {
                var entityMethod = typeof(DbModelBuilder).GetMethod("Entity");
                entityInvocation = entityMethod.MakeGenericMethod(type).Invoke(builder, new object[] { });
            }
    
            var parameter = Expression.Parameter(type, "entity");
            //TODO: implement component key
            if (keyNames.Length > 1) throw new NotImplementedException("need to implement for component key");
            var body = Expression.Property(parameter, type, keyNames.Single());
            var pInfo = type.GetProperty(keyNames.Single());
            var delegateType = typeof(Func<,>).MakeGenericType(type, pInfo.PropertyType);
            dynamic lambda = Expression.Lambda(delegateType, body, parameter);
            var keyMethod = typeof(EntityTypeConfiguration<>).GetMethod("HasKey");
            var generickeyMethod = keyMethod.MakeGenericMethod( pInfo.PropertyType);
            var arg = Convert.ChangeType(lambda, generickeyMethod.GetGenericArguments()[0].DeclaringType);
            keyMethod.Invoke(entityInvocation, new object[] { arg });
        }

        public static void SetHasDatabaseGeneratedOption(this DbModelBuilder builder, Type type, string field, DatabaseGeneratedOption option, object entityInvocation = null)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrWhiteSpace(field)) throw new ArgumentNullException(nameof(field));
            if (!type.IsMapEntity()) throw new ArgumentOutOfRangeException(nameof(type));
            if (entityInvocation == null)
            {
                var entityMethod = typeof(DbModelBuilder).GetMethod("Entity");
                entityInvocation = entityMethod.MakeGenericMethod(type).Invoke(builder, new object[] { });
            }

            var parameter = Expression.Parameter(type, "entity");
            var body = Expression.Property(parameter, type, field);
            var pInfo = type.GetProperty(field);
            var delegateType = typeof(Func<,>).MakeGenericType(type, pInfo.PropertyType);
            dynamic lambda = Expression.Lambda(delegateType, body, parameter);
            var primitivePropertyConfiguration = entityInvocation.GetType()
                .InvokeMember("Property", BindingFlags.Public | BindingFlags.Instance, null, entityInvocation, new[] { lambda });
            primitivePropertyConfiguration.GetType()
                .InvokeMember("HasDatabaseGeneratedOption", BindingFlags.Public | BindingFlags.Instance, null,
                primitivePropertyConfiguration, new object[] { option });
        }
    }
}