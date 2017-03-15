using System.Collections.Generic;
using System.Threading.Tasks;
using Borg.Framework.Postal;
using Borg.Framework.System;
using Borg.Infra.DTO;
using Borg.Infra.Postal;
using Microsoft.Extensions.Logging;

namespace Borg.Framework.GateKeeping.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    [BorgModule]
    public class AuthMessageSender : IEmailAccountService, ISmsSender
    {
        private readonly ILogger<AuthMessageSender> _logger;
        private readonly IEmailAccountService emails = new NullEmailService();

        public AuthMessageSender(ILogger<AuthMessageSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            // Plug in your email service here to send an email.
            _logger.LogInformation("Email: {email}, Subject: {subject}, Message: {message}", email, subject, message);
            return Task.FromResult(0);
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            _logger.LogInformation("SMS: {number}, Message: {message}", number, message);
            return Task.FromResult(0);
        }

        public Task<Tidings> SendSingleMail(MailAddress sender, MailAddress recipient, string subject, string body, string messageId = "",
            IEnumerable<IAttachmentInfo> attachments = null, IEnumerable<IMailHeaderInfo> headers = null)
        {
            return emails.SendSingleMail(sender, recipient, subject, body, messageId, attachments, headers);
        }

        public Task<IEnumerable<Tidings>> SendBatchMails(MailMessage[] messages)
        {
            return emails.SendBatchMails(messages);
        }

        public MailAddress ServiceAccount()
        {
            return emails.ServiceAccount();
        }
    }
}