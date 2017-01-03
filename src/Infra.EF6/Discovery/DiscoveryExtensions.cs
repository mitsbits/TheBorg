using Borg.Infra.EF6.Discovery;
using System;
using System.Linq;

namespace Borg
{
    internal static class MapEntityExtensions
    {
        public static bool IsMapEntity(this Type entity)
        {
            if (entity.IsInterface || entity.IsAbstract) return false;
            if (entity.GetInterfaces().Any(i => i == typeof(IMapEntity))) return true;
            Attribute[] attrs = Attribute.GetCustomAttributes(entity);  
            return  attrs.Any(a => a.GetType() == typeof(MapEntityAttribute));
        }
    }
}