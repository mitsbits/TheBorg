﻿using Borg.Infra.EF6.Discovery;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
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
            return entity.IsMapEntity() && Attribute.GetCustomAttribute(entity, typeof(MapSequenceEntityAttribute), false) != null;
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

            if (keyNames.Length > 1)
            {
                //TODO: what the fuck?
                //var args = new ExpandoObject() as IDictionary<string, object>;
                Expression body = parameter;
                for (var i = 0; i < keyNames.Length; i++)
                {
                    var keyName = keyNames[i];
                    // args.Add(keyName, null);
                    var isFirst = i == 0;
                    var isLast = i == keyNames.Length - 1;

                    body = Expression.PropertyOrField(body, keyName);

                    //var propexp = Expression.Property(parameter, type, keyNames.Single());
                    //if (body == null)
                    //{
                    //body = propexp;
                    //}
                    //body = Expression.MemberInit()
                }

                dynamic lambda = Expression.Lambda(body, parameter);
                var d = entityInvocation as dynamic;
                d.HasKey(lambda);
            }
            else
            {
                if (type.GetProperty(keyNames.Single()) == null) throw new ApplicationException($"There is no property {keyNames.Single()} on type {type.Name}. Can not set Entity Key.");
                var body = Expression.Property(parameter, type, keyNames.Single());

                var pInfo = type.GetProperty(keyNames.Single());
                var delegateType = typeof(Func<,>).MakeGenericType(type, pInfo.PropertyType);
                dynamic lambda = Expression.Lambda(delegateType, body, parameter);
                var d = entityInvocation as dynamic;
                d.HasKey(lambda);
            }
        }

        private static LambdaExpression PropertyGetLambda(Type parameterType, string propertyName)
        {
            var parameter = Expression.Parameter(parameterType);
            var memberExpression = Expression.Property(parameter, propertyName);
            var lambdaExpression = Expression.Lambda(memberExpression, parameter);
            return lambdaExpression;
        }

        public static void SetHasDatabaseGeneratedOption(this DbModelBuilder builder, Type type, string field, DatabaseGeneratedOption option, object entityInvocation = null)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrWhiteSpace(field)) throw new ArgumentNullException(nameof(field));
            if (!type.IsMapEntity()) throw new ArgumentOutOfRangeException(nameof(type));
            if (entityInvocation == null)
            {
                entityInvocation = EntityInvocation(builder, type);
            }

            var parameter = Expression.Parameter(type, "entity");
            var body = Expression.Property(parameter, type, field);
            var pInfo = type.GetProperty(field);
            var delegateType = typeof(Func<,>).MakeGenericType(type, pInfo.PropertyType);
            dynamic lambda = Expression.Lambda(delegateType, body, parameter);

            var primitivePropertyConfiguration = (entityInvocation as dynamic).Property(lambda);
            primitivePropertyConfiguration.GetType()
                .InvokeMember("HasDatabaseGeneratedOption", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.GetProperty, null, primitivePropertyConfiguration, new object[] { option });
        }

        private static object EntityInvocation(DbModelBuilder builder, Type type)
        {
            var entityMethod = typeof(DbModelBuilder).GetMethod("Entity");
            var entityInvocation = entityMethod.MakeGenericMethod(type).Invoke(builder, new object[] { });
            return entityInvocation;
        }

        public static void ToTable(this DbModelBuilder builder, Type type, string table, string schema = "", object entityInvocation = null)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (entityInvocation == null)
            {
                entityInvocation = EntityInvocation(builder, type);
            }
            var dynamic = entityInvocation as dynamic;
            dynamic.ToTable(table, schema);
        }
    }
}