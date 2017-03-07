namespace Borg.Framework.UserNotifications
{
    public class UserNotificationsViewModel
    {
        public IUserNotification[] UserNotifications { get; set; } = new IUserNotification[0];
    }
}