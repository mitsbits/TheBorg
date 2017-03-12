using System.Collections.Generic;
using Borg.Infra.Postal;

namespace Borg.Framework.Postal
{
    public class PostalSettings
    {
        public List<SmtpSpec> Smtps { get; set; }
        public List<ImapSpec> Imaps { get; set; }
    }
}