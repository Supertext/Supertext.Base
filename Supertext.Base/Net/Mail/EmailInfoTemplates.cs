using System.Collections.Generic;

namespace Supertext.Base.Net.Mail
{

    public class EmailInfoTemplates<TDynamicData>
    {
        public EmailInfoTemplates(string templateId,
                                  TDynamicData dynamicTemplateData,
                                  PersonInfo from,
                                  PersonInfo to,
                                  string subject)
        {
            TemplateId = templateId;
            DynamicTemplateData = dynamicTemplateData;
            From = from;
            To = to;
            Subject = subject;
            Recipients = new List<PersonInfo> { to };
        }

        public EmailInfoTemplates(string templateId,
                                  TDynamicData dynamicTemplateData,
                                  PersonInfo from,
                                  ICollection<PersonInfo> to,
                                  string subject)
        {
            TemplateId = templateId;
            DynamicTemplateData = dynamicTemplateData;
            From = from;
            Recipients = to;
            Subject = subject;
        }

        public string TemplateId { get; }

        public TDynamicData DynamicTemplateData { get; }

        public PersonInfo From { get; }

        public PersonInfo To { get; }

        public ICollection<PersonInfo> Recipients { get; }

        public string Subject { get; set; }
    }
}
