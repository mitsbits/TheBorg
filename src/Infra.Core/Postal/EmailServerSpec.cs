

namespace Borg.Infra.Postal
{
    public abstract class EmailServerSpec : NamedSpec
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string UseName { get; set; }
        public string Password { get; set; }
        public bool SSL { get; set; }
    }
}