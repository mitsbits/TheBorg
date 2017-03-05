using System;
using System.Reflection;

namespace Borg
{
    public static class ReflectionExtensions
    {
        public static Type BaseType(this Type type)
        {
            Type result = null;
#if NET46
            result = type.BaseType;
#endif
#if NETSTANDARD1_6
            result = type.GetTypeInfo().BaseType;
#endif
            return result;
        }
    }
}