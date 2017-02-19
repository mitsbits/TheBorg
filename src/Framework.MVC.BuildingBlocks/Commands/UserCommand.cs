using Borg.Infra.CQRS;
using System.Security.Principal;

namespace Borg.Framework.MVC.Commands
{
    public abstract class UserCommand : ICommand
    {
        protected UserCommand(IIdentity user)
        {
            User = user;
        }

        public IIdentity User { get; }
    }
}