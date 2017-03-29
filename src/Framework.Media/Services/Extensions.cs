using Borg.Framework.Media;
using Borg.Infra.Relational;
using Borg.Infra.Storage;
using System.Linq;
using System.Threading.Tasks;

namespace Borg
{
    public static class IFileSpecExtensions
    {
        private static readonly string[] _images = { "image/jpeg" };

        public static bool IsImage(this IFileSpec spec)
        {
            return _images.Contains(spec.MimeType.ToLowerInvariant());
        }
    }

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