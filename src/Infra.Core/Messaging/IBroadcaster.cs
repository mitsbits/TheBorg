using Borg.Infra.Messaging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Borg.Infra.Messaging
{
    public interface IBroadcaster
    {
        Task Broadcast(string[] topics, Type messageType, object message, TimeSpan? delay = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}

namespace Borg
{
    public static class MessagePublisherExtensions
    {
        private const string ALL = "ALL";
        public static Task BroadcastTopics<T>(this IBroadcaster broadcaster, string[] topics, T message, TimeSpan? delay = null) where T : class
        {
            if (topics == null || !topics.Distinct().Any()) throw new ArgumentNullException(nameof(topics));
            if (topics.Length == 1 && topics[0].Equals(ALL, StringComparison.InvariantCultureIgnoreCase)) throw new ArgumentException(nameof(topics));
            return broadcaster.Broadcast(topics, typeof(T), message, delay);
        }

        public static Task BroadcastMessage<T>(this IBroadcaster broadcaster, T message, TimeSpan? delay = null) where T : class
        {
            return broadcaster.Broadcast(new string[0], typeof(T), message, delay);
        }

        public static Task BroadcastAll<T>(this IBroadcaster broadcaster, T message, TimeSpan? delay = null) where T : class
        {
            return broadcaster.Broadcast(new[] { ALL }, typeof(T), message, delay);
        }
    }
}