using System.Threading.Tasks;

namespace Borg.Framework.Identity.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
