using Borg.Infra.CQRS;
using Borg.Infra.Storage.Assets;
using System.Threading.Tasks;

namespace Borg.Framework.Media.EventHandlers
{
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