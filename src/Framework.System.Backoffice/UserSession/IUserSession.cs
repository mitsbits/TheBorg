using System;
using System.Linq;
using System.Threading.Tasks;
using Borg;
using Borg.Framework.System.Backoffice.UserSession;
using Org.BouncyCastle.Asn1.Sec;

namespace Borg.Framework.System.Backoffice.UserSession
{
    public interface IUserSession
    {
        DateTimeOffset SessionStart { get; }
        string UserIdentifier { get; }

        T Setting<T>(string key, T value );
        T Setting<T>(string key);
    }
}

namespace Borg
{
    public static class IUserSessionExtensions
    {
        private static readonly string MenuIsCollapsedKey = "Borg.MenuIsCollapsed";
        public static bool MenuIsCollapsed(this IUserSession userSession)
        {
            return userSession.Setting<bool>(MenuIsCollapsedKey);
        }
        public static bool MenuIsCollapsed(this IUserSession userSession, bool value)
        {
            return userSession.Setting(MenuIsCollapsedKey, value);
        }
    }
}