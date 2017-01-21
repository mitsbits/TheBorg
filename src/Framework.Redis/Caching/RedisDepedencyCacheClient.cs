using Borg.Infra;
using Borg.Infra.BuildingBlocks;
using Borg.Infra.Caching;
using Borg.Infra.Messaging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Borg.Framework.Redis
{
    public sealed class RedisDepedencyCacheClient : IDepedencyCacheClient
    {
        private readonly RedisCacheClient _internal;
        private readonly IMessageBus _subscriber;

        private readonly Func<Type, string> _entityKey = (t) => t.Name;

        public RedisDepedencyCacheClient(ConnectionMultiplexer connectionMultiplexer, IMessageBus subscriber, ISerializer serializer = null)
        {
            _internal = new RedisCacheClient(connectionMultiplexer, serializer);
            //_subscriber = new RedisMessageBus(connectionMultiplexer.GetSubscriber(), Constants.CACHE_DEPEDENCY_TOPIC, serializer);
            subscriber.Subscribe<EntityCacheDepedencyEvictionEvent>(message => Invalidate((message as IEntityCacheDepedencyEvictionEvent).Type, message.Keys));
            _subscriber = subscriber;
        }

        #region ICacheClient

        public void Dispose()
        {
            _internal.Dispose();
        }

        public Task<int> RemoveAllAsync(IEnumerable<string> keys = null)
        {
            return _internal.RemoveAllAsync(keys);
        }

        public Task<int> RemoveByPrefixAsync(string prefix)
        {
            return _internal.RemoveByPrefixAsync(prefix);
        }

        public Task<CacheValue<T>> GetAsync<T>(string key)
        {
            return _internal.GetAsync<T>(key);
        }

        public Task<IDictionary<string, CacheValue<T>>> GetAllAsync<T>(IEnumerable<string> keys)
        {
            return _internal.GetAllAsync<T>(keys);
        }

        public Task<bool> AddAsync<T>(string key, T value, TimeSpan? expiresIn = null)
        {
            return _internal.AddAsync(key, value, expiresIn);
        }

        public Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiresIn = null)
        {
            return _internal.SetAsync(key, value, expiresIn);
        }

        public Task<int> SetAllAsync<T>(IDictionary<string, T> values, TimeSpan? expiresIn = null)
        {
            return _internal.SetAllAsync(values, expiresIn);
        }

        public Task<bool> ReplaceAsync<T>(string key, T value, TimeSpan? expiresIn = null)
        {
            return _internal.ReplaceAsync(key, value, expiresIn);
        }

        public Task<double> IncrementAsync(string key, double amount = 1, TimeSpan? expiresIn = null)
        {
            return _internal.IncrementAsync(key, amount, expiresIn);
        }

        public Task<bool> ExistsAsync(string key)
        {
            return _internal.ExistsAsync(key);
        }

        public Task<TimeSpan?> GetExpirationAsync(string key)
        {
            return _internal.GetExpirationAsync(key);
        }

        public Task SetExpirationAsync(string key, TimeSpan expiresIn)
        {
            return _internal.SetExpirationAsync(key, expiresIn);
        }

        public Task<double> SetIfHigherAsync(string key, double value, TimeSpan? expiresIn = null)
        {
            return _internal.SetIfHigherAsync(key, value, expiresIn);
        }

        public Task<double> SetIfLowerAsync(string key, double value, TimeSpan? expiresIn = null)
        {
            return _internal.SetIfLowerAsync(key, value, expiresIn);
        }

        public Task<long> SetAddAsync<T>(string key, IEnumerable<T> value, TimeSpan? expiresIn = null)
        {
            return _internal.SetAddAsync(key, value, expiresIn);
        }

        public Task<long> SetRemoveAsync<T>(string key, IEnumerable<T> value, TimeSpan? expiresIn = null)
        {
            return _internal.SetRemoveAsync(key, value, expiresIn);
        }

        public Task<CacheValue<ICollection<T>>> GetSetAsync<T>(string key)
        {
            return _internal.GetSetAsync<T>(key);
        }

        Task<int> ICacheClient.RemoveAllAsync(IEnumerable<string> keys)
        {
            throw new NotImplementedException();
        }

        Task<int> ICacheClient.RemoveByPrefixAsync(string prefix)
        {
            throw new NotImplementedException();
        }

        Task<CacheValue<T>> ICacheClient.GetAsync<T>(string key)
        {
            throw new NotImplementedException();
        }

        Task<IDictionary<string, CacheValue<T>>> ICacheClient.GetAllAsync<T>(IEnumerable<string> keys)
        {
            throw new NotImplementedException();
        }

        Task<bool> ICacheClient.AddAsync<T>(string key, T value, TimeSpan? expiresIn)
        {
            throw new NotImplementedException();
        }

        Task<bool> ICacheClient.SetAsync<T>(string key, T value, TimeSpan? expiresIn)
        {
            throw new NotImplementedException();
        }

        Task<int> ICacheClient.SetAllAsync<T>(IDictionary<string, T> values, TimeSpan? expiresIn)
        {
            throw new NotImplementedException();
        }

        Task<bool> ICacheClient.ReplaceAsync<T>(string key, T value, TimeSpan? expiresIn)
        {
            throw new NotImplementedException();
        }

        Task<double> ICacheClient.IncrementAsync(string key, double amount, TimeSpan? expiresIn)
        {
            throw new NotImplementedException();
        }

        Task<bool> ICacheClient.ExistsAsync(string key)
        {
            throw new NotImplementedException();
        }

        Task<TimeSpan?> ICacheClient.GetExpirationAsync(string key)
        {
            throw new NotImplementedException();
        }

        Task ICacheClient.SetExpirationAsync(string key, TimeSpan expiresIn)
        {
            throw new NotImplementedException();
        }

        Task<double> ICacheClient.SetIfHigherAsync(string key, double value, TimeSpan? expiresIn)
        {
            throw new NotImplementedException();
        }

        Task<double> ICacheClient.SetIfLowerAsync(string key, double value, TimeSpan? expiresIn)
        {
            throw new NotImplementedException();
        }

        Task<long> ICacheClient.SetAddAsync<T>(string key, IEnumerable<T> value, TimeSpan? expiresIn)
        {
            throw new NotImplementedException();
        }

        Task<long> ICacheClient.SetRemoveAsync<T>(string key, IEnumerable<T> value, TimeSpan? expiresIn)
        {
            throw new NotImplementedException();
        }

        Task<CacheValue<ICollection<T>>> ICacheClient.GetSetAsync<T>(string key)
        {
            throw new NotImplementedException();
        }

        #endregion ICacheClient

        #region ICacheDepedencyManager

        async Task ICacheDepedencyManager.Add(string key, Type entiType, PartitionedKey[] identifiers)
        {
            var entityCacheKeys = identifiers.Select(x => $"{_entityKey(entiType)}:{x.ToString()}").ToArray();
            var cacheValues = await _internal.GetAllAsync<string[]>(entityCacheKeys);
            var valuesToAdd = new Dictionary<string, string>();
            foreach (var entityCacheKey in entityCacheKeys)
            {
                if (cacheValues.ContainsKey(entityCacheKey))
                {
                    var localvalue = cacheValues[entityCacheKey];
                    if (localvalue.HasValue)
                    {
                        if (localvalue.Value.Contains(key)) continue;
                        var bucket = new List<string>(localvalue.Value) { key };
                        await _internal.SetAsync(entityCacheKey, bucket.ToArray());
                    }
                    else
                    {
                        valuesToAdd.Add(entityCacheKey, key);
                    }
                }
                else
                {
                    valuesToAdd.Add(entityCacheKey, key);
                }
            }
            var tasks = valuesToAdd.Select(x => _internal.AddAsync(x.Key, new[] { x.Value }));
            var addOperations = tasks as Task<bool>[] ?? tasks.ToArray();
            if (addOperations.Any()) await Task.WhenAll(addOperations).AnyContext();
        }

        public async Task Invalidate(Type entiType, PartitionedKey[] identifiers)
        {
            var entityCacheKeys = identifiers.Select(x => $"{_entityKey(entiType)}:{x.ToString()}").ToArray();
            var cachedValues = await _internal.GetAllAsync<string[]>(entityCacheKeys);
            var pointers = cachedValues.SelectMany(x => (x.Value.HasValue) ? x.Value.Value.ToArray() : new string[0]);
            var keystoremove = pointers.Union(entityCacheKeys).Distinct().ToArray();
            if (keystoremove.Any()) await _internal.RemoveAllAsync(keystoremove);
        }

        #endregion ICacheDepedencyManager
    }
}