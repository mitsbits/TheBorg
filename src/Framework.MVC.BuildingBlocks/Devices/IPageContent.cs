namespace Borg.Framework.MVC.BuildingBlocks.Devices
{
    public interface IPageContent
    {
        string Title { get; }
        string Subtitle { get; }
        string[] Body { get; }
    }

    public class PageContent : IPageContent
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }

        public string[] Body { get; set; }
    }

    public interface IPageContentAccessor<out TPage> where TPage : IPageContent
    {
        TPage Page { get; }
    }

    public interface IDeviceAccessor<out TDevice> where TDevice : IDevice
    {
        TDevice Device { get; }
    }

    public interface IDevice
    {
        string FriendlyName { get; }

        string Path { get; }
    }

    public class Device : IDevice
    {
        public string FriendlyName { get; set; }

        public string Path { get; set; }
    }
}