﻿using Borg.Infra;
using Borg.Infra.Core.Messaging;
using Borg.Infra.Messaging;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Borg.Framework.Redis.Messaging
{
    public class RedisMessageBus : MessageBusBase, IMessageBus, ITopicPublisher
    {
        private readonly ISubscriber _subscriber;
        private readonly string _topic;
        private readonly ISerializer _serializer;
        private static readonly object _lockObject = new object();
        private bool _isSubscribed;

        public RedisMessageBus(ISubscriber subscriber, string topic = null, ISerializer serializer = null, ILoggerFactory loggerFactory = null) : base(loggerFactory)
        {
            _subscriber = subscriber;
            _topic = topic ?? "messages";
            _serializer = serializer ?? new JsonNetSerializer();
        }

        private void EnsureTopicSubscription()
        {
            if (_isSubscribed)
                return;

            lock (_lockObject)
            {
                if (_isSubscribed)
                    return;

                //_logger.Trace("Subscribing to topic: {0}", _topic);
                _subscriber.Subscribe(_topic, OnMessage);
                _isSubscribed = true;
            }
        }

        private async void OnMessage(RedisChannel channel, RedisValue value)
        {
            //_logger.Trace("OnMessage: {channel}", channel);

            var message = await _serializer.DeserializeAsync<MessageBusData>((string)value).AnyContext();

            Type messageType;
            try
            {
                messageType = Type.GetType(message.Type);
            }
            catch (Exception ex)
            {
                //_logger.Error(ex, "Error getting message body type: {0}", ex.Message);
                return;
            }

            object body = await _serializer.DeserializeAsync(message.Data, messageType).AnyContext();
            await SendMessageToSubscribersAsync(messageType, body).AnyContext();
        }

        public override async Task PublishAsync(Type messageType, object message, TimeSpan? delay = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (message == null)
                return;

            if (delay.HasValue && delay.Value > TimeSpan.Zero)
            {
                //_logger.Trace("Schedule delayed message: {messageType} ({delay}ms)", messageType.FullName, delay.Value.TotalMilliseconds);
                await AddDelayedMessageAsync(messageType, message, delay.Value).AnyContext();
                return;
            }

            // _logger.Trace("Message Publish: {messageType}", messageType.FullName);
            var data = await _serializer.SerializeAsync(new MessageBusData
            {
                Type = messageType.AssemblyQualifiedName,
                Data = await _serializer.SerializeToStringAsync(message).AnyContext()
            }).AnyContext();

            await Run.WithRetriesAsync(() => _subscriber.PublishAsync(_topic, data, CommandFlags.FireAndForget)/*, logger: _logger*/, cancellationToken: cancellationToken).AnyContext();
        }

        public override void Subscribe<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default(CancellationToken))
        {
            EnsureTopicSubscription();
            base.Subscribe(handler, cancellationToken);
        }

        //public  void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                base.Dispose();

                if (_isSubscribed)
                {
                    lock (_lockObject)
                    {
                        if (!_isSubscribed)
                            return;

                        _subscriber.UnsubscribeAll(CommandFlags.FireAndForget);
                        _isSubscribed = false;
                    }
                }
            }
        }

        public Task Stop(CancellationToken token = new CancellationToken())
        {
            return Task.CompletedTask;
        }

        public string Topic => _topic;
        public override bool SupportsTopics => true;
    }
}