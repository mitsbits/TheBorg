using Borg.Infra.CQRS;
using Borg.Infra.Messaging;
using System;
using System.Threading.Tasks;

namespace Borg.Framework.Services.Notifications
{
    public class NotifyUserCommand : ICommand
    {
        public NotifyUserCommand(string recipientIdentifier, ResponseStatus responseStatus, string title, string message)
        {
            RecipientIdentifier = recipientIdentifier;
            ResponseStatus = responseStatus;
            Title = title;
            Message = message;
        }

        public string Message { get; }

        public string RecipientIdentifier { get; }

        public ResponseStatus ResponseStatus { get; }

        public string Title { get; }
    }

    public class NotifyUserCommandHandler : IHandlesCommand<NotifyUserCommand>
    {
        private readonly IUserNotificationsStore _service;

        public NotifyUserCommandHandler(IUserNotificationsStore service)
        {
            _service = service;
        }

        public async Task<ICommandResult> Execute(NotifyUserCommand message)
        {
            try
            {
                await _service.Add(message.RecipientIdentifier, message.ResponseStatus, message.Title, message.Message);
                return CommandResult.Create(true);
            }
            catch (Exception e)
            {
                return CommandResult.Create(e);
            }
        }
    }
}