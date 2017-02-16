using System.Threading.Tasks;

namespace Borg.Famework.Backoffice.Identity.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
