using Borg.Framework.MVC.BuildingBlocks.Devices;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Borg.Framework.MVC
{
    public class DefaultDeviceAccessor : IPageContentAccessor<IPageContent>, IDeviceAccessor<IDevice>, IViewContextAware
    {
        private ViewContext _viewContext;
        private IPageContent _page;
        private static readonly IPageContent _defaultContent = new PageContent() { Title = "default" };

        private IDevice _device;
        private static readonly IDevice _defaultDevice = new Device() { FriendlyName = "Default", Path = string.Empty };

        public void Contextualize(ViewContext viewContext)
        {
            _viewContext = viewContext;
        }

        public IPageContent Page
        {
            get
            {
                var page = _page ?? (_page = _viewContext.ViewBag.ContentInfo as IPageContent);
                if (page != null) return page;
                page = _defaultContent;
                _viewContext.ViewBag.ContentInfo = page;
                return page;
            }
        }

        public IDevice Device
        {
            get
            {
                var device = _device ?? (_device = _viewContext.ViewBag.DeviceInfo as IDevice);
                if (device != null) return device;
                device = _defaultDevice;
                _viewContext.ViewBag.DeviceInfo = device;
                return device;
            }
        }
    }
}