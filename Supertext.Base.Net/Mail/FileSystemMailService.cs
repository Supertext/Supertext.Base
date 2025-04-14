using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Supertext.Base.Net.Mail
{
    internal class FileSystemMailService : IMailService
    {
        private static ILogger<IMailService> _logger;
        private readonly MailServiceConfig _mailServiceConfig;

        public FileSystemMailService(ILogger<IMailService> logger, MailServiceConfig mailServiceConfig)
        {
            _logger = logger;
            _mailServiceConfig = mailServiceConfig;
        }

        public Task SendAsync(EmailInfo mail, CancellationToken ct = default)
        {
            SendInternal(mail, false);

            return Task.CompletedTask;
        }

        public Task SendAsHtmlAsync(EmailInfo mail, CancellationToken ct = default)
        {
            SendInternal(mail, true);

            return Task.CompletedTask;
        }

        public Task SendUsingTemplateAsync<TDynamicTemplateData>(EmailInfoTemplates<TDynamicTemplateData> mailInfo, CancellationToken ct = default)
        {
            _logger.LogInformation($"{nameof(SendUsingTemplateAsync)}: TemplateData: {mailInfo.DynamicTemplateData}");
            _logger.LogInformation($"{nameof(SendUsingTemplateAsync)}: No email will be sent from {nameof(FileSystemMailService)}.");

            return Task.CompletedTask;
        }

        private void SendInternal(EmailInfo emailInfo, bool isHtml)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(emailInfo.From.Name, emailInfo.From.Email));
            message.To.AddRange(emailInfo.Recipients.Select(recipient => new MailboxAddress(recipient.Name, recipient.Email)));
            message.Subject = emailInfo.Subject;

            var body = new BodyBuilder();
            if (isHtml)
            {
                body.HtmlBody = emailInfo.Message;
            }
            else
            {
                body.TextBody = emailInfo.Message;
            }

            foreach (var attachmentInfo in emailInfo.Attachments)
            {
                body.Attachments.Add(attachmentInfo.Name, attachmentInfo.Content);
            }

            message.Body = body.ToMessageBody();

            var outputPath = Path.Combine(_mailServiceConfig.LocalEmailDirectory, $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.eml");
            using (var stream = File.Create(outputPath))
            {
                try
                {
                    message.WriteTo(stream);
                }
                catch (Exception ex)
                {
                    var logMsg = $"Failed to save email to {outputPath}.";
                    _logger.LogError(ex, logMsg);
                    throw new Exception(logMsg, ex);
                }
            }

            Console.WriteLine($"Email saved to: {outputPath}");
        }
    }
}