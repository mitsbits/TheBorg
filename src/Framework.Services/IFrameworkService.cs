using System;

namespace Borg.Framework
{
    internal class FrameworkEventArgs : EventArgs
    {
    }

    internal interface IFrameworkService
    {
    }

    internal abstract class FrameworkService : IFrameworkService { }
}