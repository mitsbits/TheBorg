namespace Borg.Infra.Postal
{
    public class MailAddress
    {
        private readonly string _email;

        public MailAddress(string email)
        {
            if (!email.WellFormedEmail()) throw new EmailFormatException(email);
            _email = email;
        }

        public MailAddress(string email, string displayName) : this(email)
        {
            DisplayName = displayName;
        }

        public string Email => _email.ToLower();
        public string DisplayName { get; }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(DisplayName) ? Email : $"{Email} [{DisplayName}]";
        }
    }
}