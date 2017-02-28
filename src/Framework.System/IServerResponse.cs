
using Borg.Infra.Messaging;

namespace Borg.Framework.System
{
    public interface IServerResponse
    {
        ResponseStatus Status { get; set; }

        string Title { get; set; }

        string Message { get; set; }
    }
}