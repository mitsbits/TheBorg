using System;
using System.Threading;
using System.Threading.Tasks;

namespace Borg.Infra.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishAsync(Type messageType, object message, TimeSpan? delay = null, CancellationToken cancellationToken = default(CancellationToken));

        bool SupportsTopics { get; }
    }

    public interface ITopicPublisher : IMessagePublisher
    {
        string Topic { get; }
    }

    public static class MessagePublisherExtensions
    {
        public static Task PublishAsync<T>(this IMessagePublisher publisher, T message, TimeSpan? delay = null) where T : class
        {
            return publisher.PublishAsync(typeof(T), message, delay);
        }
    }
}