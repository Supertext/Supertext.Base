using System;
using Supertext.Base.Configuration;

namespace Supertext.Base.Net.Mail
{
    [ConfigSection("MailServiceSettings")]
    public class MailServiceConfig: IConfiguration
    {
        public string SendGridHost { get; set; }

        public string SendGridUsername { get; set; }

        public bool SendGridEnabled { get; set; }

        public int SendGridPort { get; set; } = 587;

        public string LocalEmailDirectory { get; set; }

        [KeyVaultSecret]
        public string SendGridPassword { get; set; }
    }
}
