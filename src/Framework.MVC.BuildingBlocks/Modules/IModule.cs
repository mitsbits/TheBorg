using System.Collections.Generic;

namespace Borg.Framework.MVC.BuildingBlocks.Modules
{
    internal interface IModule<out TData> : IModule where TData : IDictionary<string, string>
    {
        TData Parameters { get; }
    }

    internal interface IModule
    {
        string FriendlyName { get; }
        ModuleType ModuleType { get; }
    }
}