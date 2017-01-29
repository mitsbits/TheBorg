namespace Borg.Framework.MVC.BuildingBlocks.Devices
{
    public class Device : IDevice
    {
        public string FriendlyName { get; set; }

        public string Path { get; set; }
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
}