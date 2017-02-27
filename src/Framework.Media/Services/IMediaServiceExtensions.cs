using Borg.Framework.Media;

using Borg.Infra.Relational;
using System.Linq;
using System.Threading.Tasks;

namespace Borg
{
    internal static class IMediaServiceExtensions
    {
        public static async Task<AssetSpec> Get(this IMediaService service, int id)
        {
            var hits = await service.Assets(x => x.Id == id, 1, 1,
                new[]
                {
                    new OrderByInfo<AssetSpec>()
                    {
                        Ascending = true,
                        Property = x => x.Id
                    },
                }, spec => spec.Versions);
            return hits.Records.FirstOrDefault();
        }
    }
}