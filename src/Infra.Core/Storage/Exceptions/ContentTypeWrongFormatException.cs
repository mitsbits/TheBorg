using System;

namespace Borg.Infra.Storage
{
    public class ContentTypeWrongFormatException : Exception
    {
        public ContentTypeWrongFormatException(string contentType) : base($"'{contentType}' is not a well formated Content Type string.")
        {
        }
    }
}