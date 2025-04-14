using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Supertext.Base.Net.Mail
{
    internal class SendGridMailService : IMailService
    {
        private static ILogger<IMailService> _logger;
        private readonly MailServiceConfig _mailServiceConfig;

        public SendGridMailService(ILogger<SendGridMailService> logger, MailServiceConfig mailServiceConfig)
        {
            _logger = logger;
            _mailServiceConfig = mailServiceConfig;
        }

        public async Task SendAsync(EmailInfo mail, CancellationToken ct = default)
        {
            await SendInternalAsync(mail, false, ct).ConfigureAwait(false);
        }

        public async Task SendAsHtmlAsync(EmailInfo mail, CancellationToken ct = default)
        {
            await SendInternalAsync(mail, true, ct).ConfigureAwait(false);
        }

        public async Task SendUsingTemplateAsync<TDynamicTemplateData>(EmailInfoTemplates<TDynamicTemplateData> mailInfo, CancellationToken ct = default)
        {
            try
            {
                _logger.LogDebug($"{nameof(SendUsingTemplateAsync)} - TemplateData: {mailInfo.DynamicTemplateData}");
                var options = new SendGridClientOptions
                              {
                                  ApiKey = _mailServiceConfig.SendGridPassword
                              };
                var client = new SendGridClient(options);
                var message = new SendGridMessage
                              {
                                  TemplateId = mailInfo.TemplateId,
                                  From = ConvertToSendGridEmailAddress(mailInfo.From),
                              };
                message.SetTemplateData(mailInfo.DynamicTemplateData);
                message.AddTos(mailInfo.Recipients.Select(ConvertToSendGridEmailAddress).ToList());

                var response = await client.SendEmailAsync(message, ct).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Couldn't send email via SendGrid API. Status code: {response.StatusCode}, error: {await response.Body.ReadAsStringAsync()}");
                }
            }
            catch (Exception ex)
            {
                var recipients = String.Join("; ", mailInfo.Recipients.Select(r => r.Email));
                _logger.LogError(ex, $"{nameof(SendAsHtmlAsync)}: Couldn't send email. To={recipients}");
                throw;
            }

            EmailAddress ConvertToSendGridEmailAddress(PersonInfo personInfo)
            {
                return new EmailAddress(personInfo.Email, personInfo.Name);
            }
        }

        private async Task SendInternalAsync(EmailInfo emailInfo, bool isHtml, CancellationToken ct = default)
        {
            var apiKey = _mailServiceConfig.SendGridPassword;
            var msg = CreateEmail(emailInfo, isHtml);
            var client = new SendGridClient(apiKey);

            ct.ThrowIfCancellationRequested();

            var response = await client.SendEmailAsync(msg, ct).ConfigureAwait(false);

            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                var logMsg = $"Couldn't send email via SendGrid API. Status code: {response.StatusCode}, error: {await response.Body.ReadAsStringAsync().ConfigureAwait(false)}";
                _logger.LogInformation(logMsg);
                throw new Exception(logMsg);
            }

            _logger.LogInformation($"Email sent. To={String.Join(", ", emailInfo.Recipients.Select(recipient => recipient.Email))}; Subject={emailInfo.Subject}.");
        }

        private static SendGridMessage CreateEmail(EmailInfo emailInfo, bool isHtml)
        {
            const string pdfMimeType = "application/pdf";

            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(new EmailAddress(emailInfo.From.Email, emailInfo.From.Name),
                                                                       emailInfo.Recipients.Select(recipient => new EmailAddress(recipient.Email, recipient.Name)).ToList(),
                                                                       emailInfo.Subject,
                                                                       isHtml ? String.Empty : emailInfo.Message,
                                                                       isHtml ? emailInfo.Message : String.Empty);

            if (!String.IsNullOrEmpty(emailInfo.BccEmail))
            {
                msg.AddBcc(new EmailAddress(emailInfo.BccEmail));
            }

            if (!String.IsNullOrEmpty(emailInfo.ReplyTo))
            {
                msg.ReplyTo = new EmailAddress(emailInfo.ReplyTo, emailInfo.ReplyToName);
            }

            foreach (var attachment in emailInfo.Attachments)
            {
                var base64Pdf = Convert.ToBase64String(attachment.Content);
                msg.AddAttachment(attachment.Name, base64Pdf, pdfMimeType);
            }

            return msg;
        }
    }
}