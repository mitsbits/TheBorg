using System;
using System.Collections.Generic;
using System.Text;

namespace Borg.Framework.System
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class BorgModuleAttribute : Attribute
    {
    }
}