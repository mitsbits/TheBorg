using System.Collections.Generic;

namespace Borg.Framework.Postal
{
    public class PostalSettings
    {
        public List<SmtpSpec> Smtps { get; set; }
        public List<ImapSpec> Imaps { get; set; }
    }
}