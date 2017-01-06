using System;

namespace Borg.Infra.EF6.Discovery
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class MapEntityAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class MapSequenceEntityAttribute : Attribute
    {
        public MapSequenceEntityAttribute(string idField = "Id")
        {
            IdField = idField;
        }

        public string IdField { get; private set; }
    }
}