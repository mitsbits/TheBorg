using System.Threading.Tasks;

namespace Borg.Framework.Backoffice.Identity.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
