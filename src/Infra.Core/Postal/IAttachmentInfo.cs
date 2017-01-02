using System.IO;

namespace Borg.Infra.Postal
{
    public interface IAttachmentInfo
    {
        string FileName { get; }
        string MediaType { get; }
        string MediaTSubType { get; }
        string ContentType { get; }

        Stream GetStream();
    }
}