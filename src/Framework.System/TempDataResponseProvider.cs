using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Borg.Infra;

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
            var bucket = context.TempData[key] as IEnumerable<string>;
            if (bucket == null || !bucket.Any()) return Empty();
            return new TempDataResponseProvider(bucket.Select(x => serializer.DeserializeAsync<ServerResponse>(x).Result));
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
            
            var source = controller.TempData[TempDataResponseProvider.Key] as IEnumerable<IServerResponse>;
            var bucket = source == null ? new List<string>() : new List<string>(source.Select(x => serializer.SerializeToStringAsync(x).Result));
            bucket.AddRange(messages.Select(x => serializer.SerializeToStringAsync(x).Result));
            controller.TempData[TempDataResponseProvider.Key] = bucket;
        }
    }

    public class ServerResponse : IServerResponse
    {
        public ServerResponseStatus Status { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }

        protected ServerResponse()
        {
        }

        public ServerResponse(ServerResponseStatus status, string title, string message)
        {
            Status = status;
            Title = title;
            Message = message;
        }

        public ServerResponse(Exception exc)
        {
            if (exc is ApplicationException)
            {
                Status = ServerResponseStatus.Warning;
                Title = "Application Error";
                Message = exc.Message;
            }
            else
            {
                Status = ServerResponseStatus.Error;
                Title = exc.GetType().ToString();
                Message = exc.Message;
            }

        }
    }
}