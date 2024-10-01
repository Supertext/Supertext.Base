using System.Collections.Generic;

namespace Supertext.Base.Net.Mail
{
    public class EmailInfoTemplates
    {
        public EmailInfoTemplates(string templateId,
                                  string dynamicTemplateDataAsJson,
                                  PersonInfo from,
                                  PersonInfo to,
                                  string subject)
        {
            TemplateId = templateId;
            DynamicTemplateDataAsJson = dynamicTemplateDataAsJson;
            From = from;
            To = to;
            Subject = subject;
            Recipients = new List<PersonInfo> { to };
        }

        public EmailInfoTemplates(string templateId,
                                  string dynamicTemplateDataAsJson,
                                  PersonInfo from,
                                  ICollection<PersonInfo> to,
                                  string subject)
        {
            TemplateId = templateId;
            DynamicTemplateDataAsJson = dynamicTemplateDataAsJson;
            From = from;
            Recipients = to;
            Subject = subject;
        }

        public string TemplateId { get; }

        public string DynamicTemplateDataAsJson { get; }

        public PersonInfo From { get; }

        public PersonInfo To { get; }

        public ICollection<PersonInfo> Recipients { get; }

        public string Subject { get; set; }
    }
}
