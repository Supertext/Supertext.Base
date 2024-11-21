﻿using System.Collections.Generic;

namespace Supertext.Base.Net.Mail
{
    public class EmailInfo
    {
        public EmailInfo(string subject, string message, PersonInfo from, PersonInfo to)
        {
            Subject = subject;
            Message = message;
            From = from;
            To = to;
            Recipients = new List<PersonInfo> { to };
        }

        public EmailInfo(string subject, string message, PersonInfo from, ICollection<PersonInfo> to)
        {
            Subject = subject;
            Message = message;
            From = from;
            Recipients = to;
        }

        public string Subject { get; }

        public string Message { get; }

        public PersonInfo From { get; }

        public PersonInfo To { get; }

        public ICollection<PersonInfo> Recipients { get; }

        public string BccEmail { get; set; }

        public string ReplyTo { get; set; }

        public string ReplyToName { get; set; }

        public List<AttachmentInfo> Attachments { get; } = new List<AttachmentInfo>();
    }
}
