using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Borg.Framework.Postal;
using Borg.Infra;
using Borg.Infra.CQRS;
using Borg.Infra.DTO;
using Borg.Infra.Messaging;
using Borg.Infra.Postal;
using Microsoft.Extensions.Logging;

namespace Borg.Framework.System.Backoffice
{
    public class BackofficeService : SystemService, IBackofficeService<BorgSettings>
    {
        public BackofficeService(IBorgHost borgHost, ILoggerFactory loggerFactory, BorgSettings settings, ISerializer serializer, ICommandBus commands, IEventBus events, IQueryBus queries, IBroadcaster broadcaster)
            : base((IBorgHost)borgHost, loggerFactory, (BorgSettings)settings, serializer)
        {
            Commands = commands;
            Events = events;
            Queries = queries;
            Broadcaster = broadcaster;
            Emails = Settings.Backoffice.Smtp != null
                ? Emails = new EmailService(settings.Backoffice.Smtp)
                : new NullEmailService();
        }

        public IBroadcaster Broadcaster { get; }

        public ICommandBus Commands { get; }

        public IEventBus Events { get; }

        public IQueryBus Queries { get; }

        public IEmailAccountService Emails { get; }

        public Task Broadcast(string[] topics, Type messageType, object message, TimeSpan? delay = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return Broadcaster.Broadcast(topics, messageType, message, delay, cancellationToken);
        }

        public Task<ICommandResult> Process<TCommand>(TCommand command) where TCommand : ICommand
        {
            return Commands.Process(command);
        }

        public Task Publish<T>(T @event) where T : IEvent
        {
            return Events.Publish(@event);
        }

        public Task<IQueryResult> Fetch<T>(T request) where T : IQueryRequest
        {
            return Queries.Fetch(request);
        }

        public Task<Tidings> SendSingleMail(MailAddress sender, MailAddress recipient, string subject, string body, string messageId = "",
            IEnumerable<IAttachmentInfo> attachments = null, IEnumerable<IMailHeaderInfo> headers = null)
        {
            return Emails.SendSingleMail(sender, recipient, subject, body, messageId, attachments, headers);
        }

        public Task<IEnumerable<Tidings>> SendBatchMails(MailMessage[] messages)
        {
            return Emails.SendBatchMails(messages);
        }

        public MailAddress ServiceAccount()
        {
            return Emails.ServiceAccount();
        }
    }
}