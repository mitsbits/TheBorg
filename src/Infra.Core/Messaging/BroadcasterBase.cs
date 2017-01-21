using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Borg.Infra.Messaging
{
    public abstract class BroadcasterBase : IBroadcaster
    {
        protected readonly IEnumerable<IMessagePublisher> _publishers;

        protected BroadcasterBase(IEnumerable<IMessagePublisher> publishers)
        {
            if (publishers == null) throw new ArgumentNullException(nameof(publishers));
            _publishers = publishers;
        }

        public virtual Task Broadcast(string[] topics, Type messageType, object message, TimeSpan? delay = default(TimeSpan?), CancellationToken cancellationToken = default(CancellationToken))
        {
            var broadcastEmpty = !topics.Any();
            var broadcastAll = topics.Length == 1 & topics[0].Equals("ALL", StringComparison.InvariantCultureIgnoreCase);
            var sanitized = topics.Select(x => x.Trim()).Distinct().Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            var broadcastTopics = sanitized.Any();

            List<Task> tasks = null;

            if (broadcastTopics)
            {
                tasks = new List<Task>(
                    _publishers.Where(x => x.SupportsTopics)
                        .Cast<ITopicPublisher>()
                        .Where(x => sanitized.Contains(x.Topic.ToLower()))
                        .Select(x => x.PublishAsync(messageType, message, delay, cancellationToken)));
            }
            else
            {
                if (broadcastAll)
                {
                    tasks = new List<Task>(
                        _publishers
                            .Select(x => x.PublishAsync(messageType, message, delay, cancellationToken)));
                }
                else
                {
                    if (broadcastEmpty)
                    {
                        tasks = new List<Task>(
                            _publishers.Where(x => !x.SupportsTopics)
                                .Select(x => x.PublishAsync(messageType, message, delay, cancellationToken)));
                    }
                }
            }

            return tasks != null && tasks.Any() ? Task.WhenAll(tasks) : Task.CompletedTask;
        }
    }
}