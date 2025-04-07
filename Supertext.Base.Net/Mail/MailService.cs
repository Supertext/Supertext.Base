using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Email;
using Aspose.Email.Clients;
using Aspose.Email.Clients.Smtp;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using Supertext.Base.Exceptions;
using Attachment = Aspose.Email.Attachment;

namespace Supertext.Base.Net.Mail
{
    internal class MailService : IMailService
    {
        private static ILogger<IMailService> _logger;
        private readonly MailServiceConfig _mailServiceConfig;

        public MailService(ILogger<IMailService> logger, MailServiceConfig mailServiceConfig)
        {
            _logger = logger;
            _mailServiceConfig = mailServiceConfig;
        }

        public async Task SendAsync(EmailInfo mail, CancellationToken ct = default)
        {
            try
            {
                await SendInternalAsync(mail,
                                        message =>
                                        {
                                            message.IsBodyHtml = false;
                                            message.Body = mail.Message;
                                        },
                                        ct)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var recipients = String.Join("; ", mail.Recipients.Select(r => r.Email));
                _logger.LogError(ex, $"{nameof(SendAsync)}: Couldn't send email. To={recipients}; Subject={mail.Subject}");
                throw;
            }
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

        public async Task SendAsHtmlAsync(EmailInfo mail, CancellationToken ct = default)
        {
            try
            {
                await SendInternalAsync(mail,
                                        (message) =>
                                        {
                                            message.IsBodyHtml = true;
                                            message.HtmlBody = mail.Message;
                                        },
                                        ct)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var recipients = String.Join("; ", mail.Recipients.Select(r => r.Email));
                _logger.LogError(ex, $"{nameof(SendAsHtmlAsync)}: Couldn't send email. To={recipients}; Subject={mail.Subject}");
                throw;
            }
        }

        private async Task SendInternalAsync(EmailInfo mail, Action<MailMessage> handleHtml, CancellationToken ct = default)
        {
            using (var msg = new MailMessage())
            {
                CreateEmail(mail, msg, handleHtml);

                using (var client = new SmtpClient())
                {
                    if (_mailServiceConfig.SendGridEnabled)
                    {
                        SetSendGridClient(client);
                    }
                    else
                    {
                        client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                        client.PickupDirectoryLocation = _mailServiceConfig.LocalEmailDirectory;
                    }

                    try
                    {
                        foreach (var att in mail.Attachments)
                        {
                            var attachment = ConvertAttachmentInfoToAttachment(att);
                            msg.Attachments.Add(attachment);
                        }

                        await client.SendAsync(msg, ct).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        var to = String.Join("; ", mail.Recipients.Select(r => r.Email));
                        _logger.LogError(ex, $"Sending an email to {to} with subject '{mail.Subject}' failed.");
                        throw;
                    }

                    // We are deliberately not explicitly disposing of the Attachment objects.
                    // - Aspose.Email.Clients.Smtp.SendAsync spawns a new thread and sends the email there, which means
                    //   we can't be certain that the attachments and their underlying streams have been fully consumed
                    //   at this point. We had occurrences of 'corrupted' attachments which couldn't be opened, despite
                    //   the blob (in the storage container) being valid.
                    //   So our solution is to leave the Attachment objects to the Garbage Collector.
                }

                var recipients = String.Join("; ", mail.Recipients.Select(r => r.Email));
                _logger.LogInformation($"Email sent. To={recipients}; Subject={mail.Subject}.");
            }
        }

        private static void CreateEmail(EmailInfo mail, MailMessage msg, Action<MailMessage> handleHtml)
        {
            handleHtml(msg);
            msg.BodyEncoding = Encoding.UTF8;
            msg.From = new MailAddress(mail.From.Email, mail.From.Name);
            msg.Subject = mail.Subject;
            msg.SubjectEncoding = Encoding.UTF8;

            foreach (var recipient in mail.Recipients)
            {
                msg.To.Add(new MailAddress(recipient.Email, recipient.Name));
            }

            if (!String.IsNullOrEmpty(mail.BccEmail))
            {
                msg.Bcc.Add(new MailAddress(mail.BccEmail));
            }

            if (!String.IsNullOrEmpty(mail.ReplyTo))
            {
                msg.ReplyToList = new MailAddress(mail.ReplyTo, mail.ReplyToName);
            }
        }

        private void SetSendGridClient(SmtpClient client)
        {
            var port = _mailServiceConfig.SendGridPort;

            client.SecurityOptions = SecurityOptions.SSLExplicit;
            client.UseAuthentication = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Host = _mailServiceConfig.SendGridHost;
            client.Password = _mailServiceConfig.SendGridPassword;
            client.Port = port;
            client.Username = _mailServiceConfig.SendGridUsername;

            if (String.IsNullOrWhiteSpace(client.Host))
            {
                Console.WriteLine("The SendGrid 'host' property was not set in the mailServiceConfig's AppSettings.");
                throw new ConfigurationException("The SendGrid 'host' property was not set in the mailServiceConfig's AppSettings.");
            }

            if (String.IsNullOrWhiteSpace(client.Password))
            {
                Console.WriteLine("The SendGrid 'password' property was not set in the mailServiceConfig's AppSettings.");
                throw new ConfigurationException("The SendGrid 'password' property was not set in the mailServiceConfig's AppSettings.");
            }

            if (String.IsNullOrWhiteSpace(client.Username))
            {
                Console.WriteLine("The SendGrid 'username' property was not set in the mailServiceConfig's AppSettings.");
                throw new ConfigurationException("The SendGrid 'username' property was not set in the mailServiceConfig's AppSettings.");
            }
        }

        private static Attachment ConvertAttachmentInfoToAttachment(AttachmentInfo attachmentInfo)
        {
            var stream = ConvertToStream(attachmentInfo.Content);
            return new Attachment(stream, attachmentInfo.Name);
        }

        private static Stream ConvertToStream(byte[] content)
        {
            var memStream = new MemoryStream();
            memStream.Write(content, 0, content.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            return memStream;
        }
    }
}