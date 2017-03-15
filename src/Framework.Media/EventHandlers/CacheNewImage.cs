using Borg.Infra.CQRS;
using Borg.Infra.Storage.Assets;
using System.Threading.Tasks;
using Borg.Framework.System;

namespace Borg.Framework.Media.EventHandlers
{
    [BorgModule]
    public class CacheNewImage : IHandlesEvent<FileAddedToAssetEvent<int>>
    {
        public Task Handle(FileAddedToAssetEvent<int> message)
        {
            //TODO: do
            var m = message;
            return Task.CompletedTask;
        }
    }
}