using System.Collections.Generic;

namespace Borg.Framework.System
{
    public interface IServerResponseProvider
    {
        ICollection<IServerResponse> Messages { get; }
    }
}