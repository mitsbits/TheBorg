using System;
using System.Threading;
using System.Threading.Tasks;

namespace Borg.Infra.Messaging
{
    public class InMemoryMessageBus : MessageBusBase, IMessageBus
    {
        public InMemoryMessageBus(/*ILoggerFactory loggerFactory = null*/) : base(/*loggerFactory*/)
        {
        }

        public string Topic => string.Empty;
        public override bool SupportsTopics => false;

        public override Task PublishAsync(Type messageType, object message, TimeSpan? delay = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (message == null)
                return Task.CompletedTask;

            if (delay.HasValue && delay.Value > TimeSpan.Zero)
                return AddDelayedMessageAsync(messageType, message, delay.Value);

            Task.Run(async () => await SendMessageToSubscribersAsync(messageType, message.Copy()).AnyContext(), cancellationToken);
            return Task.CompletedTask;
        }

        public Task Stop(CancellationToken token = default(CancellationToken))
        {
            if (token.IsCancellationRequested) throw new OperationCanceledException();
            _subscribers.Clear();
            return Task.CompletedTask;
        }
    }
}