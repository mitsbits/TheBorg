namespace Borg.Framework.System
{
    public interface IServerResponse
    {
        ServerResponseStatus Status { get; set; }

        string Title { get; set; }

        string Message { get; set; }
    }
}