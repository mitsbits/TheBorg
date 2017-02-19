using Borg.Framework.MVC;
using Borg.Framework.MVC.BuildingBlocks.Devices;
using Borg.Framework.System;
using Borg.Infra.CQRS;
using Borg.Infra.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Borg.Framework.MVC
{
    public abstract class FrameworkController : Controller
    {
        public IBroadcaster Broadcaster { get; set; }

        //protected ILoggerFactory LoggerFactory { get; set; }
        protected ISystemService<BorgSettings> System { get; }

        protected ILogger Logger { get; }

        protected FrameworkController(ISystemService<BorgSettings> system)
        {
            System = system;
            Logger = System.CreateLogger(GetType());
            Logger.LogDebug("{@Controller} is born", this);
        }

        protected TContent PageContent<TContent>(TContent content = default(TContent)) where TContent : IPageContent
        {
            if (content == null || content.Equals(default(TContent))) return this.GetContent<TContent>();
            this.SetContent(content);
            return content;
        }

        protected TDevice PageDevice<TDevice>(TDevice device = default(TDevice)) where TDevice : IDevice
        {
            if (device == null || device.Equals(default(TDevice))) return this.GetDevice<TDevice>();
            this.SetDevice(device);
            return device;
        }
    }

    public abstract class BackofficeController : FrameworkController
    {
        protected BackofficeController(ISystemService<BorgSettings> systemService) : base(systemService)
        {
        }

        public IEventBus Events { get; set; }
        public ICommandBus Commands { get; set; }
        public IQueryBus Queries { get; set; }

        #region Pager

        private RequestPager _pager;

        protected RequestPager Pager
        {
            get
            {
                string pageNumerVariableName = System.Settings.Backoffice.Pager.PageVariable;
                string rowCountVariableName = System.Settings.Backoffice.Pager.RowsVariable;

                if (_pager != null) return _pager;

                var p = 1;

                if (!string.IsNullOrWhiteSpace(Request.Query[pageNumerVariableName]))
                    int.TryParse(Request.Query[pageNumerVariableName], out p);

                var r = 10;

                if (!string.IsNullOrWhiteSpace(Request.Query[rowCountVariableName]))
                    int.TryParse(Request.Query[rowCountVariableName], out r);

                _pager = new RequestPager() { Current = p, RowCount = r };
                return _pager;
            }
        }

        protected class RequestPager
        {
            public int Current { get; internal set; }
            public int RowCount { get; internal set; }
        }

        #endregion Pager
    }
}

namespace Borg
{
    internal static class FrameworkControllerExtensions
    {
        private static readonly IPageContent _defaultContent = new PageContent { Title = "default" };

        private static readonly IDevice _defaultDevice = new Device { FriendlyName = "Default", Path = string.Empty };

        public static TContent GetContent<TContent>(this FrameworkController controller) where TContent : IPageContent
        {
            var page = controller.ViewBag.ContentInfo as IPageContent ?? _defaultContent;
            return (TContent)page;
        }

        public static void SetContent<TContent>(this FrameworkController controller, TContent content) where TContent : IPageContent
        {
            controller.ViewBag.ContentInfo = content;
        }

        public static TDevice GetDevice<TDevice>(this FrameworkController controller) where TDevice : IDevice
        {
            var device = controller.ViewBag.DeviceInfo as IDevice ?? _defaultDevice;
            return (TDevice)device;
        }

        public static void SetDevice<TDevice>(this FrameworkController controller, TDevice device) where TDevice : IDevice
        {
            controller.ViewBag.DeviceInfo = device;
        }
    }
}