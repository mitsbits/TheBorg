using System.Threading.Tasks;

namespace Borg.Famework.Backoffice.Identity.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
