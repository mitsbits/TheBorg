using System;
using Microsoft.AspNetCore.Mvc;

namespace Borg.Framework.MVC.BuildingBlocks.Devices
{
    public static class DeviceExtensions
    {
        public static Func<int, string> PagerAnchorGenerator(this IUrlHelper helper, IDevice device)
        {     
            return (i) => helper.Action(device.Action, device.Controller, new { p = i, area = device.Area });
        }
    }
}
