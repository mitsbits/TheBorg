namespace Borg.Framework.Postal
{
    public class SmtpSpec : EmailServerSpec
    {
        public int BatchSize { get; set; } = 16;

        public string[] TrackHeadersOnSend { get; set; } = new[] { "X-Mailer" };
    }
}