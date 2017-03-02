using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Borg.Infra;
using Borg.Infra.Core.Messaging;

namespace Borg.Framework.System
{
    public class TempDataResponseProvider : IServerResponseProvider
    {
        internal const string Key = "796FF7DF-924D-4C78-80D8-C8149ACBBE4B";

  

        private TempDataResponseProvider(IEnumerable<IServerResponse> bucket)
        {
            Messages = new HashSet<IServerResponse>(bucket);

        }

        public ICollection<IServerResponse> Messages { get; }

        public static TempDataResponseProvider Create(ViewContext context, ISerializer serializer)
        {
            return Create(context, Key, serializer);
        }

        private static  TempDataResponseProvider Create(ViewContext context, string key, ISerializer serializer)
        {
            if (context.TempData == null) return Empty();
            if (!context.TempData.ContainsKey(key)) return Empty();
            var bucket = AsyncHelpers.RunSync(()=> serializer.DeserializeAsync<List<ServerResponse>>( context.TempData[key].ToString()));
            if (bucket == null || !bucket.Any()) return Empty();
            return new TempDataResponseProvider(bucket);
        }

        private static TempDataResponseProvider Empty()
        {
            return new TempDataResponseProvider(new List<IServerResponse>());
        }
    }

    public static class TempDataResponseProviderExtentions
    {
        public static void AddRedirectMessages(this Controller controller, ISerializer serializer, params ServerResponse[] messages)
        {
            var txt = controller.TempData[TempDataResponseProvider.Key]?.ToString() ?? string.Empty;
            var source = AsyncHelpers.RunSync(()=> serializer.DeserializeAsync<List<ServerResponse>>(txt));
            var bucket = source == null ? new List<ServerResponse>() : new List<ServerResponse>(source.Select(x => x));
            bucket.AddRange(messages);
            controller.TempData[TempDataResponseProvider.Key] = AsyncHelpers.RunSync(() => serializer.SerializeToStringAsync(bucket));
        }
    }
}