using Borg.Infra.DTO;
using Borg.Infra.Postal;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Borg.Infra.Postal
{
    public interface IEmailAccountService
    {
        Task<Tidings> SendSingleMail(MailAddress sender, MailAddress recipient, string subject, string body, string messageId = "", IEnumerable<IAttachmentInfo> attachments = null, IEnumerable<IMailHeaderInfo> headers = null);

        Task<IEnumerable<Tidings>> SendBatchMails(MailMessage[] messages);

        MailAddress ServiceAccount();
    }
}

