using System.Text;

namespace Borg.Infra.Postal
{
    public interface IMailHeaderInfo
    {
        string HeaderId { get; }
        string Value { get; }
        Encoding Encoding { get; }
    }
}