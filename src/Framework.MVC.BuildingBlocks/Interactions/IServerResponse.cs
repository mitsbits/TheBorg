
using Borg.Infra.Messaging;

namespace Borg.Framework.MVC.BuildingBlocks.Interactions
{
    public interface IServerResponse
    {
        ResponseStatus Status { get; set; }

        string Title { get; set; }

        string Message { get; set; }
    }
}