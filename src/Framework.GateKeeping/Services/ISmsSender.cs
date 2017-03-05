using System.Threading.Tasks;

namespace Borg.Framework.GateKeeping.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
