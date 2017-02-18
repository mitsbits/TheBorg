using System;

namespace Borg.Framework.Services.Navigation
{
    [Flags]
    public enum NavigationElementRole
    {
        Devider = 2,
        Label = 4,
        Header = 8,
        Anchor = 16
    }
}