namespace Borg.Framework.Identity
{
    public class IdentityServerSettings
    {
        public DatabaseSettings Database { get; set; }
    }

    public class DatabaseSettings
    {
        public string ConnectionString { get; set; }
    }

    public class IdentityClientSettings
    {
    }
}