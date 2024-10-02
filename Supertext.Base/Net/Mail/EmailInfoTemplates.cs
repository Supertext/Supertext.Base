using System.Collections.Generic;

namespace Supertext.Base.Net.Mail
{
    /// <summary>
    /// Represents an email template with dynamic data and recipient information.
    /// </summary>
    /// <typeparam name="TDynamicData">The type of the dynamic data which will be serialised into JSON. </typeparam>
    /// <example>
    /// Sample of TDynamicData:
    /// <code>
    /// public class ConfirmEmailData
    /// {
    ///     public AlertRow AlertRow { get; set; }
    ///     public Message Message { get; set; }
    ///     public Footer Footer { get; set; }
    ///     public string Address { get; set; }
    ///     public string Subject { get; set; }
    /// }
    ///
    /// public class AlertRow
    /// {
    ///     public string Column1 { get; set; }
    ///     public string Column2 { get; set; }
    /// }
    ///
    /// public class Footer
    /// {
    ///     public string FooterMessage { get; set; }
    /// }
    ///
    /// public class Message
    /// {
    ///     public string Salutation { get; set; }
    ///     public string Message1 { get; set; }
    /// }
    /// </code>
    /// </example>
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
