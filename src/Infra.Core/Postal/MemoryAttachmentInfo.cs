using Borg.Infra.Storage;
using System.IO;
using System.Linq;

namespace Borg.Infra.Postal
{
    public class MemoryAttachmentInfo : IAttachmentInfo
    {
        public MemoryAttachmentInfo(byte[] data, string contentType, string fileName = "")
        {
            Data = data;
            if (contentType.ToCharArray().Any(x => x == '\\' || x == '/')) throw new ContentTypeWrongFormatException(contentType);
            var mTypes = contentType.Split('\\', '/');
            MediaType = mTypes[0];
            MediaTSubType = mTypes[1];
            FileName = fileName;
            ContentType = contentType;
        }

        public string FileName { get; private set; }
        public byte[] Data { get; private set; }

        public string MediaType { get; private set; }
        public string MediaTSubType { get; private set; }
        public string ContentType { get; private set; }

        public Stream GetStream()
        {
            return new MemoryStream(Data);
        }
    }
}