using System.IO;

namespace Borg.Infra.Postal
{
    public class FileAttachmentInfo : IAttachmentInfo
    {
        public FileAttachmentInfo(string path, string contentType = "", string fileName = "")
        {
            FilePath = path;
            FileName = (string.IsNullOrWhiteSpace(fileName)) ? Path.GetFileName(path) : fileName;
            if (string.IsNullOrWhiteSpace(contentType)) contentType = path.GetMimeType();
            var mTypes = contentType.Split('\\', '/');
            MediaType = mTypes[0];
            MediaTSubType = (mTypes.Length > 1) ? mTypes[1] : string.Empty;
            ContentType = contentType;
        }

        public string FileName { get; private set; }
        public string FilePath { get; private set; }

        public string MediaType { get; private set; }
        public string MediaTSubType { get; private set; }
        public string ContentType { get; private set; }

        public Stream GetStream()
        {
            return File.OpenRead(FilePath);
        }
    }
}