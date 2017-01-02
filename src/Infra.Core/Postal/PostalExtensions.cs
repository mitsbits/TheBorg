using Borg.Infra.Postal;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Borg
{
    public static class EmailExtensions
    {
        public static bool WellFormedEmail(this string email)
        {
            Regex g = new Regex(@"^(([^<>()[\]\\.,;:\s@\""]+"
                                + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                                + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                                + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                                + @"[a-zA-Z]{2,}))$");
            return g.Match(email).Success;
        }
    }

    public static class IEmailAccountServiceExtensions
    {
        public static Task SendSingleMail(this IEmailAccountService service, MailAddress recipient, string subject, string body, string messageId = "", IEnumerable<IAttachmentInfo> attachments = null, IEnumerable<IMailHeaderInfo> headers = null)
        {
            var serviceAccount = service.ServiceAccount();
            return service.SendSingleMail(serviceAccount, recipient, subject, body, messageId, attachments, headers);
        }

        public static Task SendSingleMail(this IEmailAccountService service, MailMessage message)
        {
            return service.SendSingleMail(message.Sender, message.Recipient, message.Subject, message.Body,
                message.MessageId, message.Attachments, message.Headers);
        }
    }
}