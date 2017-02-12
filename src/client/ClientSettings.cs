namespace Borg.Client
{
    public class ClientSettings
    {
        public DatabaseSettings Database { get; set; }
    }

    public class DatabaseSettings
    {
        public string ConnectionString { get; set; }
    }
}