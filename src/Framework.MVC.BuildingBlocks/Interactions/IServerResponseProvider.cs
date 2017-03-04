using System.Collections.Generic;

namespace Borg.Framework.MVC.BuildingBlocks.Interactions
{
    public interface IServerResponseProvider
    {
        ICollection<IServerResponse> Messages { get; }
    }
}