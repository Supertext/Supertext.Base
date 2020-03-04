using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Email.Clients.Activity;

namespace Supertext.Base.Net.Mail
{
    public class AttachmentInfo
    {
        public byte[] Content { set; get; }

        public string Name { set; get; }

        public AttachmentInfo(byte[] content, string name)
        {
            Content = content;
            Name = name;
        }
    }
}
