using System;

namespace Borg.Infra.EF6.Discovery
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class MapEntityAttribute : Attribute
    {
    }
}