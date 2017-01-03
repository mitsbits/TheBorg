using Borg.Infra.DTO;
using Borg.Infra.Postal;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borg.Framework.Postal
{
    public class EmailService : IEmailAccountService
    {
        //private readonly ILogger _log = Log.Logger.ForContext(typeof(EmailService));
        protected readonly SmtpSpec _smtpSpec;

        //protected readonly ImapInfo _imapInfo;
        private readonly MailAddress _serviceAccount;

        public EmailService(SmtpSpec smtpSpec, MailAddress serviceAccount = null)
        {
            if (smtpSpec == null) throw new ArgumentNullException(nameof(smtpSpec));
            _smtpSpec = smtpSpec;
            _serviceAccount = serviceAccount ?? new MailAddress(_smtpSpec.UseName);
        }

        public async Task<Tidings> SendSingleMail(MailAddress sender, MailAddress recipient, string subject, string body,
            string messageId, IEnumerable<IAttachmentInfo> attachments, IEnumerable<IMailHeaderInfo> headers)
        {
            var message = CreateMessage(sender, recipient, subject, body, messageId, attachments, headers);
            return (await SendMessage(new[] { message })).First();
        }

        private async Task<IEnumerable<Tidings>> SendMessage(MimeMessage[] messages)
        {
            // _log.Debug("Sending {@Messages}", messages);
            var sentDictionary = new List<Tidings>();

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_smtpSpec.Host, _smtpSpec.Port, (_smtpSpec.SSL) ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.None);

                    if (!string.IsNullOrWhiteSpace(_smtpSpec.UseName))
                    {
                        await client.AuthenticateAsync(_smtpSpec.UseName, _smtpSpec.Password);
                    }
                    // _log.Debug("Aquired {@Client}", client);

                    await LoopMessagesToSend(messages, client, sentDictionary);
                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    throw;
                    //  _log.Error(ex, "{@Client}", ex);
                }
            }

            return sentDictionary;
        }

        private async Task LoopMessagesToSend(MimeMessage[] messages, SmtpClient client, List<Tidings> sentDictionary)
        {
            for (var i = 0; i < messages.Count(); i++)
            {
                var message = messages[i];
                // _log.Debug("{@Client} is about to send {@Message}", client, message);
                try
                {
                    await client.SendAsync(message);
                    var tidings = new SuccessEmailAccountResponse(message.MessageId, message.ToString());
                    foreach (var smtpInfoTrackHeader in _smtpSpec.TrackHeadersOnSend)
                    {
                        tidings.Add(smtpInfoTrackHeader, message.Headers[smtpInfoTrackHeader] ?? "");
                    }

                    sentDictionary.Add(tidings);
                    //  _log.Debug("{@Client} delivered {@Message} with {@Result}", client, message, tidings);
                }
                catch (Exception ex)
                {
                    var tidings = new FailedEmailAccountResponse(message.MessageId, ex.ToString());
                    foreach (var smtpInfoTrackHeader in _smtpSpec.TrackHeadersOnSend)
                    {
                        tidings.Add(smtpInfoTrackHeader, message.Headers[smtpInfoTrackHeader] ?? "");
                    }
                    sentDictionary.Add(tidings);

                    //   _log.Debug("{@Client} filed to deliver {@Message} with {@Result}", client, message, tidings);
                }
            }
        }

        public virtual MailAddress ServiceAccount()
        {
            return _serviceAccount;
        }

        public async Task<IEnumerable<Tidings>> SendBatchMails(MailMessage[] messages)
        {
            // _log.Debug("Prepariong {Messages} Messages for batch delivery. {@BatchSize}", messages.Length, _smtpInfo.BatchSize);
            List<MimeMessage> mimeMessages = new List<MimeMessage>();
            foreach (var message in messages)
            {
                mimeMessages.Add(CreateMessage(message.Sender, message.Recipient, message.Subject, message.Body, message.MessageId, message.Attachments, message.Headers));
            }

            var result = new List<Tidings>();
            var parts = (mimeMessages.Count / _smtpSpec.BatchSize) + 1;
            var i = 0;
            var splits = from item in mimeMessages
                         group item by i++ % parts into part
                         select part.AsEnumerable();

            // _log.Debug("Prepared {Batches} Batches", splits.Count());
            foreach (var split in splits)
            {
                //_log.Debug("Preparing to send {@Batch}", split);
                var results = await SendMessage(split.ToArray());
                //_log.Debug("Sending {@Batch} resulted in {@Results}", split, results);
                result.AddRange(results);
            }
            //_log.Debug("Sent {@Messages} resulted in {@Results}", mimeMessages, result);
            return result;
        }

        private MimeMessage CreateMessage(MailAddress sender, MailAddress recipient, string subject, string body, string messageId, IEnumerable<IAttachmentInfo> attachments, IEnumerable<IMailHeaderInfo> headers)
        {
            // _log.Debug("About to create message - {@Sender}, {@Recipient}, {@Subject}, {@Body}, {@MessageId}, {@Attachments}, {@Headers}", sender, recipient, subject, body, messageId, attachments, headers);
            if (!recipient.Email.WellFormedEmail()) throw new EmailFormatException(recipient.Email);
            var attachmentInfos = (attachments == null) ? new IAttachmentInfo[0] : attachments as IAttachmentInfo[] ?? attachments.ToArray();
            var hasAttachmets = attachmentInfos.Any();
            var mailHeaderInfos = (headers == null) ? new IMailHeaderInfo[0] : headers as IMailHeaderInfo[] ?? headers.ToArray();
            var hasHeaders = mailHeaderInfos.Any();
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(sender.DisplayName, sender.Email));
            message.To.Add(new MailboxAddress(recipient.DisplayName, recipient.Email));
            message.ReplyTo.Add(new MailboxAddress(sender.DisplayName, sender.Email));
            message.Headers.Add("Return-Path", ServiceAccount().Email);
            message.Headers.Add("Errors-To", ServiceAccount().Email);
            message.Subject = subject;

            if (!string.IsNullOrWhiteSpace(messageId))
            {
                message.MessageId = MimeUtils.GenerateMessageId(messageId);
            }

            var multipart = new Multipart("alternative");
            var plain = new TextPart();
            plain.SetText(Encoding.UTF8, "");
            var html = new TextPart("html");
            html.SetText(Encoding.UTF8, body);
            multipart.Add(plain); // always add plain first
            multipart.Add(html);

            if (hasAttachmets)
            {
                foreach (var info in attachmentInfos)
                {
                    var attachment = new MimePart(info.MediaType, info.MediaTSubType)
                    {
                        ContentObject = new ContentObject(info.GetStream(), ContentEncoding.Default),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = info.FileName
                    };
                    multipart.Add(attachment);
                }
            }

            if (hasHeaders)
            {
                foreach (var header in mailHeaderInfos)
                {
                    message.Headers.Add(header.HeaderId, header.Encoding, header.Value);
                }
            }

            message.Body = multipart;
            //_log.Debug("Created {@Message}", message);
            return message;
        }
    }
}