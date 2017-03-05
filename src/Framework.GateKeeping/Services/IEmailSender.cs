using System.Threading.Tasks;

namespace Borg.Framework.GateKeeping.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
