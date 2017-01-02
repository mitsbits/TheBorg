using System.Collections.Generic;

namespace Borg.Infra.Postal
{
    public class MailMessage
    {
        public MailAddress Sender { get; set; }
        public MailAddress Recipient { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public IEnumerable<IAttachmentInfo> Attachments { get; set; }
        public IEnumerable<IMailHeaderInfo> Headers { get; set; }
        public string MessageId { get; set; }
        public string[] ReferenceMessageIds { get; set; }

        public MailMessage()
        { }

        public MailMessage(MailAddress sender, MailAddress recipient, string subject, string body, string messageId, IEnumerable<IAttachmentInfo> attachments, IEnumerable<IMailHeaderInfo> headers)
        {
            Sender = sender;
            Recipient = recipient;
            Subject = subject;
            Body = body;
            Attachments = attachments;
            Headers = headers;
            MessageId = messageId;
        }
    }
}