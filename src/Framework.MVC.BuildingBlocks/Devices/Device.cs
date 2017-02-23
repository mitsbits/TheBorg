using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Borg.Framework.MVC.BuildingBlocks.Devices
{
    public class Device : IDevice
    {
        private const string ControllerKey = "controller";
        private const string ActionKey = "action";
        private const string AreaKey = "area";
        public string FriendlyName { get; set; }

        public string Path { get; set; }
        public string Area { get; protected set; }
        public string Controller { get; protected set; }
        public string Action { get; protected set; }


        protected internal void Populate(ViewContext context)
        {
            Controller = (context.ActionDescriptor.RouteValues.ContainsKey(ControllerKey)) ? context.ActionDescriptor.RouteValues[ControllerKey] : string.Empty;
            Area = (context.ActionDescriptor.RouteValues.ContainsKey(AreaKey)) ? context.ActionDescriptor.RouteValues[AreaKey] : string.Empty;
            Action = (context.ActionDescriptor.RouteValues.ContainsKey(ActionKey)) ? context.ActionDescriptor.RouteValues[ActionKey] : string.Empty;
        }
    }

    public interface IDeviceAccessor<out TDevice> where TDevice : IDevice
    {
        TDevice Device { get; }
    }

    public interface IDevice
    {
        string FriendlyName { get; }

        string Path { get; }

        string Area { get; }
        string Controller { get; }
        string Action { get; }
    }


}