using System;
using System.Collections.Generic;
using System.Text;

namespace Supertext.Base.Net.Mail
{
    public class EmailInfo
    {
        public string Subject { set; get; }

        public string Message { set; get; }

        public PersonInfo From { get; set; }

        public PersonInfo To { get; set; }

        public string BccEmail { get; set; }

        public string ReplyTo { get; set; }

        public string ReplyToName { get; set; }

        public List<AttachmentInfo> Attachments { get; } = new List<AttachmentInfo>();
    }
}
