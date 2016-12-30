using System;
using System.Threading.Tasks;

namespace Borg.Infra.Core
{
    public interface ISerializer
    {
        Task<object> DeserializeAsync(byte[] data, Type objectType);

        Task<byte[]> SerializeAsync(object value);
    }
}