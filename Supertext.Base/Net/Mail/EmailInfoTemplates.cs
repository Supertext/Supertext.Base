using System.Collections.Generic;

namespace Supertext.Base.Net.Mail
{

    public class EmailInfoTemplates<TDynamicData>
    {
        public EmailInfoTemplates(string templateId,
                                  TDynamicData dynamicTemplateData,
                                  PersonInfo from,
                                  PersonInfo to)
        {
            TemplateId = templateId;
            DynamicTemplateData = dynamicTemplateData;
            From = from;
            To = to;
            Recipients = new List<PersonInfo> { to };
        }

        public EmailInfoTemplates(string templateId,
                                  TDynamicData dynamicTemplateData,
                                  PersonInfo from,
                                  ICollection<PersonInfo> to)
        {
            TemplateId = templateId;
            DynamicTemplateData = dynamicTemplateData;
            From = from;
            Recipients = to;
        }

        public string TemplateId { get; }

        public TDynamicData DynamicTemplateData { get; }

        public PersonInfo From { get; }

        public PersonInfo To { get; }

        public ICollection<PersonInfo> Recipients { get; }
    }
}
