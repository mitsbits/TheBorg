using System;
using Borg.Infra.Messaging;

namespace Borg.Framework.System
{
    public class ServerResponse : IServerResponse
    {
        public ResponseStatus Status { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }

        protected ServerResponse()
        {
        }

        public ServerResponse(ResponseStatus status, string title, string message)
        {
            Status = status;
            Title = title;
            Message = message;
        }

        public ServerResponse(Exception exc)
        {
            if (exc is ApplicationException)
            {
                Status = ResponseStatus.Warning;
                Title = "Application Error";
                Message = exc.Message;
            }
            else
            {
                Status = ResponseStatus.Error;
                Title = exc.GetType().ToString();
                Message = exc.Message;
            }

        }
    }
}