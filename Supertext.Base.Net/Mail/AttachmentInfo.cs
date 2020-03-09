using Supertext.Base.Common;

namespace Supertext.Base.Net.Mail
{
    public class AttachmentInfo
    {
        public byte[] Content { get; }

        public string Name { get; }

        public AttachmentInfo(byte[] content, string name)
        {
            Validate.NotEmpty(content, nameof(content));
            Validate.NotNullOrWhitespace(name, nameof(name));
            Content = content;
            Name = name;
        }
    }
}
