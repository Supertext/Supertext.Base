using System.Collections.Generic;

namespace Supertext.Base.Net.Mail
{
    public class EmailInfoTemplates
    {
        public EmailInfoTemplates(string templateId, string dynamicTemplateDataAsJson, PersonInfo from, PersonInfo to)
        {
            TemplateId = templateId;
            DynamicTemplateDataAsJson = dynamicTemplateDataAsJson;
            From = from;
            To = to;
            Recipients = new List<PersonInfo> { to };
        }

        public EmailInfoTemplates(string templateId, string dynamicTemplateDataAsJson, PersonInfo from, ICollection<PersonInfo> to)
        {
            TemplateId = templateId;
            DynamicTemplateDataAsJson = dynamicTemplateDataAsJson;
            From = from;
            Recipients = to;
        }

        public string TemplateId { get; }

        public string DynamicTemplateDataAsJson { get; }

        public PersonInfo From { get; }

        public PersonInfo To { get; }

        public ICollection<PersonInfo> Recipients { get; }
    }
}
