using Microsoft.Extensions.Logging;

namespace Borg.Framework.System
{
    public interface ISystemService<out TSettings> : ILoggerFactory where TSettings : BorgSettings
    {
        TSettings Settings { get; }
    }
}